using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Playlists
{
    public class PublicPlaylistCountOfUser
    {
        public class Query : IRequest<int>
        {
            public required Guid UserId { get; set; }
        }
        public class Handler : IRequestHandler<Query, int>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(Query query, CancellationToken cancellationToken)
            {
                return await _context.Playlists
                    .Where(pl => pl.UserId == query.UserId)
                    .Where(pl => pl.Visibility == Domain.Visibility.Public)
                    .CountAsync(cancellationToken);
            }
        }
    }
}