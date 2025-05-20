using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using adminProfolio.Models;
using adminProfolio.Settings;
using adminProfolio.Interfaces;
using Microsoft.Extensions.Options;

namespace adminProfolio.Services
{
    public class TokenService : ITokenService
    {
        private readonly Settings.JwtSettings _jwtSettings;

        public TokenService(IOptions<Settings.JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<(string AccessToken, string RefreshToken)> GetTokensAsync(string userId, string email, string role)
        {
            var accessToken = GenerateToken(userId, email, role, _jwtSettings.AccessTokenSecret, _jwtSettings.AccessTokenExpirationMinutes);
            var refreshToken = GenerateToken(userId, email, role, _jwtSettings.RefreshTokenSecret, _jwtSettings.RefreshTokenExpirationDays);

            return (accessToken, refreshToken);
        }

        private string GenerateToken(string userId, string email, string role, string secret, int expirationMinutes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
