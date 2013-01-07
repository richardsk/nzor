using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NZOR.Web.Service.Controllers
{
    public class FilesController : Controller
    {
        public ActionResult Index(string fileName)
        {
            return File(fileName, "*/*", Server.UrlEncode(fileName));
        }
    }
}
