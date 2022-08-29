using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TicketManagement.UserApi.Models;
using TicketManagement.UserApi.Services.Interfaces;

namespace TicketManagement.UserApi.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly string _key;
        private readonly string _audience;
        private readonly string _issuer;

        public TokenService(string key, string audience, string issuer)
        {
            _key = key;
            _audience = audience;
            _issuer = issuer;
        }

        public string GetToken(UserModel user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("id", user.Id),
                new Claim("timezoneId", user.TimeZoneId!),
                new Claim("culture", user.CultureName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            claims.AddRange(roleClaims);

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                expires: DateTime.Now.AddMinutes(30),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                    ValidateLifetime = false,
                },
                out var _);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
