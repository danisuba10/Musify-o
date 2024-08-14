using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Songs
{
    public class SearchSongs
    {
        public class Query : IRequest<List<Song>>
        {
            public string? SongTitle { get; set; }
            public string? ArtistName { get; set; }
            public string? AlbumName { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<Song>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<Song>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Songs
                    .Include(s => s.Album)
                        .ThenInclude(a => a.AlbumArtistRelations)
                            .ThenInclude(aar => aar.Artist)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.SongTitle))
                {
                    query = query.Where(s => s.Title.Contains(request.SongTitle, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(request.AlbumName))
                {
                    query = query.Where(s => s.Album.Name.Contains(request.AlbumName, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(request.ArtistName))
                {
                    query = query.Where(s => s.Album.AlbumArtistRelations
                        .Any(aar => aar.Artist.Name.Contains(request.ArtistName, StringComparison.OrdinalIgnoreCase)));
                }

                var songs = await query.ToListAsync(cancellationToken);

                return songs;
            }
        }
    }
}