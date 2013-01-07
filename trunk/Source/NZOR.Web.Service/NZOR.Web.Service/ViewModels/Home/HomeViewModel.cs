using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NZOR.Web.Service.ViewModels.Home
{
    public class HomeViewModel
    {
        public string Description { get; set; }
        public string Version { get; set; }

        public HomeViewModel()
        {
            Description = String.Empty;
            Version = String.Empty;
        }
    }
}