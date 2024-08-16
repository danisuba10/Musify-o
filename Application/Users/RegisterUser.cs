using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;
using MediatR;
using Persistence;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata;

namespace Application.Users
{
    public class RegisterUser
    {
        public class Command : IRequest
        {
            public required string UserName { get; set; }
            public required string Password { get; set; }
        }
        public class Handler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _context;
            private readonly IPasswordHasher<User> _passwordHasher;
            public Handler(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
            {
                _context = context;
                _passwordHasher = passwordHasher;
            }
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                //TODO: Check if UserName already exists!

                var User = new User
                { UserName = command.UserName };
                User.PasswordHash = _passwordHasher.HashPassword(User, command.Password);

                await _context.Users.AddAsync(User, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}