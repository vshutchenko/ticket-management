namespace TicketManagement.WebApplication.Middlewares
{
    public static class FeatureMiddlewareExtensions
    {
        public static IApplicationBuilder UseFeatureMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FeatureMiddleware>();
        }
    }

    public class FeatureMiddleware
    {
        private readonly RequestDelegate _next;

        public FeatureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IConfiguration configuration)
        {
            if (httpContext.Request.Path.Value!.Contains("/Event")
                || httpContext.Request.Path.Value.Contains("/Account")
                || httpContext.Request.Path.Value!.Equals("/"))
            {
                var redirectUrl = configuration["ReactUI:BaseAddress"];
                httpContext.Response.Redirect(redirectUrl);
            }

            // Move forward into the pipeline
            await _next(httpContext);
        }
    }
}
