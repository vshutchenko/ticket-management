using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TicketManagement.UserApi.Services.Interfaces;

namespace TicketManagement.UserApi.JwtAuthentication
{
    internal class JwtAuthenticationHandler : AuthenticationHandler<JwtAuthenticationOptions>
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<JwtAuthenticationHandler> _logger;

        public JwtAuthenticationHandler(
            IOptionsMonitor<JwtAuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            ITokenService tokenService,
            ILogger<JwtAuthenticationHandler> logger)
            : base(options, loggerFactory, encoder, clock)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return await Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
            }

            var token = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token) || "Bearer ".Length > token.Length)
            {
                return await Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
            }

            var isValid = _tokenService.ValidateToken(token);

            if (!isValid)
            {
                return await Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}