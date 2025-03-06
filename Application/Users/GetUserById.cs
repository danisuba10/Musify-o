using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
    public class GetUserById
    {
        public class Query : IRequest<User>
        {
            public required Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, User?>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<User?> Handle(Query query, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == query.Id, cancellationToken);

                return user;
            }
        }
    }
}