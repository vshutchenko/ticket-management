using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Localization;

namespace TicketManagement.WebApplication.Services
{
    internal class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _accessor;

        public TokenService(IHttpContextAccessor accessor)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        public void DeleteToken()
        {
            _accessor.HttpContext!.Session.Remove("JwtToken");
            SetCookies("en-US", TimeZoneInfo.Local.Id);
        }

        public string GetToken()
        {
            return _accessor.HttpContext!.Session.GetString("JwtToken") ?? string.Empty;
        }

        public void SaveToken(string tokenString)
        {
            _accessor.HttpContext!.Session.SetString("JwtToken", $"Bearer {tokenString}");

            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);

            var timezone = token.Claims.First(c => c.Type == "timezoneId").Value;

            var culture = token.Claims.First(c => c.Type == "culture").Value;

            SetCookies(culture, timezone);
        }

        private void SetCookies(string culture, string timeZone)
        {
            var cookieOptions = new CookieOptions
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddYears(1),
            };

            _accessor.HttpContext!.Response.Cookies.Append("timezoneId", timeZone, cookieOptions);

            _accessor.HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }
    }
}
