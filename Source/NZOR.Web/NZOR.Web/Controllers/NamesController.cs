using System.Configuration;
using System.Web.Mvc;
using NZOR.Web.Service.Client;
using NZOR.Web.ViewModels;
using NZOR.Publish.Model.Names;

namespace NZOR.Web.Controllers
{
    public class NamesController : Controller
    {
        private static ServiceClient _service;

        static NamesController()
        {
            _service = new ServiceClient("", ConfigurationManager.AppSettings["ServiceUrl"]);
        }

        public ActionResult Detail(string id)
        {
            Name name = _service.GetName(id);

            NameViewModel viewModel = new NameViewModel(name);

            return View(viewModel);
        }

    }
}
