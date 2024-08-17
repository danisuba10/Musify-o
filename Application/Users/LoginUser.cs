using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Persistence;
using Microsoft.AspNetCore.Identity;
using Domain;

namespace Application.Users
{
    public class LoginUser
    {
        public class Command : IRequest<bool>
        {
            public required string UserName { get; set; }
            public required string Password { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            private readonly IPasswordHasher<User> _passwordHasher;
            public Handler(ApplicationDbContext context, IMediator mediator, IPasswordHasher<User> passwordHasher)
            {
                _context = context;
                _mediator = mediator;
                _passwordHasher = passwordHasher;
            }
            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                var user = await _mediator.Send(new GetUserByUserName.Query { UserName = command.UserName });

                if (user == null)
                {
                    throw new Exception("User does not exist!");
                }

                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, command.Password);

                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    return false;
                }

                return true;

            }
        }
    }
}