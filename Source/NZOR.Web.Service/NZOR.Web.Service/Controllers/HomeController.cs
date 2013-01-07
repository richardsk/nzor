using NZOR.Web.Service.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace NZOR.Web.Service.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var viewModel = new HomeViewModel();

            try
            {
                var asm = Assembly.GetExecutingAssembly();
                object[] attributes;

                attributes = asm.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                if (attributes.Length > 0)
                {
                    viewModel.Version = ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion;
                }

                attributes = asm.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    viewModel.Description = ((AssemblyDescriptionAttribute)attributes[0]).Description;
                }
            }
            catch (Exception)
            {
                // Do nothing.
            }

            return View(viewModel);
        }
    }
}
