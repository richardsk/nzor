using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml.Serialization;
using NZOR.Publish.Data.Repositories;
using NZOR.Publish.Model.Common;
using NZOR.Web.Service.Responses;
using System.Net;

namespace NZOR.Web.Service.APIs
{
    public class ProvidersController : ApiController
    {
        private const int DefaultPageSize = 10;

        private readonly ProviderRepository _providerRepository;

        public ProvidersController(ProviderRepository providerRepository)
        {
            _providerRepository = providerRepository;
        }

        public ProviderPagedResponse GetList(string page = "", string pageSize = "")
        {
            var response = new ProviderPagedResponse();

            int parsedPage;
            int parsedPageSize;

            if (!int.TryParse(page, out parsedPage)) { parsedPage = 1; }
            if (!int.TryParse(pageSize, out parsedPageSize)) { parsedPageSize = DefaultPageSize; }

            parsedPageSize = Math.Max(parsedPageSize, DefaultPageSize);
            parsedPage = Math.Max(parsedPage, 1);

            List<Provider> providers = _providerRepository.GetAll();

            response.Providers = providers.Skip((parsedPage - 1) * parsedPageSize).Take(parsedPageSize).ToList();
            response.Total = providers.Count();
            response.Page = parsedPage;
            response.PageSize = parsedPageSize;

            return response;
        }

        [OutputCache(Duration = 100)]
        public Provider Get(string id)
        {
            Guid parsedId;

            if (!Guid.TryParse(id, out parsedId))
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(String.Format("The id '{0}' is not a valid identifier.", id)),
                    ReasonPhrase = "Id Invalid"
                };
                throw new HttpResponseException(response);
            }

            var provider = _providerRepository.SingleOrDefault(parsedId);

            if (provider == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(String.Format("The id '{0}' cannot be found.", id)),
                    ReasonPhrase = "Id Not Found"
                };
                throw new HttpResponseException(response);
            }

            return provider;
        }

        [XmlRoot(ElementName = "Response")]
        public class ProviderPagedResponse : PagedResponse
        {
            public List<Provider> Providers { get; set; }

            public ProviderPagedResponse()
            {
                Providers = new List<Provider>();
            }
        }
    }
}