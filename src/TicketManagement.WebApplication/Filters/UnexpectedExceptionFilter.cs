using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TicketManagement.WebApplication.Filters
{
    public class UnexpectedExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;

            context.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml",
            };
        }
    }
}
