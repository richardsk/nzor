using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

using NZOR.Web.Service.Client;
using NZOR.Publish.Model.Users;

namespace NZOR.Web.Controllers
{
    public class UsersController : Controller
    {
        private static ServiceClient _service;

        static UsersController()
        {
            _service = new ServiceClient("", ConfigurationManager.AppSettings["ServiceUrl"]);
        }

        public ActionResult Index()
        {
            return View();
        }

        //[RequireHttps()]
        public ActionResult RegisterUser(string email, string name, string password, string organisation)
        {
            ViewModels.Users.UserRegistration ur = new ViewModels.Users.UserRegistration();
            ur.Email = email;
            ur.Name = name;
            ur.Password = password;
            ur.Organisation = organisation;
            
            ur.RegistrationResponse = _service.RegisterUser(email, name, password, organisation);

            return View("Index", ur);
        }

        public ActionResult ResendAPIKey(string userId)
        {
            _service.ResendAPIKey(userId);
            return View("Index");
        }

        public ActionResult CompleteRegistration(string userId)
        {
            _service.CompleteRegistration(userId);
            return View("Complete");
        }
    }
}
