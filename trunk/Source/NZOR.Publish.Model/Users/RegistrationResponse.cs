using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NZOR.Publish.Model.Users
{
    public class RegistrationResponse
    {        
        public string Status { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}