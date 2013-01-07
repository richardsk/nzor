using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Administration;

namespace NZOR.Web.Service.Client.Responses
{
    public class StatisticsResponse : PagedResponse
    {
        public List<Statistic> Statistics { get; set; }
    }
}
