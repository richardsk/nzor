using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NZOR.Web.ViewModels;
using System.Reflection;
using System.Configuration;
using NZOR.Web.Service.Client;
using NZOR.Web.Service.Client.Responses;
using NZOR.Publish.Model.Administration;

namespace NZOR.Web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private static ServiceClient _service;

        public HomeController()
        {
            _service = new ServiceClient("", ConfigurationManager.AppSettings["ServiceUrl"]);
        }

        public ActionResult Index()
        {
            StatisticsResponse sr = _service.GetStatistics(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), null, 300);
            if (sr == null) sr = _service.GetStatistics(null, null, 300);
            if (sr != null)
            {
                ViewBag.Statistics = sr.Statistics;
                if (sr.Statistics.Count > 0) ViewBag.LastStatsDate = sr.Statistics[0].Date.ToString("dd MMM yyyy");
            }

            try
            {
                var asm = Assembly.GetExecutingAssembly();

                object[] attributes;
                attributes = asm.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Any()) { ViewBag.Description = ((AssemblyDescriptionAttribute)attributes[0]).Description; }

                attributes = asm.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                if (attributes.Any()) { ViewBag.InformationalVersion = ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion; }

                ViewBag.Version = asm.GetName().Version.ToString();
                ViewBag.WebServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"];
            }
            catch (Exception)
            {
                // Do nothing.
            }

            return View();
        }

        public ActionResult Disclaimer()
        {
            return View();
        }

        public ActionResult TermsOfUse()
        {
            return View();
        }

        public ActionResult WhatIsNZOR()
        {
            return View();
        }
    }
}
