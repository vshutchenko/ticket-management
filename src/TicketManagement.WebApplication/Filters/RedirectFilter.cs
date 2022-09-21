using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.FeatureManagement;

namespace TicketManagement.WebApplication.Filters
{
    public class RedirectFilter : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var featureManagement = context.HttpContext.RequestServices.GetRequiredService<IFeatureManager>();

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            if (await featureManagement.IsEnabledAsync(FeatureFlags.ReactUI))
            {
                var redirectUrl = configuration["ReactUI:BaseAddress"];

                if (context.HttpContext.Request.Path != "/")
                {
                    redirectUrl += context.HttpContext.Request.Path;
                    if (context.HttpContext.Request.QueryString.ToString() != string.Empty)
                    {
                        redirectUrl += context.HttpContext.Request.QueryString;
                    }
                }

                context.Result = new RedirectResult(redirectUrl, true);
            }
            else
            {
                await next();
            }
        }
    }
}
