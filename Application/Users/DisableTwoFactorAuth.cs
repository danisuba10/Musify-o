using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Persistence;

namespace Application.Users
{
    public class DisableTwoFactorAuth
    {
        public class Command : IRequest<bool>
        {
            public Guid UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    return false;
                }

                user.IsTwoFactorEnabled = false;
                user.TwoFactorSecret = null;
                user.TwoFactorRecoveryCodes = null;
                await _context.SaveChangesAsync();

                return true;
            }
        }
    }
}