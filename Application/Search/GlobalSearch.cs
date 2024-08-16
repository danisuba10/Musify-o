using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Search
{
    public class GlobalSearch
    {
        public class Query : IRequest<List<SearchResult>>
        {
            public required string SearchString { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<SearchResult>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<SearchResult>> Handle(Query query, CancellationToken cancellationToken)
            {
                var songs = await _context.Songs
                    .Where(s => s.Title.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    .Select(s => new SearchResult { Type = "Song", Id = s.Id, Name = s.Title, ImgPath = null })
                    .ToListAsync(cancellationToken);

                var albums = await _context.Albums
                    .Where(a => a.Name.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    .Select(a => new SearchResult { Type = "Album", Id = a.Id, Name = a.Name, ImgPath = a.ImageLocation })
                    .ToListAsync(cancellationToken);

                var artists = await _context.Artists
                    .Where(art => art.Name.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    .Select(art => new SearchResult { Type = "Artist", Id = art.Id, Name = art.Name, ImgPath = art.ImageLocation })
                    .ToListAsync(cancellationToken);

                var result = songs.Concat(albums).Concat(artists).ToList();

                return result;
            }
        }

    }
}