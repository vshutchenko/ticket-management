using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TicketManagement.IntegrationTests.Addition
{
    internal class TokenHelper
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenHelper()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _key = config["Jwt:Key"];
            _issuer = config["Jwt:Issuer"];
            _audience = config["Jwt:Audience"];
        }

        public string GetToken(IList<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                expires: DateTime.Now.AddMinutes(30),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
