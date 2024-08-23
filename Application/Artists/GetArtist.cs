using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Artists
{
    public class GetArtist
    {
        public class Query : IRequest<Artist?>
        {
            public Guid? Id { get; set; } = null;
            public string? Name { get; set; } = null;
        }
        public class Handler : IRequestHandler<Query, Artist?>
        {
            ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Artist?> Handle(Query query, CancellationToken cancellationToken)
            {
                if (query.Id == null && query.Name == null)
                {
                    return null;
                }

                IQueryable<Artist> artistsQuery = _context.Artists;

                if (!String.IsNullOrWhiteSpace(query.Name))
                {
                    artistsQuery = artistsQuery.Where(a => a.Name == query.Name);
                }

                if (query.Id != null)
                {
                    artistsQuery = artistsQuery.Where(a => a.Id == query.Id);
                }

                var Artist = await artistsQuery.
                    FirstOrDefaultAsync(cancellationToken);

                return Artist;
            }
        }
    }
}