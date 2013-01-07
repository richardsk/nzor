using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NZOR.Web.Service
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Names
            config.Routes.MapHttpRoute(
                    name: "Names",
                    routeTemplate: "names",
                    defaults: new { controller = "names", action = "getnames" }
                );
            config.Routes.MapHttpRoute(
                name: "Names.Lookups",
                routeTemplate: "names/lookups",
                defaults: new { controller = "names", action = "getlookups" }
            );
            config.Routes.MapHttpRoute(
                name: "Names.Search",
                routeTemplate: "names/search",
                defaults: new { controller = "names", action = "getsearch" }
            );
            config.Routes.MapHttpRoute(
                name: "Names.ByProviderId",
                routeTemplate: "names/byproviderid/{id}",
                defaults: new { controller = "names", action = "getbyproviderid" }
            );

            // Feedback
            config.Routes.MapHttpRoute(
                name: "Feedback.Submit",
                routeTemplate: "feedback/submitfeedback",
                defaults: new { controller = "feedback", action = "getsubmitfeedback" }
            );

            // Users
            config.Routes.MapHttpRoute(
                name: "Users.Register",
                routeTemplate: "users/registeruser",
                defaults: new { controller = "users", action = "registeruser" }
            );
            config.Routes.MapHttpRoute(
                name: "Users.Complete",
                routeTemplate: "users/complete",
                defaults: new { controller = "users", action = "complete" }
            );
            config.Routes.MapHttpRoute(
                name: "Users.ResendApiKey",
                routeTemplate: "users/resendapikey",
                defaults: new { controller = "users", action = "resendapikey" }
            );

            // Vocabularies
            config.Routes.MapHttpRoute(
                name: "Vocabularies",
                routeTemplate: "vocabularies",
                defaults: new { controller = "vocabularies", action = "getvocabularies" }
            );

            config.Routes.MapHttpRoute(
                name: "Vocabularies.ConceptRelationships",
                routeTemplate: "vocabularies/conceptrelationships",
                defaults: new { controller = "vocabularies", action = "getconceptrelationships" }
            );

            config.Routes.MapHttpRoute(
                name: "Vocabularies.TaxonProperties",
                routeTemplate: "vocabularies/taxonproperties",
                defaults: new { controller = "vocabularies", action = "gettaxonproperties" }
            );

            config.Routes.MapHttpRoute(
                name: "Vocabularies.TaxonProperty",
                routeTemplate: "vocabularies/taxonproperties/{name}",
                defaults: new { controller = "vocabularies", action = "gettaxonproperty" }
            );

            config.Routes.MapHttpRoute(
                name: "Vocabularies.TaxonRanks",
                routeTemplate: "vocabularies/taxonranks",
                defaults: new { controller = "vocabularies", action = "gettaxonranks" }
            );

            config.Routes.MapHttpRoute(
                name: "Vocabularies.GeographicSchemas",
                routeTemplate: "vocabularies/geographicschemas",
                defaults: new { controller = "geographicschemas" }
            );

            config.Routes.MapHttpRoute(
                name: "Vocabularies.GeographicSchema",
                routeTemplate: "vocabularies/geographicschemas/{id}",
                defaults: new { controller = "geographicschemas" }
            );

            config.Routes.MapHttpRoute(
                name: "Vocabularies.Vocabulary",
                routeTemplate: "vocabularies/{use}",
                defaults: new { controller = "vocabularies" }
            );

            config.Routes.MapHttpRoute("Admin.GetSetting", "admin/getsetting", new { controller = "admin", action = "getsetting" });
            config.Routes.MapHttpRoute("Admin.SetSetting", "admin/setsetting", new { controller = "admin", action = "setsetting" });
            config.Routes.MapHttpRoute("Admin.GetNameRequests", "admin/getnamerequests", new { controller = "admin", action = "getnamerequests" });

            config.Routes.MapHttpRoute(
                  name: "DefaultApi",
                  routeTemplate: "{controller}/{id}",
                  defaults: new { id = RouteParameter.Optional }
              );
        }
    }
}
