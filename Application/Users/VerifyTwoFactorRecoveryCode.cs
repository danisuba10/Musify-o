using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Persistence;

namespace Application.Users
{
    public class VerifyTwoFactorRecoveryCode
    {
        public class Command : IRequest<bool>
        {
            public Guid UserId { get; set; }
            public required string RecoveryCode { get; set; }
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
                if (user == null || string.IsNullOrEmpty(user.TwoFactorRecoveryCodes))
                {
                    return false;
                }

                var recoveryCodes = JsonSerializer.Deserialize<string[]>(user.TwoFactorRecoveryCodes);
                if (recoveryCodes == null || !recoveryCodes.Any())
                {
                    return false;
                }

                var isValid = recoveryCodes.Contains(request.RecoveryCode);

                if (isValid)
                {
                    // Remove the used recovery code
                    var updatedCodes = recoveryCodes.Where(c => c != request.RecoveryCode).ToArray();
                    user.TwoFactorRecoveryCodes = JsonSerializer.Serialize(updatedCodes);
                    await _context.SaveChangesAsync();
                }

                return isValid;
            }
        }
    }
}