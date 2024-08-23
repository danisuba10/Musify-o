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
    public class GetAlbum
    {
        public class Query : IRequest<Album?>
        {
            public Guid? Id { get; set; }
            public string? Name { get; set; } = null;
            public List<String>? Artists { get; set; } = null;
            //Returns related Songs and Artists, not just Album information
            public bool ExtendedQuery { get; set; } = false;
            //All artists must be present, not just some of them
            public bool AllArtistsPresent { get; set; } = false;
        }

        public class Handler : IRequestHandler<Query, Album?>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Album?> Handle(Query query, CancellationToken cancellationToken)
            {
                if (query.Name == null && query.Artists == null && query.Id == null)
                {
                    return null;
                }

                IQueryable<Album> albumsQuery = _context.Albums;

                if (query.Id != null)
                {
                    albumsQuery = albumsQuery.Where(a => a.Id == query.Id);
                }

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

                Album? Album;
                if (query.ExtendedQuery)
                {
                    Album = await albumsQuery
                    .Include(a => a.Songs)
                    .Include(a => a.AlbumArtistRelations)
                    .ThenInclude(ar => ar.Artist)
                    .FirstOrDefaultAsync(cancellationToken);
                }
                else
                {
                    Album = await albumsQuery
                    .FirstOrDefaultAsync(cancellationToken);
                }

                return Album;
            }
        }

    }
}