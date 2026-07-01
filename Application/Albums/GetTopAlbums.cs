using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Albums
{
    public class GetTopAlbums
    {
        public class Query : IRequest<List<Album>>
        {
            public int Count { get; set; } = 25;
        }

        public class Handler : IRequestHandler<Query, List<Album>>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<Album>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Get the top album IDs by save count from UserLibraryItems
                var topAlbumIds = await _context.UserLibraryItems
                    .Where(uli => uli.ItemType == "Album")
                    .GroupBy(uli => uli.ItemId)
                    .OrderByDescending(g => g.Count())
                    .Take(request.Count)
                    .Select(g => g.Key)
                    .ToListAsync(cancellationToken);

                // Fetch the actual Album entities in the correct order
                var albums = await _context.Albums
                    .Where(a => topAlbumIds.Contains(a.Id))
                    .ToListAsync(cancellationToken);

                // Preserve the ranking order (most saved first)
                var albumDict = albums.ToDictionary(a => a.Id);
                return topAlbumIds
                    .Select(id => albumDict.GetValueOrDefault(id))
                    .Where(a => a != null)
                    .Select(a => a!)
                    .ToList();
            }
        }
    }
}
