using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http;

namespace NZOR.Web.Service.Filters
{
    class ExceptionHandlerFilter : ExceptionFilterAttribute, IExceptionFilter
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null && !(actionExecutedContext.Exception is HttpResponseException))
            {
                actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                actionExecutedContext.Response.Content = new System.Net.Http.StringContent("An error has occurred while processing the request. This error message has been logged.");

                NLog.LogManager.GetLogger("NZOR-Web-Service-Public").ErrorException(actionExecutedContext.Exception.Message, actionExecutedContext.Exception);
            }
            else
            {
                base.OnException(actionExecutedContext);
            }
        }
    }
}