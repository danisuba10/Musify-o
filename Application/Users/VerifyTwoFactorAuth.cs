using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Responses;
using Application.Services;
using MediatR;
using Persistence;

namespace Application.Users
{
    public class VerifyTwoFactorAuth
    {
        public class Command : IRequest<VerifyTwoFactorAuthResponse>
        {
            public Guid UserId { get; set; }
            public string Code { get; set; }
        }

        public class Handler : IRequestHandler<Command, VerifyTwoFactorAuthResponse>
        {
            private readonly ApplicationDbContext _context;
            private readonly TwoFactorAuthService _twoFactorAuthService;
            private readonly JwtTokenService _jwtTokenService;

            public Handler(ApplicationDbContext context, TwoFactorAuthService twoFactorAuthService, JwtTokenService jwtTokenService)
            {
                _context = context;
                _twoFactorAuthService = twoFactorAuthService;
                _jwtTokenService = jwtTokenService;
            }

            public async Task<VerifyTwoFactorAuthResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null || string.IsNullOrEmpty(user.TwoFactorSecret))
                {
                    return new VerifyTwoFactorAuthResponse { IsValid = false };
                }

                var isValid = await _twoFactorAuthService.VerifyTwoFactorCodeAsync(user.TwoFactorSecret, request.Code);
                if (!isValid)
                {
                    return new VerifyTwoFactorAuthResponse { IsValid = false, Token = null };
                }

                return new VerifyTwoFactorAuthResponse
                {
                    IsValid = true,
                    Token = _jwtTokenService.GenerateFullAuthToken(user)
                };
            }
        }
    }
}
