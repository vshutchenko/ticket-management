using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TicketManagement.VenueApi.Services.Validation;

namespace TicketManagement.VenueApi.Filters
{
    public class ValidationExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception.GetType() != typeof(ValidationException))
            {
                return;
            }

            context.ExceptionHandled = true;

            context.Result = new BadRequestObjectResult(new { error = context.Exception.Message });
        }
    }
}
