using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NZOR.Web.Service.Client;
using System.Configuration;

namespace NZOR.Web.Controllers
{
    public class LaunchController : Controller
    {
        private static ServiceClient _service;

        public LaunchController()
        {
            _service = new ServiceClient("", ConfigurationManager.AppSettings["ServiceUrl"]);
        }

        public ActionResult Launch()
        {
            _service.SetSetting("Launched", "true");
            return RedirectToAction("index", "home");
        }

        public ActionResult UnLaunch()
        {
            _service.SetSetting("Launched", "false");
            return RedirectToAction("index", "home");
        }

    }
}
