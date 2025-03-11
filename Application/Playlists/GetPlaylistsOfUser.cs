using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Playlists
{
    public class GetPlaylistsOfUser
    {
        public class Query : IRequest<List<Playlist>>
        {
            public required Guid UserId { get; set; }
            public int Count { get; set; } = 10;
        }
        public class Handler : IRequestHandler<Query, List<Playlist>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<Playlist>> Handle(Query query, CancellationToken cancellationToken)
            {
                int count = query.Count <= 30 ? query.Count : 15;

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

                if (user == null)
                {
                    throw new ArgumentException();
                }

                List<Guid> playlistIds = new List<Guid>();

                List<Guid>? lastPlayedPlaylists = await _context.PlaylistPlayRecords
                    .Where(pr => pr.UserId == query.UserId)
                    .OrderByDescending(pr => pr.Timestamp)
                    .Select(pr => pr.PlayedItemId)
                    .Distinct()
                    .Take(count)
                    .ToListAsync(cancellationToken);

                if (lastPlayedPlaylists == null)
                {
                    lastPlayedPlaylists = new List<Guid>();
                }

                if (lastPlayedPlaylists != null)
                {
                    playlistIds.AddRange(lastPlayedPlaylists);
                }

                if (playlistIds.Count < count)
                {
                    List<Guid>? remainingPlaylists = await _context.Playlists
                        .Where(p => p.UserId == query.UserId && !lastPlayedPlaylists.Contains(p.Id))
                        .OrderBy(p => p.CreatedAt)
                        .Take(count - lastPlayedPlaylists.Count)
                        .Select(p => p.Id)
                        .ToListAsync(cancellationToken);

                    if (remainingPlaylists != null)
                    {
                        playlistIds.AddRange(remainingPlaylists);
                    }
                }

                var playlists = await _context.Playlists
                    .Where(p => playlistIds.Contains(p.Id))
                    .Include(p => p.PlaylistSongRelations)
                    .Include(p => p.User)
                    .ToListAsync(cancellationToken);

                return playlists;
            }
        }
    }
}