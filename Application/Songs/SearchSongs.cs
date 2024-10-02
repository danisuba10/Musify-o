using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Songs
{
    public class SearchSongs
    {
        public class Query : IRequest<List<Song>>
        {
            public string? Title { get; set; }
            public List<String>? Artists { get; set; }
            public string? AlbumName { get; set; }
            public bool IncludeAlbum { get; set; } = false;
            public bool IncludeArtists { get; set; } = false;
        }

        public class Handler : IRequestHandler<Query, List<Song>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<Song>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Songs
                    .Include(s => s.SongArtistRelations)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.Title))
                {
                    query = query.Where(s => s.Title.ToLower().Contains(request.Title.ToLower()));
                }

                if (!string.IsNullOrEmpty(request.AlbumName))
                {
                    query = query.Where(s => s.Album.Name.ToLower().Contains(request.AlbumName.ToLower()));
                }


                if (request.Artists != null && request.Artists.Any())
                {
                    foreach (var artist in request.Artists)
                    {
                        String searchedArtist = artist.ToLower();
                        query = query.Where(s =>
                            s.SongArtistRelations.Any(ar => ar.Artist.Name.Contains(searchedArtist)));
                    }
                }

                if (request.IncludeAlbum)
                {
                    query = query.Include(s => s.Album);
                }

                if (request.IncludeArtists)
                {
                    query = query = query
                        .Include(s => s.SongArtistRelations)
                            .ThenInclude(sar => sar.Artist);
                }

                var songs = await query.ToListAsync(cancellationToken);

                return songs;
            }
        }
    }
}