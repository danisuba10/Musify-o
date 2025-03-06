using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Constants;
using Application.DataTransferObjects.Requests;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Playlists
{
    public class SearchPlaylist
    {
        public class Query : IRequest<List<Playlist>>
        {
            public required SearchRequest Request { get; set; }
            public Guid? UserId { get; set; }
            public string? Role { get; set; }
        }
        public class Handler : IRequestHandler<Query, List<Playlist>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<Playlist>> Handle(Query query, CancellationToken cancellationToken)
            {
                var searchTerm = query.Request.SearchTerm?.ToLower();
                var pQuery = _context.Playlists.AsQueryable();

                if (!String.IsNullOrWhiteSpace(searchTerm))
                {
                    pQuery = pQuery.Where(p => p.Name.ToLower().Contains(searchTerm));
                }

                if (query.UserId == null)
                {
                    pQuery = pQuery.Where(p => p.Visibility == Visibility.Public);
                }
                else if (!(query.Role?.Equals("Admin") ?? false))
                {
                    pQuery = pQuery.Where(p => p.Visibility == Visibility.Public || p.UserId == query.UserId);
                }

                if (!String.IsNullOrWhiteSpace(query.Request.LastName) && query.Request.LastCreatedAt.HasValue)
                {
                    var lastName = query.Request.LastName.ToLower();
                    var lastCreatedAt = query.Request.LastCreatedAt.Value;

                    pQuery = pQuery.Where(pl =>
                        String.Compare(pl.Name.ToLower(), lastName) > 0 ||
                        (String.Compare(pl.Name.ToLower(), lastName) == 0 && pl.CreatedAt > lastCreatedAt)
                    );
                }

                pQuery = pQuery
                    .OrderBy(a => a.Name)
                    .ThenBy(a => a.CreatedAt)
                    .Take(query.Request.PageSize);

                return await pQuery.ToListAsync(cancellationToken);
            }
        }
    }
}