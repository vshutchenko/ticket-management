using System.Globalization;

namespace TicketManagement.WebApplication.Middlewares
{
    public static class RequestLocalizationCookiesMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLocalizationCookies(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestLocalizationCookiesMiddleware>();
            return app;
        }
    }

    public class RequestLocalizationCookiesMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var cookieOptions = new CookieOptions
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddYears(1),
            };

            string culture = context.User.FindFirst("culture") is null ? "en-US" : context.User.FindFirst("culture")!.Value;

            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);

            context.Response.Cookies.Append("culture", culture, cookieOptions);

            string timeZone = context.User.FindFirst("timezoneId") is null ? TimeZoneInfo.Local.Id : context.User.FindFirst("timezoneId")!.Value;

            context.Response.Cookies.Append("timezone", timeZone, cookieOptions);

            await next(context);
        }
    }
}
