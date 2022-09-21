using System.Security.Claims;
using IdentityModel;
using TicketManagement.Core.Models;

namespace TicketManagement.WebApplication.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsEventManager(this ClaimsPrincipal principal)
        {
            return principal.Identities.First().HasClaim(JwtClaimTypes.Role, Roles.EventManager);
        }

        public static bool IsVenueManager(this ClaimsPrincipal principal)
        {
            return principal.Identities.First().HasClaim(JwtClaimTypes.Role, Roles.VenueManager);
        }

        public static bool IsUser(this ClaimsPrincipal principal)
        {
            return principal.Identities.First().HasClaim(JwtClaimTypes.Role, Roles.User);
        }

        public static DateTime GetLocalTime(this ClaimsPrincipal principal, DateTime date)
        {
            var timeZoneClaim = principal.Identities.First().Claims.FirstOrDefault(c => c.Type == "timezoneId");

            if (timeZoneClaim is null)
            {
                return date;
            }

            var tz = TimeZoneInfo.GetSystemTimeZones().First(z => z.Id == timeZoneClaim.Value);

            return TimeZoneInfo.ConvertTimeFromUtc(date, tz);
        }

        public static DateTime GetUtcTime(this ClaimsPrincipal principal, DateTime date)
        {
            var timeZoneClaim = principal.Identities.First().Claims.FirstOrDefault(c => c.Type == "timezoneId");

            if (timeZoneClaim is null)
            {
                return date;
            }

            var tz = TimeZoneInfo.GetSystemTimeZones().First(z => z.Id == timeZoneClaim.Value);

            return TimeZoneInfo.ConvertTimeToUtc(date, tz);
        }
    }
}
