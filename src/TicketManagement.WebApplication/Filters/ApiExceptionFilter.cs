using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RestEase;
using TicketManagement.Core.Clients.CommonModels;

namespace TicketManagement.WebApplication.Filters
{
    public class ApiExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is not ApiException)
            {
                return;
            }

            context.ExceptionHandled = true;

            var apiException = context.Exception as ApiException;

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState);

            viewData.Add("ValidationError", apiException?.DeserializeContent<ErrorModel>().Error!);

            context.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/ValidationError.cshtml",
                ViewData = viewData,
            };
        }
    }
}
