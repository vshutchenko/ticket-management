using System;
using System.Web.Mvc;
using NLog;

namespace ThirdPartyEventEditor.Filters
{
    public class ExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var logger = LogManager.GetCurrentClassLogger();

            logger.Error(context.Exception);

            context.ExceptionHandled = true;

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider());

            viewData.Add("ErrorMessage", context.Exception.Message);

            context.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml",
                ViewData = viewData,
            };
        }
    }
}
