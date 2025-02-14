using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Requests;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Albums
{
    public class SearchAlbum
    {
        public class Query : IRequest<List<Album>>
        {
            public SearchRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<Album>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<Album>> Handle(Query request, CancellationToken cancellationToken)
            {
                var searchTerm = request.Request.SearchTerm?.ToLower();
                var query = _context.Albums.AsQueryable();

                if (!String.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(a => a.Name.ToLower().Contains(searchTerm));
                }

                if (!String.IsNullOrWhiteSpace(request.Request.LastName) && request.Request.LastCreatedAt.HasValue)
                {
                    var lastName = request.Request.LastName.ToLower();
                    var lastCreatedAt = request.Request.LastCreatedAt.Value;

                    query = query.Where(a =>
                        String.Compare(a.Name.ToLower(), lastName) > 0 ||
                        (String.Compare(a.Name.ToLower(), lastName) == 0 && a.CreatedAt > lastCreatedAt)
                    );
                }

                query = query
                    .OrderBy(a => a.Name)
                    .ThenBy(a => a.CreatedAt)
                    .Take(request.Request.PageSize);

                return await query.ToListAsync(cancellationToken);
            }
        }
    }
}
}