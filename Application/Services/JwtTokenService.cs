using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateTwoFactorPendingToken(User user)
        {
            var claims = GenerateBaseClaims(user);
            claims.Add(new Claim("TwoFactorEnabled", "true"));
            claims.Add(new Claim("TwoFactorPending", "true"));

            return GenerateToken(claims, DateTime.Now.AddMinutes(5));
        }

        public string GenerateFullAuthToken(User user)
        {
            var claims = GenerateBaseClaims(user);
            claims.Add(new Claim("TwoFactorPending", "false"));
            claims.Add(new Claim("TwoFactorEnabled", user.IsTwoFactorEnabled.ToString().ToLower()));

            return GenerateToken(claims, DateTime.Now.AddMinutes(60));
        }

        public string GenerateJwtToken(User user)
        {
            var claims = GenerateBaseClaims(user);
            return GenerateToken(claims, DateTime.Now.AddMinutes(60));
        }

        private List<Claim> GenerateBaseClaims(User user)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Identifier", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Role", user.Role)
            };
        }

        private string GenerateToken(IEnumerable<Claim> claims, DateTime? expiration = null)
        {
            var jwtSecret = _configuration["Jwt_Secret"];
            var jwtIssuer = _configuration["Jwt_Issuer"];
            var jwtAudience = _configuration["Jwt_Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiration ?? DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}