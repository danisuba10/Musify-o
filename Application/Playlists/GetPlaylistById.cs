using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Application.Exceptions.Common;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Playlists
{
    public class GetPlaylistById
    {
        public class Query : IRequest<Playlist>
        {
            public required Guid Id { get; set; }
            public required Guid? UserId { get; set; }
            public required string? Role { get; set; }
        }
        public class Handler : IRequestHandler<Query, Playlist>
        {
            ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Playlist> Handle(Query query, CancellationToken cancellationToken)
            {
                var playlist = await _context.Playlists
                    .Include(pl => pl.PlaylistSongRelations)
                        .ThenInclude(pl => pl.Song)
                    .Include(pl => pl.User)
                    .FirstOrDefaultAsync(pl => pl.Id == query.Id, cancellationToken);

                if (playlist == null)
                {
                    throw new NotExistingObjectExceptions("Playlist");
                }

                if (playlist.Visibility == Visibility.Private)
                {
                    if (playlist.UserId != query.UserId && !(query.Role?.Equals("Admin") ?? false))
                    {
                        throw new UnauthorizedAccessException("Can't view playlist as it is private and not created by current user!");
                    }
                }

                return playlist;
            }
        }
    }
}