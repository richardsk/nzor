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
    public class GeographicSchemasController : ApiController
    {
        private const int DefaultPageSize = 10;

        private readonly GeographicSchemaRepository _geographicSchemaRepository;

        public GeographicSchemasController(GeographicSchemaRepository geographicSchemaRepository)
        {
            _geographicSchemaRepository = geographicSchemaRepository;
        }

        public GeographicSchemaPagedResponse GetList(string page = "", string pageSize = "")
        {
            var response = new GeographicSchemaPagedResponse();

            int parsedPage;
            int parsedPageSize;

            if (!int.TryParse(page, out parsedPage)) { parsedPage = 1; }
            if (!int.TryParse(pageSize, out parsedPageSize)) { parsedPageSize = DefaultPageSize; }

            parsedPageSize = Math.Max(parsedPageSize, DefaultPageSize);
            parsedPage = Math.Max(parsedPage, 1);

            var geographicSchemas = _geographicSchemaRepository.GetAll();

            response.GeographicSchemas = geographicSchemas.Skip((parsedPage - 1) * parsedPageSize).Take(parsedPageSize).ToList();
            response.Total = geographicSchemas.Count();
            response.Page = parsedPage;
            response.PageSize = parsedPageSize;

            return response;
        }

        [OutputCache(Duration = 100)]
        public GeographicSchema Get(string id)
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

            var geographicSchema = _geographicSchemaRepository.SingleOrDefault(parsedId);

            if (geographicSchema == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(String.Format("The provider record id '{0}' cannot be found.", id)),
                    ReasonPhrase = "Provider Record Id Not Found"
                };
                throw new HttpResponseException(response);
            }

            return geographicSchema;
        }

        [XmlRoot(ElementName = "Response")]
        public class GeographicSchemaPagedResponse : PagedResponse
        {
            public List<GeographicSchema> GeographicSchemas { get; set; }

            public GeographicSchemaPagedResponse()
            {
                GeographicSchemas = new List<GeographicSchema>();
            }
        }
    }
}
