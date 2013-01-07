using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Web.Service.Client.Responses
{
    public abstract class PagedResponse
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
