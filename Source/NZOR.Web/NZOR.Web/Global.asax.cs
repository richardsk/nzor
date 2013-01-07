using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NZOR.Publish.Model.Administration;
using NZOR.Web.Service.Client;
using System.Configuration;

namespace NZOR.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorWithElmahAttribute());
        }

        public void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*robotstxt}", new { robotstxt = @"(.*/)?robots.txt(/.*)?" });

            routes.MapRoute(
                "AjaxSearch", // Route name
                "search/GetSearchAutoComplete", // URL with parameters
                new { controller = "search", action = "GetSearchAutoComplete" } // Parameter defaults
            );

            routes.MapRoute(
                "Search", // Route name
                "search", // URL with parameters
                new { controller = "search", action = "search", query = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "Name", // Route name
                "names/{id}", // URL with parameters
                new { controller = "names", action = "detail", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "User.completeRegistration", // Route name
                "users/completeRegistration", // URL with parameters
                new { controller = "users", action = "completeRegistration" } // Parameter defaults
            );

            routes.MapRoute(
                "Feedback.submitFeedback", // Route name
                "feedback/submitfeedback", // URL with parameters
                new { controller = "feedback", action = "submitfeedback" } // Parameter defaults
            );
            
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "home", action = "index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}