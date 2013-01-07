using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NZOR.Web.Service
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "Matches.SubmitBatchMatch",
            url: "matches/submitbatchmatch",
            defaults: new { controller = "matches", action = "submitbatchmatch" }
        );
            routes.MapRoute(
                name: "Matches.SubmitBatchMatchWithEmail",
                url: "matches/submitbatchmatchwithemail",
                defaults: new { controller = "matches", action = "submitbatchmatchwithemail" }
            );
            routes.MapRoute(
                name: "Matches.SubmitAutoBatchMatch",
                url: "matches/submitautobatchmatch",
                defaults: new { controller = "matches", action = "submitautobatchmatch" }
            );
            routes.MapRoute(
                name: "Matches.PollBatchMatch",
                url: "matches/pollbatchmatch",
                defaults: new { controller = "matches", action = "pollbatchmatch" }
            );
            routes.MapRoute(
                name: "Matches.SubmitBrokeredNames",
                url: "matches/submitbrokerednames",
                defaults: new { controller = "matches", action = "submitbrokerednames" }
            );
            routes.MapRoute(
                name: "Matches.GetBatchMatch",
                url: "matches/{id}",
                defaults: new { controller = "matches", action = "getbatchmatch" }
            );
            routes.MapRoute(
                name: "Matches.GetExternalMatchResults",
                url: "matches/externalresults/{id}",
                defaults: new { controller = "matches", action = "getbatchexternalresults" }
            );

            routes.MapRoute("Download", "downloads/{fileName}", new { controller = "file", action = "index" });
        }
    }
}