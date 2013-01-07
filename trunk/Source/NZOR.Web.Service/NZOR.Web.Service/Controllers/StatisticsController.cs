using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Xml.Serialization;
using NZOR.Publish.Data.Repositories;
using NZOR.Publish.Model.Common;
using NZOR.Web.Service.Helpers;
using NZOR.Web.Service.Responses;
using NZOR.Publish.Model.Administration;

namespace NZOR.Web.Service.APIs.Controllers
{
    public class StatisticsController : ApiController
    {
        private const int DefaultPageSize = 100;

        private readonly StatisticRepository _statisticRepository;

        public StatisticsController(StatisticRepository statisticRepository)
        {
            _statisticRepository = statisticRepository;
        }

        public StatisticPagedResponse GetList(string fromDate = "", string page = "", string pageSize = "")
        {
            var response = new StatisticPagedResponse();

            int parsedPage;
            int parsedPageSize;
            DateTime? parsedFromDate = Utility.ParseDate(fromDate);

            parsedPageSize = DefaultPageSize;

            if (!int.TryParse(page, out parsedPage)) { parsedPage = 1; }
            if (!int.TryParse(pageSize, out parsedPageSize)) 
            { 
                parsedPageSize = DefaultPageSize;                
            }

            parsedPage = Math.Max(parsedPage, 1);

            var statistics = _statisticRepository.GetAll();

            if (parsedFromDate.HasValue)
            {
                statistics = statistics.Where(o => o.Date >= parsedFromDate.Value).ToList();
            }

            response.Statistics = statistics.OrderByDescending(s => s.Date).Skip((parsedPage - 1) * parsedPageSize).Take(parsedPageSize).ToList();
            response.Total = statistics.Count();
            response.Page = parsedPage;
            response.PageSize = parsedPageSize;

            return response;
        }

        [XmlRoot(ElementName = "Response")]
        public class StatisticPagedResponse : PagedResponse
        {
            public List<Statistic> Statistics { get; set; }

            public StatisticPagedResponse()
            {
                Statistics = new List<Statistic>();
            }
        }
    }
}