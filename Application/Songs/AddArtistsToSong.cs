using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Exceptions.Song;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Songs
{
    public class AddArtistsToSong
    {
        public class Query : IRequest<int>
        {
            public Guid SongId { get; set; }
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
                var songExists = await _context.Songs
                    .AnyAsync(a => a.Id == query.SongId, cancellationToken);

                if (!songExists)
                {
                    throw new SongDoesNotExistException();
                }

                var songArtistRelations = new List<SongArtistRelation>();

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
                        songArtistRelations.Add(new SongArtistRelation { SongId = query.SongId, ArtistId = artistId });
                    }
                    else
                    {
                        var relationAlreadyExists = await _context.SongArtistRelations
                            .AnyAsync(sar => sar.SongId == query.SongId && sar.ArtistId == artistId, cancellationToken);

                        if (!relationAlreadyExists)
                        {
                            songArtistRelations.Add(new SongArtistRelation { SongId = query.SongId, ArtistId = artistId });
                        }
                    }

                    if (songArtistRelations.Count > 0)
                    {
                        if (query.Replace)
                        {
                            var existingRelations = _context.SongArtistRelations
                                .Where(sar => sar.SongId == query.SongId);
                            _context.SongArtistRelations.RemoveRange(existingRelations);
                            await _context.SaveChangesAsync(cancellationToken);
                        }
                        await _context.AddRangeAsync(songArtistRelations, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }
                return query.ArtistIds.Count - songArtistRelations.Count;
            }
        }
    }
}