using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Albums
{
    public class GetAllAlbums
    {
        public class Query : IRequest<List<Album>> { }

        public class Handler : IRequestHandler<Query, List<Album>>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<Album>> Handle(Query request, CancellationToken cancellationToken)
            {
                var albums = await _context.Albums
                    .Include(a => a.Songs)
                    .ToListAsync(cancellationToken);

                if (albums == null)
                {
                    return new List<Album>();
                }

                return albums;
            }
        }
    }
}