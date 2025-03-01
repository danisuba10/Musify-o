using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Application.DataTransferObjects.Requests;
using Application.Exceptions.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Playlists
{
    public class RemoveSongFromPlaylist
    {
        public class Command : IRequest<Unit>
        {
            public required PlaylistSongRequest req;
            public required Guid UserId { get; set; }
            public string? Role { get; set; }
        }
        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Command cmd, CancellationToken cancellationToken)
            {
                var playlist = await _context.Playlists
                    .FirstOrDefaultAsync(pl => pl.Id == cmd.req.PlaylistId, cancellationToken);

                if (playlist == null)
                {
                    throw new NotExistingObjectExceptions("Playlist");
                }

                if (playlist.UserId != cmd.UserId && !(cmd.Role?.Equals("Admin") ?? false))
                {
                    throw new UnauthorizedAccessException();
                }

                var songRelation = playlist.PlaylistSongRelations
                    .FirstOrDefault(psr => psr.SongId == cmd.req.SongId);

                if (songRelation == null)
                {
                    throw new NotExistingObjectExceptions("Song in Playlist");
                }

                playlist.PlaylistSongRelations.Remove(songRelation);

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }

    }
}