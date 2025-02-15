using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects;
using Application.DataTransferObjects.Responses;
using Application.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Search
{
    public class GlobalSearch
    {
        public class Query : IRequest<GlobalSearchResult>
        {
            public required string SearchString { get; set; }
        }

        public class Handler : IRequestHandler<Query, GlobalSearchResult>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<GlobalSearchResult> Handle(Query query, CancellationToken cancellationToken)
            {
                var songs = await _context.Songs
                    .Where(s => s.Title.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    .Include(s => s.Album)
                    .OrderBy(s => s.Title)
                    .Select(s => new SearchResult
                    {
                        Type = "Song",
                        Id = s.Id,
                        Name = s.Title,
                        ImageLocation = s.Album.ImageLocation
                    })
                    .Take(20)
                    .ToListAsync(cancellationToken);

                var albums = await _context.Albums
                    .Where(a => a.Name.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(s => s.Name)
                    .Select(a => new SearchResult { Type = "Album", Id = a.Id, Name = a.Name, ImageLocation = a.ImageLocation })
                    .Take(20)
                    .ToListAsync(cancellationToken);

                var artists = await _context.Artists
                    .Where(art => art.Name.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(s => s.Name)
                    .Select(art => new SearchResult { Type = "Artist", Id = art.Id, Name = art.Name, ImageLocation = art.ImageLocation })
                    .Take(20)
                    .ToListAsync(cancellationToken);

                return new GlobalSearchResult
                {
                    Songs = songs,
                    Albums = albums,
                    Artists = artists
                };
            }
        }

    }
}