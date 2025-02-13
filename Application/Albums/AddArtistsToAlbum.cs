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
            public bool Replace { get; set; } = false;
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
                var albumExists = await _context.Albums
                    .AnyAsync(a => a.Id == query.AlbumId, cancellationToken);

                if (!albumExists)
                {
                    throw new Exception("Album does not exist!");
                }

                var albumArtistRelations = new List<AlbumArtistRelation>();

                foreach (Guid artistId in query.ArtistIds)
                {
                    var artistExists = await _context.Artists
                        .AnyAsync(a => a.Id == artistId, cancellationToken);

                    if (!artistExists)
                    {
                        continue;
                    }

                    if (query.Replace)
                    {
                        albumArtistRelations.Add(new AlbumArtistRelation { AlbumId = query.AlbumId, ArtistId = artistId });
                    }
                    else
                    {
                        var relationAlreadyExists = await _context.AlbumArtistRelations
                        .AnyAsync(aar => aar.AlbumId == query.AlbumId && aar.ArtistId == artistId, cancellationToken);
                        if (!relationAlreadyExists)
                        {
                            albumArtistRelations.Add(new AlbumArtistRelation { AlbumId = query.AlbumId, ArtistId = artistId });
                        }
                    }
                }

                if (albumArtistRelations.Count > 0)
                {
                    if (query.Replace)
                    {
                        var existingRelations = _context.AlbumArtistRelations
                            .Where(aar => aar.AlbumId == query.AlbumId);
                        _context.AlbumArtistRelations.RemoveRange(existingRelations);
                    }
                    await _context.AlbumArtistRelations.AddRangeAsync(albumArtistRelations, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return query.ArtistIds.Count - albumArtistRelations.Count;
            }
        }
    }
}