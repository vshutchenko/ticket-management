using System;
using System.Diagnostics;
using System.Web.Mvc;
using NLog;

namespace ThirdPartyEventEditor.Filters
{
    public class ExecutionTimeActionFilter : Attribute, IActionFilter
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private Stopwatch _stopwatch = new Stopwatch();

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _stopwatch.Stop();

            var elapsedTime = _stopwatch.ElapsedMilliseconds;

            _logger.Info("{0} - execution ednded. Elapsed milliseconds: {1} ms.",
                filterContext.ActionDescriptor.ActionName, elapsedTime);
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _logger.Info("{0} - execution started. HTTP method: {1}",
                filterContext.ActionDescriptor.ActionName, filterContext.HttpContext.Request.HttpMethod);

            _stopwatch.Restart();
        }
    }
}
