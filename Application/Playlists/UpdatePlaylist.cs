using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Requests;
using Application.Exceptions.Common;
using Application.Images;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Persistence;

namespace Application.Playlists
{
    public class UpdatePlaylist
    {
        public class Command : IRequest<Unit>
        {
            public required Guid Id { get; set; }
            public required PlaylistUpdateRequest request { get; set; }
            public required Guid UserId { get; set; }
            public string? Role { get; set; }
            public required string ImageFolderPath { get; set; }
        }
        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            public Handler(ApplicationDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }
            public async Task<Unit> Handle(Command cmd, CancellationToken cancellationToken)
            {
                var playlist = await _context.Playlists
                    .FirstOrDefaultAsync(pl => pl.Id == cmd.Id);

                if (playlist == null)
                {
                    throw new NotExistingObjectExceptions("Playlist");
                }

                if (playlist.UserId != cmd.UserId && !(cmd.Role?.Equals("Admin") ?? false))
                {
                    throw new UnauthorizedAccessException("User does not have permission to update playlist!");
                }

                if (!String.IsNullOrWhiteSpace(cmd.request.Name))
                {
                    playlist.Name = cmd.request.Name;
                }

                if (!String.IsNullOrWhiteSpace(cmd.request.Description))
                {
                    playlist.Description = cmd.request.Description;
                }

                if (cmd.request.Visibility != null)
                {
                    playlist.Visibility = (Domain.Visibility)cmd.request.Visibility;
                }


                Console.WriteLine("Image null:" + (cmd.request.Image == null).ToString());
                if (cmd.request.Image != null)
                {
                    Console.WriteLine("Image not null!");
                    try
                    {
                        await _mediator.Send(new UploadImage.Command { Name = cmd.Id.ToString(), formFile = cmd.request.Image, Path = Path.Combine(cmd.ImageFolderPath, "playlist") });
                        playlist.ImageLocation = Path.Combine("playlist", playlist.Id + ".jpg");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Update album error: Image upload failed:\n", ex);
                    }
                }

                Console.WriteLine("Request description - result" + cmd.request.Description + playlist.Description);
                _context.Playlists.Update(playlist);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}