using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Application.DataTransferObjects.Responses;
using Application.Exceptions.User;
using Application.Services;
using MediatR;
using Persistence;

namespace Application.Users
{
    public class EnableTwoFactorAuth
    {
        public class Command : IRequest<TwoFactorAuthResponse>
        {
            public Guid UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, TwoFactorAuthResponse>
        {
            private readonly ApplicationDbContext _context;
            private readonly TwoFactorAuthService _twoFactorAuthService;

            public Handler(ApplicationDbContext context, TwoFactorAuthService twoFactorAuthService)
            {
                _context = context;
                _twoFactorAuthService = twoFactorAuthService;
            }

            public async Task<TwoFactorAuthResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    throw new UserDoesNotExistException();
                }

                var secret = await _twoFactorAuthService.GenerateTwoFactorSecretAsync();
                var recoveryCodes = await _twoFactorAuthService.GenerateRecoveryCodesAsync();
                var qrCodeUri = await _twoFactorAuthService.GenerateQrCodeUri(user.Email, secret, "Meloptica");

                user.IsTwoFactorEnabled = false;
                user.TwoFactorSecret = secret;
                user.TwoFactorRecoveryCodes = JsonSerializer.Serialize(recoveryCodes);

                await _context.SaveChangesAsync();

                return new TwoFactorAuthResponse
                {
                    IsTwoFactorEnabled = false,
                    SecretKey = secret,
                    QrCodeUri = qrCodeUri,
                    RecoveryCodes = recoveryCodes
                };
            }
        }
    }
}