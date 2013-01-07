using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Xml.Serialization;
using NZOR.Publish.Data.Repositories;
using NZOR.Publish.Model.Common;
using NZOR.Web.Service.Helpers;
using NZOR.Web.Service.Responses;

namespace NZOR.Web.Service.APIs.Controllers
{
    public class DeprecatedRecordsController : ApiController
    {
        private const int DefaultPageSize = 10;

        private readonly DeprecatedRecordRepository _deprecatedRecordRepository;

        public DeprecatedRecordsController(DeprecatedRecordRepository deprecatedRecordRepository)
        {
            _deprecatedRecordRepository = deprecatedRecordRepository;
        }

        public DeprecatedRecordPagedResponse GetList(string fromDeprecatedDate = "", string page = "", string pageSize = "")
        {
            var response = new DeprecatedRecordPagedResponse();

            int parsedPage;
            int parsedPageSize;
            DateTime? parsedFromDeprecatedDate = Utility.ParseDate(fromDeprecatedDate);

            if (!int.TryParse(page, out parsedPage)) { parsedPage = 1; }
            if (!int.TryParse(pageSize, out parsedPageSize)) { parsedPageSize = DefaultPageSize; }

            parsedPageSize = Math.Max(parsedPageSize, DefaultPageSize);
            parsedPage = Math.Max(parsedPage, 1);

            var deprecatedRecords = _deprecatedRecordRepository.GetAll();

            if (parsedFromDeprecatedDate.HasValue)
            {
                deprecatedRecords = deprecatedRecords.Where(o => o.DeprecatedDate >= parsedFromDeprecatedDate.Value).ToList();
            }

            response.DeprecatedRecords = deprecatedRecords.Skip((parsedPage - 1) * parsedPageSize).Take(parsedPageSize).ToList();
            response.Total = deprecatedRecords.Count();
            response.Page = parsedPage;
            response.PageSize = parsedPageSize;

            return response;
        }

        [XmlRoot(ElementName = "Response")]
        public class DeprecatedRecordPagedResponse : PagedResponse
        {
            public List<DeprecatedRecord> DeprecatedRecords { get; set; }

            public DeprecatedRecordPagedResponse()
            {
                DeprecatedRecords = new List<DeprecatedRecord>();
            }
        }
    }
}