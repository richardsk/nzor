using NZOR.Web.Service.Filters;
using System.Web;
using System.Web.Mvc;

namespace NZOR.Web.Service
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            System.Web.Http.GlobalConfiguration.Configuration.Filters.Add(new ExceptionHandlerFilter());
        }
    }
}