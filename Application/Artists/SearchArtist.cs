using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Requests;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Artists
{
    public class SearchArtist
    {
        public class Query : IRequest<List<Artist>>
        {
            public SearchRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<Artist>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<Artist>> Handle(Query request, CancellationToken cancellationToken)
            {
                var searchTerm = request.Request.SearchTerm?.ToLower();
                var query = _context.Artists.AsQueryable();

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