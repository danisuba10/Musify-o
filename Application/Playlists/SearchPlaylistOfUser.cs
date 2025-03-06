using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Playlists
{
    public class SearchPlaylistOfUser
    {
        public class Query : IRequest<List<Playlist>>
        {
            public required string Term { get; set; }
            public required Guid UserId { get; set; }
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
                var pQuery = _context.Playlists.AsQueryable();

                if (!String.IsNullOrWhiteSpace(query.Term))
                {
                    pQuery = pQuery.Where(p => p.Name.ToLower().Contains(query.Term));
                    pQuery = pQuery.Where(p => p.UserId == query.UserId);
                }

                pQuery = pQuery
                    .OrderBy(a => a.Name)
                    .ThenBy(a => a.CreatedAt)
                    .Take(10);

                return await pQuery.ToListAsync(cancellationToken);
            }
        }
    }
}