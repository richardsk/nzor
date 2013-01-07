using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Provider;
using NZOR.Admin.Data.Entities;
using NZOR.Admin.Data.Repositories;
using NZOR.Data.LookUps.Common;
using System.Net;
using System.IO;

namespace NZOR.ExternalLookups
{
    public abstract class AbstractExternalLookupProvider
    {
        protected ExternalLookupService LookupService { get; set; }
        protected IProviderRepository ProviderRepository { get; set; }
        protected TaxonRankLookUp RankLookup { get; set; }
        
        public AbstractExternalLookupProvider(ExternalLookupService svc, IProviderRepository provRepository, TaxonRankLookUp rankLookup)
        {
            LookupService = svc;
            ProviderRepository = provRepository;
            RankLookup = rankLookup;
        }

        public abstract string GetNameSearchUrl(string fullName);
        public abstract string GetNameResolutionUrl(string id);

        public abstract List<ExternalNameResult> ParseMatchingNames(string nameSearchResult, bool topLevelOnly);

        public string DoNameLookup(string url)
        {
            WebRequest req = WebRequest.Create(url);

            StreamReader rdr = new StreamReader(req.GetResponse().GetResponseStream());
            string result = rdr.ReadToEnd();

            return result;
        }
    }
}
