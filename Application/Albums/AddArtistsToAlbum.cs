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
    public class AddArtistsToAlbum
    {
        public class Query : IRequest<int>
        {
            public Guid AlbumId { get; set; }
            public List<Guid> ArtistIds { get; set; }
        }
        public class Handler : IRequestHandler<Query, int>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(Query query, CancellationToken cancellationToken)
            {
                var albumArtistRelations = new List<AlbumArtistRelation>();

                foreach (Guid artistId in query.ArtistIds)
                {
                    var exists = await _context.AlbumArtistRelations
                        .AnyAsync(aar => aar.AlbumId == query.AlbumId && aar.ArtistId == artistId, cancellationToken);
                    if (!exists)
                    {
                        albumArtistRelations.Add(new AlbumArtistRelation { AlbumId = query.AlbumId, ArtistId = artistId });
                    }
                }

                if (albumArtistRelations.Count > 0)
                {
                    await _context.AlbumArtistRelations.AddRangeAsync(albumArtistRelations, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return query.ArtistIds.Count - albumArtistRelations.Count;
            }
        }
    }
}