using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Domain;
using Persistence;

namespace Application.Users
{
    public class GetAllUsers
    {
        public class Query : IRequest<List<User>> { }
        public class Handler : IRequestHandler<Query, List<User>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<User>> Handle(Query query, CancellationToken cancellationToken)
            {
                List<User> Users = await _context.Users.ToListAsync(cancellationToken);
                return Users;
            }
        }
    }
}