using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Requests;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Songs
{
    public class SearchSong
    {
        public class Query : IRequest<List<Song>>
        {
            public SearchRequest Request { get; set; }
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
                var searchTerm = request.Request.SearchTerm?.ToLower();
                var query = _context.Songs.AsQueryable();

                if (!String.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(a => a.Title.ToLower().Contains(searchTerm));
                }

                if (!String.IsNullOrWhiteSpace(request.Request.LastName) && request.Request.LastCreatedAt.HasValue)
                {
                    var lastName = request.Request.LastName.ToLower();
                    var lastCreatedAt = request.Request.LastCreatedAt.Value;

                    query = query.Where(a =>
                        String.Compare(a.Title.ToLower(), lastName) > 0 ||
                        (String.Compare(a.Title.ToLower(), lastName) == 0 && a.CreatedAt > lastCreatedAt)
                    );
                }

                query = query
                    .OrderBy(a => a.Title)
                    .ThenBy(a => a.CreatedAt)
                    .Include(a => a.Album)
                    .Take(request.Request.PageSize);

                return await query.ToListAsync(cancellationToken);
            }
        }
    }
}