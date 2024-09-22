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
            public Guid Id { get; set; }
            public bool IncludeSongs { get; set; } = false;
            public bool IncludeArtists { get; set; } = false;
        }

        public class Handler : IRequestHandler<Query, Album>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Album> Handle(Query query, CancellationToken cancellationToken)
            {
                IQueryable<Album> albumQuery = _context.Albums.AsQueryable();

                if (query.IncludeSongs)
                {
                    albumQuery = albumQuery
                                    .Include(a => a.Songs);
                }

                if (query.IncludeArtists)
                {
                    albumQuery = albumQuery
                                    .Include(a => a.AlbumArtistRelations)
                                        .ThenInclude(aar => aar.Artist);
                }

                Album? album = await albumQuery
                    .FirstOrDefaultAsync(a => a.Id == query.Id);

                if (album == null)
                {
                    throw new Exception($"Album with ID {query.Id} not found");
                }

                return album;
            }
        }
    }
}