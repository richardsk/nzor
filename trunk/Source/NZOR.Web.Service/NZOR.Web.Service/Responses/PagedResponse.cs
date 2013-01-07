using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NZOR.Web.Service.Responses
{
    public abstract class PagedResponse
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PagedResponse()
        {
            Total = 0;
            Page = 0;
            PageSize = 0;
        }
    }
}