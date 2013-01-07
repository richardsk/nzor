using NZOR.Publish.Data.Repositories;
using NZOR.Publish.Model.Names;
using NZOR.Publish.Model.Search;
using NZOR.Publish.Model.Search.Names;
using NZOR.Web.Service.Helpers;
using NZOR.Web.Service.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Serialization;

namespace NZOR.Web.Service.APIs.Controllers
{
    public class NamesController : ApiController
    {
        private const int DefaultPageSize = 10;

        private readonly NameRepository _nameRepository;

        public NamesController(NameRepository nameRepository)
        {
            _nameRepository = nameRepository;
        }

        [Description("Returns a list of names based on filtering criteria.")]
        public NamePagedResponse GetNames(string parentNameId = "", string ancestorNameId = "", string biostatus = "", string status = "", string fromModifiedDate = "", string page = "", string pageSize = "")
        {
            var response = new NamePagedResponse();

            Guid parsedParentNameId;
            Guid? queryParentNameId = null;
            Guid parsedAncestorNameId;
            Guid? queryAncestorNameId = null;
            DateTime? parsedFromModifiedDate = Utility.ParseDate(fromModifiedDate);
            int parsedPage;
            int parsedPageSize;

            if (Guid.TryParse(parentNameId, out parsedParentNameId)) { queryParentNameId = parsedParentNameId; }
            if (Guid.TryParse(ancestorNameId, out parsedAncestorNameId)) { queryAncestorNameId = parsedAncestorNameId; }

            if (!int.TryParse(page, out parsedPage)) { parsedPage = 1; }
            if (!int.TryParse(pageSize, out parsedPageSize)) { parsedPageSize = DefaultPageSize; }

            parsedPageSize = Math.Max(parsedPageSize, DefaultPageSize);
            parsedPage = Math.Max(parsedPage, 1);

            var nameWhereResult = _nameRepository.Where(queryParentNameId, queryAncestorNameId, biostatus, status, parsedFromModifiedDate, (parsedPage - 1) * parsedPageSize, parsedPageSize);

            response.Names = nameWhereResult.Names;
            response.Total = nameWhereResult.Total;
            response.Page = parsedPage;
            response.PageSize = parsedPageSize;

            NLog.LogManager.GetCurrentClassLogger().Info("/names");

            return response;
        }

        [Description("Returns the NZOR name for the provider name specified")]
        public Name GetByProviderId(string id = "")
        {
            var name = _nameRepository.GetByProviderId(id);

            if (name == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(String.Format("The provider record id '{0}' cannot be found.", id)),
                        ReasonPhrase = "Provider Record Id Not Found"
                    };
                throw new HttpResponseException(response);
            }

            return name;
        }

        [Description("Returns a specific name by Id.")]
        public Name Get(string id)
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

            var name = _nameRepository.SingleOrDefault(parsedId);

            if (name == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(String.Format("The id '{0}' cannot be found.", id)),
                        ReasonPhrase = "Id Not Found"
                    };
                throw new HttpResponseException(response);
            }

            NLog.LogManager.GetCurrentClassLogger().Info("/names id: {0}", id.ToString());

            return name;
        }

        [Description("Returns a list of full names for completion lookups based on a partial full name.")]
        public List<string> GetLookups(string query = "", int? take = null)
        {
            List<string> lookups = _nameRepository.LookupNames(query, take.HasValue && take.Value > 0 ? take.Value : DefaultPageSize);

            return lookups;
        }

        [Description("Returns a list of search results and faceting details for a query on names.")]
        public NamesSearchResponse GetSearch(string query = "", string filter = "", string page = "", string pageSize = "", string orderBy = "")
        {
            int parsedPage;
            int parsedPageSize;

            if (!int.TryParse(page, out parsedPage)) { parsedPage = 1; }
            if (!int.TryParse(pageSize, out parsedPageSize)) { parsedPageSize = DefaultPageSize; }

            var searchRequest = new SearchRequest()
            {
                Query = query,
                Filter = filter,
                Page = parsedPage,
                PageSize = parsedPageSize,
                OrderBy = orderBy
            };

            var searchResponse = _nameRepository.Search(searchRequest);

            NLog.LogManager.GetCurrentClassLogger().Info("/names/search query: {0}", query);

            return searchResponse;
        }

        [XmlRoot(ElementName = "Response")]
        public class NamePagedResponse : PagedResponse
        {
            public List<Name> Names { get; set; }

            public NamePagedResponse()
            {
                Names = new List<Name>();
            }
        }
    }
}