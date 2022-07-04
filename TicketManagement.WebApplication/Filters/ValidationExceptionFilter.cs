using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TicketManagement.WebApplication.Filters
{
    public class ValidationExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState);

            viewData.Add("ValidationError", context.Exception.Message);

            context.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/ValidationError.cshtml",
                ViewData = viewData,
            };
        }
    }
}
