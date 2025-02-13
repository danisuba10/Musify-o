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

                var albumSongs = await _context.Songs
                    .Where(s => query.SongIds.Contains(s.Id) && s.AlbumId == null)
                    .ToListAsync(cancellationToken);

                foreach (Song song in albumSongs)
                {
                    song.AlbumId = query.AlbumId;
                }

                if (albumSongs.Count > 0)
                {
                    if (query.Replace)
                    {
                        var existingSongs = await _context.Songs
                            .Where(s => s.AlbumId == query.AlbumId)
                            .ToListAsync(cancellationToken);
                        _context.Songs.RemoveRange(existingSongs);
                        _context.Songs.AddRange(albumSongs);
                    }
                    else
                    {
                        _context.Songs.UpdateRange(albumSongs);
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return query.SongIds.Count - albumSongs.Count;
            }
        }
    }
}