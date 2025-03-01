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
                List<Guid> playlistIds = new List<Guid>();

                List<Guid>? lastPlayedPlaylists = await _context.PlaylistPlayRecords
                    .Where(pr => pr.UserId == query.UserId)
                    .OrderByDescending(pr => pr.Timestamp)
                    .Take(10)
                    .Select(pr => pr.PlayedItemId)
                    .ToListAsync(cancellationToken);

                if (lastPlayedPlaylists != null)
                {
                    playlistIds.AddRange(lastPlayedPlaylists);
                }

                if (playlistIds.Count < 10)
                {
                    List<Guid>? remainingPlaylists = await _context.Playlists
                        .Where(p => p.UserId == query.UserId && !lastPlayedPlaylists.Contains(p.Id))
                        .OrderBy(p => p.CreatedAt)
                        .Take(10 - lastPlayedPlaylists.Count)
                        .Select(p => p.Id)
                        .ToListAsync(cancellationToken);

                    if (remainingPlaylists != null)
                    {
                        playlistIds.AddRange(remainingPlaylists);
                    }
                }

                var playlists = await _context.Playlists
                    .Where(p => playlistIds.Contains(p.Id))
                    .ToListAsync(cancellationToken);

                return playlists;
            }
        }
    }
}