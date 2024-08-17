using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
    public class GetUserByUserName
    {
        public class Query : IRequest<User>
        {
            public required string UserName { get; set; }
        }

        public class Handler : IRequestHandler<Query, User>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<User> Handle(Query query, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == query.UserName, cancellationToken);

                if (user == null)
                {
                    throw new Exception("Username does not exist!");
                }

                return user;
            }
        }
    }
}