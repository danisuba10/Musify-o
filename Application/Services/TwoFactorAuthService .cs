using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using OtpNet;
using QRCoder;

namespace Application.Services
{
    public class TwoFactorAuthService
    {
        private const int RecoveryCodeCount = 10;
        private const int RecoveryCodeLength = 8;

        public Task<string> GenerateTwoFactorSecretAsync()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            return Task.FromResult(Base32Encoding.ToString(key));
        }

        public Task<bool> VerifyTwoFactorCodeAsync(string secret, string code)
        {
            var secretBytes = Base32Encoding.ToBytes(secret);
            var totp = new Totp(secretBytes);
            return Task.FromResult(totp.VerifyTotp(code, out _, new VerificationWindow(2, 2)));
        }

        public Task<string> GenerateQrCodeUri(string email, string secret, string issuer)
        {
            var encodedIssuer = Uri.EscapeDataString(issuer);
            var encodedEmail = Uri.EscapeDataString(email);
            var uri = $"otpauth://totp/{encodedIssuer}:{encodedEmail}?secret={secret}&issuer={encodedIssuer}";
            return Task.FromResult(uri);
        }

        public Task<IEnumerable<string>> GenerateRecoveryCodesAsync()
        {
            var codes = new List<string>();
            var random = new Random();

            for (int i = 0; i < RecoveryCodeCount; i++)
            {
                var code = string.Concat(Enumerable.Range(0, RecoveryCodeLength)
                    .Select(_ => random.Next(0, 10)));
                codes.Add(code);
            }

            return Task.FromResult<IEnumerable<string>>(codes);
        }
    }
}