using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Songs;

namespace Application.Albums
{
    public class AddSongsToAlbum
    {
        public class Query : IRequest<int>
        {
            public Guid AlbumId { get; set; }
            public List<Guid> SongIds { get; set; } = new List<Guid>();
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

                var albumSongs = new List<Song>();
                foreach (Guid songId in query.SongIds)
                {
                    var song = await _context.Songs
                        .FirstOrDefaultAsync(s => s.Id == songId && s.AlbumId == null, cancellationToken);

                    if (song != null)
                    {
                        song.AlbumId = query.AlbumId;
                        albumSongs.Add(song);
                    }

                }

                if (albumSongs.Count > 0)
                {
                    _context.Songs.UpdateRange(albumSongs);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return query.SongIds.Count - albumSongs.Count;
            }
        }
    }
}