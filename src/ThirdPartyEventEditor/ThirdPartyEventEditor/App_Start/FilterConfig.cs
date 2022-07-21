using System.Web;
using System.Web.Mvc;
using ThirdPartyEventEditor.Filters;

namespace ThirdPartyEventEditor
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ExceptionFilter());
        }
    }
}
