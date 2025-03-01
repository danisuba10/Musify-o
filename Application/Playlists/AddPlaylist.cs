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
        public class Command : IRequest<Guid>
        {
            public required AddPlaylistRequest dto { get; set; }
            public required Guid UserId { get; set; }
            public required Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Guid>
        {
            ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Guid> Handle(Command cmd, CancellationToken cancellationToken)
            {
                Playlist playlist = new Playlist
                {
                    Id = cmd.Id,
                    Name = cmd.dto.Name,
                    UserId = (Guid)cmd.UserId,
                    Description = string.IsNullOrWhiteSpace(cmd.dto.Description) ? "" : cmd.dto.Description
                };

                await _context.Playlists.AddAsync(playlist, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                Console.WriteLine("First save ok!");

                if (cmd.dto.FirstSongId == null)
                {
                    throw new NotExistingObjectExceptions("First song in playlist");
                }

                var songExists = _context.Songs
                        .Any(s => s.Id == cmd.dto.FirstSongId);
                if (songExists)
                {
                    playlist.PlaylistSongRelations.Add(
                        new PlaylistSongRelation { PlaylistId = playlist.Id, SongId = (Guid)cmd.dto.FirstSongId, PositionInPlaylist = 1 }
                    );
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return playlist.Id;
            }
        }
    }
}