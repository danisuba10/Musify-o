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
    public class GetSongByID
    {
        public class Query : IRequest<Song>
        {
            public Guid Id { get; set; }
            public bool IncludeAlbum { get; set; } = false;
            public bool IncludeArtists { get; set; } = false;
        }

        public class Handler : IRequestHandler<Query, Song>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Song> Handle(Query query, CancellationToken cancellationToken)
            {

                IQueryable<Song> songQuery = _context.Songs.AsQueryable();

                if (query.IncludeAlbum)
                {
                    songQuery = songQuery
                                    .Include(s => s.Album);
                }

                if (query.IncludeArtists)
                {
                    songQuery = songQuery
                                    .Include(s => s.SongArtistRelations)
                                        .ThenInclude(sar => sar.Artist);
                }

                Song? song = await songQuery.
                    FirstOrDefaultAsync(s => s.Id == query.Id, cancellationToken);

                if (song == null)
                {
                    throw new Exception($"Song with ID {query.Id} does not exists.");
                }

                return song;
            }

        }
    }
}