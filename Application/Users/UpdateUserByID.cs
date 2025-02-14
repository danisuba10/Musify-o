using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Constants;
using Application.DataTransferObjects.Requests;
using MediatR;
using Persistence;

namespace Application.Users
{
    public class UpdateUserByID
    {
        public class Query : IRequest
        {
            public required UpdateUserRequest Request { get; set; }
        }
        public class Handler : IRequestHandler<Query>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            public Handler(ApplicationDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }
            public async Task<Unit> Handle(Query query, CancellationToken cancellationToken)
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.Id == query.Request.Id);

                if (user == null)
                {
                    throw new Exception("User with this ID does not exist!");
                }

                if (!String.IsNullOrWhiteSpace(query.Request.DisplayName))
                {
                    user.DisplayName = query.Request.DisplayName;
                }

                if (!String.IsNullOrWhiteSpace(query.Request.Role))
                {
                    if (!RoleConstants.AvailableRoles.Contains(query.Request.Role))
                    {
                        throw new Exception("Update user exception: Invalid role!");
                    }
                    user.Role = query.Request.Role;
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}