using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Requests;
using Application.Exceptions.Common;
using Domain;
using MediatR;
using Persistence;

namespace Application.Playlists
{
    public class AddPlaylist
    {
        public class Command : IRequest<Unit>
        {
            public required AddPlaylistRequest dto { get; set; }
            public required Guid UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Command cmd, CancellationToken cancellationToken)
            {
                Playlist playlist = new Playlist
                {
                    Name = cmd.dto.Name,
                    UserId = (Guid)cmd.UserId,
                    Description = string.IsNullOrWhiteSpace(cmd.dto.Description) ? "" : cmd.dto.Description
                };

                await _context.Playlists.AddAsync(playlist, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                if (cmd.dto.FirstSongId == null)
                {
                    throw new NotExistingObjectExceptions("First song in playlist");
                }

                var songExists = _context.Songs
                        .Any(s => s.Id == cmd.dto.FirstSongId);
                if (songExists)
                {
                    playlist.PlaylistSongRelations.Add(
                        new PlaylistSongRelation { PlaylistId = playlist.Id, SongId = (Guid)cmd.dto.FirstSongId }
                    );
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}