using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NZOR.Web.Service.Client;
using System.Configuration;
using NZOR.Publish.Model.Names;
using NZOR.Web.ViewModels;

namespace NZOR.Web.Controllers
{
    public class FeedbackController : Controller
    {
        private static ServiceClient _service;

        static FeedbackController()
        {
            _service = new ServiceClient("", ConfigurationManager.AppSettings["ServiceUrl"]);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SubmitFeedback(string message, string sender, string email, Name name)
        {
            List<string> provs = new List<string>();
            foreach (string key in Request.Form.Keys)
            {
                if (key.StartsWith("provider_") && Request.Form[key] == "on") provs.Add(key.Substring(9));
            }

            if (name == null || name.NameId == Guid.Empty || provs.Count == 0) return null; //dodgy call, probably spam

            NZOR.Publish.Model.Common.Feedback fb = new Publish.Model.Common.Feedback();
            fb.Sender = sender;
            fb.SenderEmail = email;
            fb.Message = message;
            fb.NameId = name.NameId;
            fb.ProvidersToEmail.AddRange(provs);

            _service.SubmitFeedback(fb);

            return RedirectToRoute("Name", new { id = name.NameId });

        }
    }
}
