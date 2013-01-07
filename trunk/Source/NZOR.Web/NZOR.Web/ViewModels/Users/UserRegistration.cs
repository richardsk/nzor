using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NZOR.Publish.Model.Users;

namespace NZOR.Web.ViewModels.Users
{
    public class UserRegistration
    {
        public string Email { get; set; }
        public string Name { get; set; }        
        public string Password { get; set; }
        public string Organisation { get; set; }        
        public RegistrationResponse RegistrationResponse { get; set; }
    }
}