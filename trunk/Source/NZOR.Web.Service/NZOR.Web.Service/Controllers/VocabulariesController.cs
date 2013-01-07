using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.ComponentModel;
using NZOR.Publish.Model.Common;
using NZOR.Web.Service.Responses;
using System.Xml.Serialization;
using NZOR.Publish.Data.Repositories;

namespace NZOR.Web.Service.APIs.Controllers
{
    public class VocabulariesController : ApiController
    {
        private const int DefaultPageSize = 100;

        private readonly VocabularyRepository _vocabularyRepository;
        private readonly TaxonPropertyRepository _taxonPropertyRepository;
        private readonly TaxonRankRepository _taxonRankRepository;

        public VocabulariesController(VocabularyRepository vocabularyRepository,
            TaxonPropertyRepository taxonPropertyRepository,
            TaxonRankRepository taxonRankRepository)
        {
            _vocabularyRepository = vocabularyRepository;
            _taxonPropertyRepository = taxonPropertyRepository;
            _taxonRankRepository = taxonRankRepository;
        }

        [Description("")]
        public VocabularyPagedResponse GetVocabularies()
        {
            return null;
        }

        /// <summary>
        /// Returns a list of taxon property records by use.
        /// </summary>
        /// <param name="use"></param>
        /// <remarks>
        /// Spaces are removed from the uses to allow the use url parameter to work with or without spaces.
        /// </remarks>
        public TaxonPropertyPagedResponse GetVocabulary(string use)
        {
            var response = new TaxonPropertyPagedResponse();

            use = use.Replace(" ", "");

            var taxonProperties = _taxonPropertyRepository.GetAll().Where(o => o.Class.Replace(" ", "").Equals(use, StringComparison.OrdinalIgnoreCase)).ToList();
            response.TaxonProperties = taxonProperties;
            response.Page = 1;
            response.Total = taxonProperties.Count();
            response.PageSize = response.Total;

            return response;
        }

        public VocabularyPagedResponse GetConceptRelationships()
        {
            var response = new VocabularyPagedResponse();

            response.Vocabularies = _vocabularyRepository.GetAll().Where(o => o.Uses.Contains("Concept Relationships")).ToList();
            response.Page = 1;
            response.Total = response.Vocabularies.Count();
            response.PageSize = response.Total;

            return response;
        }

        public TaxonRankPagedResponse GetTaxonRanks(string page = "", string pageSize = "")
        {
            var response = new TaxonRankPagedResponse();

            int parsedPage;
            int parsedPageSize;

            if (!int.TryParse(page, out parsedPage)) { parsedPage = 1; }
            if (!int.TryParse(pageSize, out parsedPageSize)) { parsedPageSize = DefaultPageSize; }

            parsedPageSize = Math.Max(parsedPageSize, DefaultPageSize);
            parsedPage = Math.Max(parsedPage, 1);

            var taxonRanks = _taxonRankRepository.GetAll();

            response.TaxonRanks = taxonRanks.Skip((parsedPage - 1) * parsedPageSize).Take(parsedPageSize).ToList();
            response.Total = taxonRanks.Count();
            response.Page = parsedPage;
            response.PageSize = parsedPageSize;

            return response;
        }

        public TaxonPropertyPagedResponse GetTaxonProperties()
        {
            var response = new TaxonPropertyPagedResponse();

            var taxonProperties = _taxonPropertyRepository.GetAll();

            response.TaxonProperties = taxonProperties;
            response.Page = 1;
            response.Total = taxonProperties.Count();
            response.PageSize = response.Total;

            return response;
        }

        public TaxonPropertyPagedResponse GetTaxonProperty(string name )
        {
            var response = new TaxonPropertyPagedResponse();

            var taxonProperties = _taxonPropertyRepository.GetAll().Where(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
            response.TaxonProperties = taxonProperties;
            response.Page = 1;
            response.Total = taxonProperties.Count();
            response.PageSize = response.Total;

            return response;
        }

        [XmlRoot(ElementName = "Response")]
        public class TaxonPropertyPagedResponse : PagedResponse
        {
            public List<TaxonProperty> TaxonProperties { get; set; }

            public TaxonPropertyPagedResponse()
            {
                TaxonProperties = new List<TaxonProperty>();
            }
        }

        [XmlRoot(ElementName = "Response")]
        public class TaxonRankPagedResponse : PagedResponse
        {
            public List<TaxonRank> TaxonRanks { get; set; }

            public TaxonRankPagedResponse()
            {
                TaxonRanks = new List<TaxonRank>();
            }
        }

        [XmlRoot(ElementName = "Response")]
        public class VocabularyPagedResponse : PagedResponse
        {
            public List<Vocabulary> Vocabularies { get; set; }

            public VocabularyPagedResponse()
            {
                Vocabularies = new List<Vocabulary>();
            }
        }
    }
}