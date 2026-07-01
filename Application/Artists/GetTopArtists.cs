using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Artists
{
    public class GetTopArtists
    {
        public class Query : IRequest<List<Artist>>
        {
            public int Count { get; set; } = 25;
        }

        public class Handler : IRequestHandler<Query, List<Artist>>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<Artist>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Get the top artist IDs by save count from UserLibraryItems
                var topArtistIds = await _context.UserLibraryItems
                    .Where(uli => uli.ItemType == "Artist")
                    .GroupBy(uli => uli.ItemId)
                    .OrderByDescending(g => g.Count())
                    .Take(request.Count)
                    .Select(g => g.Key)
                    .ToListAsync(cancellationToken);

                // Fetch the actual Artist entities in the correct order
                var artists = await _context.Artists
                    .Where(a => topArtistIds.Contains(a.Id))
                    .ToListAsync(cancellationToken);

                // Preserve the ranking order (most saved first)
                var artistDict = artists.ToDictionary(a => a.Id);
                return topArtistIds
                    .Select(id => artistDict.GetValueOrDefault(id))
                    .Where(a => a != null)
                    .Select(a => a!)
                    .ToList();
            }
        }
    }
}
