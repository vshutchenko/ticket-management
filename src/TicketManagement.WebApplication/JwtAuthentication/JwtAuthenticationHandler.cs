using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TicketManagement.WebApplication.Clients.UserApi;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.JwtAuthentication
{
    internal class JwtAuthenticationHandler : AuthenticationHandler<JwtAuthenticationOptions>
    {
        private readonly IUserClient _userClient;
        private readonly ITokenService _tokenService;

        public JwtAuthenticationHandler(
            IOptionsMonitor<JwtAuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserClient userClient,
            ITokenService tokenService)
            : base(options, loggerFactory, encoder, clock)
        {
            _userClient = userClient;
            _tokenService = tokenService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            var tokenWithScheme = Request.Headers["Authorization"].ToString();

            if (!tokenWithScheme.StartsWith("Bearer "))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            var token = tokenWithScheme["Bearer ".Length..];

            try
            {
                await _userClient.ValidateTokenAsync(token);
            }
            catch (HttpRequestException)
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            _tokenService.SaveToken(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}