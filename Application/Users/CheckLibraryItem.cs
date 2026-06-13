using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
    public class CheckLibraryItem
    {
        public class Query : IRequest<bool>
        {
            public required Guid UserId { get; set; }
            public required Guid ItemId { get; set; }
            public required string ItemType { get; set; }
        }

        public class Handler : IRequestHandler<Query, bool>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Query query, CancellationToken cancellationToken)
            {
                return await _context.UserLibraryItems
                    .AnyAsync(uli =>
                        uli.UserId == query.UserId &&
                        uli.ItemId == query.ItemId &&
                        uli.ItemType == query.ItemType,
                        cancellationToken);
            }
        }
    }
}
