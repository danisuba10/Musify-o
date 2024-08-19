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
    public class GetArtistByName
    {
        public class Query : IRequest<Artist?>
        {
            public required string Name { get; set; }
        }

        public class Handler : IRequestHandler<Query, Artist?>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Artist?> Handle(Query query, CancellationToken cancellationToken)
            {
                var artist = await _context.Artists
                    .FirstOrDefaultAsync(art => art.Name == query.Name);

                return artist;
            }
        }
    }
}