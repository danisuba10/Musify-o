using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Requests;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
    public class SearchUser
    {
        public class Query : IRequest<List<User>>
        {
            public required SearchRequest Request { get; set; }
        }
        public class Handler : IRequestHandler<Query, List<User>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<User>> Handle(Query query, CancellationToken cancellationToken)
            {
                var searchTerm = query.Request.SearchTerm?.ToLower();
                var pQuery = _context.Users.AsQueryable();

                if (!String.IsNullOrWhiteSpace(searchTerm))
                {
                    pQuery = pQuery.Where(p => p.DisplayName.ToLower().Contains(searchTerm));
                }

                if (!String.IsNullOrWhiteSpace(query.Request.LastName) && query.Request.LastCreatedAt.HasValue)
                {
                    var lastName = query.Request.LastName.ToLower();
                    var lastCreatedAt = query.Request.LastCreatedAt.Value;

                    pQuery = pQuery.Where(pl =>
                        String.Compare(pl.DisplayName.ToLower(), lastName) > 0 ||
                        (String.Compare(pl.DisplayName.ToLower(), lastName) == 0 && pl.CreatedAt > lastCreatedAt)
                    );
                }

                pQuery = pQuery
                    .OrderBy(a => a.DisplayName)
                    .ThenBy(a => a.CreatedAt)
                    .Take(query.Request.PageSize);

                return await pQuery.ToListAsync(cancellationToken);
            }
        }
    }
}