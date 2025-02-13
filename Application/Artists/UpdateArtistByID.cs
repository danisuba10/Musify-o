using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Application.Images;

namespace Application.Artists
{
    public class UpdateArtistByID
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public IFormFile? File { get; set; }
            public string ImageFolderPath { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            public Handler(ApplicationDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                var existingArtist = await _context.Artists.FirstOrDefaultAsync(art => art.Id == command.Id);

                if (existingArtist == null)
                {
                    throw new Exception("Update artist error: Artist does not exist! Can't edit it.");
                }

                if (!String.IsNullOrEmpty(command.Name))
                {
                    existingArtist.Name = command.Name;
                }

                if (command.File != null)
                {
                    try
                    {
                        await _mediator.Send(new UploadImage.Command { Name = command.Id.ToString(), formFile = command.File, Path = Path.Combine(command.ImageFolderPath, "artist") });
                        existingArtist.ImageLocation = Path.Combine("artist/", command.Id.ToString() + ".jpg");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Update artist error: Image upload failed:\n", ex);
                    }
                }

                _context.Artists.Update(existingArtist);
                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}