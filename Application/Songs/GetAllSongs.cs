using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Songs
{
    public class GetAllSongs
    {
        public class Query : IRequest<List<Song>> { }
        public class Handler : IRequestHandler<Query, List<Song>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<Song>> Handle(Query query, CancellationToken cancellationToken)
            {
                var songs = await _context.Songs
                    .Include(s => s.Album)
                        .ThenInclude(a => a.AlbumArtistRelations)
                            .ThenInclude(aar => aar.Artist)
                    .ToListAsync(cancellationToken);

                return songs;
            }
        }
    }
}