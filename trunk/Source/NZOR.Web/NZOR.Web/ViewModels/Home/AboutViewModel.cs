using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NZOR.Web.ViewModels
{
    public class AboutViewModel
    {
        public string Description { get; set; }
        public string Version { get; set; }
        public string WebServiceUrl { get; set; }

        public AboutViewModel()
        {
            Description = String.Empty;
            Version = String.Empty;
            WebServiceUrl = String.Empty;
        }
    }
}