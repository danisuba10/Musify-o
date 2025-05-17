using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using MediatR;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Images;
using Domain;

namespace Application.Artists
{
    public class RemoveArtistByID
    {
        public class Command : IRequest<string>
        {
            public Guid Id { get; set; }
            public required string ImageFolderPath { get; set; }
            public required string SoundFolderPath { get; set; }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            public Handler(ApplicationDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }

            public async Task<string> Handle(Command command, CancellationToken cancellationToken)
            {
                var artist = await _context.Artists
                    .Include(a => a.AlbumArtistRelations)
                        .ThenInclude(aar => aar.Album)
                    .FirstOrDefaultAsync(art => art.Id == command.Id, cancellationToken);

                if (artist == null)
                {
                    throw new Exception("Artist does not exist!Can't be deleted!");
                }

                string imageLocation = artist.ImageLocation;

                var albums = artist.AlbumArtistRelations.Select(aar => aar.Album).ToList();

                if (albums != null && albums.Count > 0)
                {
                    foreach (Album album in albums)
                    {
                        await _mediator.Send(new RemoveAlbumByID.Command
                        { Id = album.Id, ImageFolderPath = command.ImageFolderPath, SoundFolderPath = command.SoundFolderPath });
                    }
                }

                _context.Artists.Remove(artist);
                await _context.SaveChangesAsync(cancellationToken);

                await _mediator.Send(new DeleteImage.Command { Path = Path.Combine(command.ImageFolderPath, imageLocation), RootImagePath = command.ImageFolderPath });

                return imageLocation;
            }
        }
    }
}