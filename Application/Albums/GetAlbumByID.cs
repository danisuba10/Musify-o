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
    public class GetAlbumByID
    {
        public class Query : IRequest<Album>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Album>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Album> Handle(Query request, CancellationToken cancellationToken)
            {
                var album = await _context.Albums
                    .Include(a => a.Songs)
                    .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

                if (album == null)
                {
                    throw new Exception($"Album with ID {request.Id} not found");
                }

                return album;
            }
        }
    }
}