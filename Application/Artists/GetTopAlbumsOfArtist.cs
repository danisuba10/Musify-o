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
    public class GetTopAlbumsOfArtist
    {
        public class Query : IRequest<List<Album>>
        {
            public required Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<Album>>
        {
            ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public Task<List<Album>> Handle(Query query, CancellationToken cancellationToken)
            {
                var albums = _context.Albums
                    .Include(a => a.AlbumArtistRelations)
                    .Where(a => a.AlbumArtistRelations.Any(ar => ar.ArtistId == query.Id))
                    .Take(10)
                    .ToListAsync(cancellationToken);

                return albums;
            }
        }
    }
}