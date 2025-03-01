using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Requests;
using Application.Exceptions.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Playlists
{
    public class AddSongToPlaylist
    {
        public class Command : IRequest<Unit>
        {
            public required PlaylistSongRequest req { get; set; }
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

                var songAlreadyExists = _context.PlaylistSongRelations
                    .Any(r => r.SongId == cmd.req.SongId);

                if (songAlreadyExists == true)
                {
                    throw new Exception("Song already exists so it can not be added to playlist!");
                }

                int lastOrder = _context.PlaylistSongRelations
                    .OrderByDescending(r => r.PositionInPlaylist)
                    .Select(r => r.PositionInPlaylist)
                    .FirstOrDefault();

                playlist.PlaylistSongRelations.Add(new Domain.PlaylistSongRelation { PlaylistId = cmd.req.PlaylistId, SongId = cmd.req.SongId, PositionInPlaylist = lastOrder + 1 });

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}