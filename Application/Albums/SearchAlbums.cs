using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Albums
{
    public class SearchAlbums
    {
        public class Query : IRequest<List<Album>?>
        {
            public string? Name { get; set; } = null;
            public List<String>? Artists { get; set; } = null;
            public List<String>? Songs { get; set; } = null;
            public bool IncludeSongs { get; set; } = false;
            public bool IncludeArtists { get; set; } = false;
            //All artists must be present, not just some of them
            public bool AllArtistsPresent { get; set; } = false;
        }

        public class Handler : IRequestHandler<Query, List<Album>?>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<Album>?> Handle(Query query, CancellationToken cancellationToken)
            {
                if (query.Name == null && query.Artists == null)
                {
                    return null;
                }

                IQueryable<Album> albumsQuery = _context.Albums;

                if (!String.IsNullOrWhiteSpace(query.Name))
                {
                    albumsQuery = albumsQuery.Where(a => a.Name == query.Name);
                }

                if (query.AllArtistsPresent)
                {
                    if (query.Artists != null && query.Artists.Any())
                    {
                        foreach (var artist in query.Artists)
                        {
                            albumsQuery = albumsQuery.Where(a =>
                                a.AlbumArtistRelations.Any(ar => ar.Artist.Name == artist));
                        }
                    }
                }
                else
                {
                    if (query.Artists != null && query.Artists.Any())
                    {
                        albumsQuery = albumsQuery
                            .Where(a => a.AlbumArtistRelations
                                .Any(ar => query.Artists.Contains(ar.Artist.Name)));
                    }
                }

                if (query.Songs != null && query.Songs.Any())
                {
                    foreach (var song in query.Songs)
                    {
                        String searchedSong = song.ToLower();
                        albumsQuery = albumsQuery
                            .Where(a => a.Songs
                                .Any(s => s.Title.ToLower().Contains(searchedSong)));
                    }
                }

                if (query.IncludeSongs)
                {
                    albumsQuery = albumsQuery
                                    .Include(a => a.Songs);
                }

                if (query.IncludeArtists)
                {
                    albumsQuery = albumsQuery
                                    .Include(a => a.AlbumArtistRelations)
                                        .ThenInclude(aar => aar.Artist);
                }

                List<Album>? Albums = await albumsQuery
                                        .ToListAsync(cancellationToken);

                return Albums;
            }
        }

    }
}