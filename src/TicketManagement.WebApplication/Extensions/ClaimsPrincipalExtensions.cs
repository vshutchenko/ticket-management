using System.Security.Claims;

namespace TicketManagement.WebApplication.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsEventManager(this ClaimsPrincipal principal)
        {
            return principal.Identities.First().HasClaim(ClaimTypes.Role, "Event manager");
        }

        public static bool IsUser(this ClaimsPrincipal principal)
        {
            return principal.Identities.First().HasClaim(ClaimTypes.Role, "User");
        }

        public static DateTime GetLocalTime(this ClaimsPrincipal principal, DateTime date)
        {
            Claim? timeZoneClaim = principal.Identities.First().Claims.FirstOrDefault(c => c.Type == "timezoneId");

            if (timeZoneClaim is null)
            {
                return date;
            }

            TimeZoneInfo? tz = TimeZoneInfo.GetSystemTimeZones().First(z => z.Id == timeZoneClaim.Value);

            return TimeZoneInfo.ConvertTimeFromUtc(date, tz);
        }

        public static DateTime GetUtcTime(this ClaimsPrincipal principal, DateTime date)
        {
            Claim? timeZoneClaim = principal.Identities.First().Claims.FirstOrDefault(c => c.Type == "timezoneId");

            if (timeZoneClaim is null)
            {
                return date;
            }

            TimeZoneInfo? tz = TimeZoneInfo.GetSystemTimeZones().First(z => z.Id == timeZoneClaim.Value);

            return TimeZoneInfo.ConvertTimeToUtc(date, tz);
        }
    }
}
