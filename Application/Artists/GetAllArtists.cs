using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Artists
{
    public class GetAllArtists
    {
        public class Query : IRequest<IEnumerable<Artist>> { }
        public class Handler : IRequestHandler<Query, IEnumerable<Artist>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<IEnumerable<Artist>> Handle(Query query, CancellationToken cancellationToken)
            {
                var Artists = await _context.Artists.ToListAsync(cancellationToken);
                return Artists;
            }
        }
    }
}