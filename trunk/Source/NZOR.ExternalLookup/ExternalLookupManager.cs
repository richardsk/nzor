using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Provider;
using NZOR.Admin.Data.Entities;
using System.Reflection;
using NZOR.Admin.Data.Repositories;
using NZOR.Data.LookUps.Common;

namespace NZOR.ExternalLookups
{
    public class ExternalLookupManager
    {
        List<ExternalLookupService> _extServices;
        IProviderRepository _provRepository;
        TaxonRankLookUp _rankLookup;

        public ExternalLookupManager(List<ExternalLookupService> externalLookups, IProviderRepository providerRepository, TaxonRankLookUp rankLookup)
        {
            _extServices = externalLookups;
            _provRepository = providerRepository;
            _rankLookup = rankLookup;
        }

        public List<ExternalNameResult> GetMatchingNames(string fullName, bool topLevelOnly)
        {
            List<ExternalNameResult> matches = new List<ExternalNameResult>();
            
            foreach (ExternalLookupService svc in _extServices)
            {
                if (svc.LookupServiceClassName != null)
                {
                    AbstractExternalLookupProvider lookupSvc = (AbstractExternalLookupProvider)Activator.CreateInstance(Type.GetType(svc.LookupServiceClassName), 
                        new object[]{svc, _provRepository, _rankLookup});

                    string url = lookupSvc.GetNameSearchUrl(fullName);
                    string result = lookupSvc.DoNameLookup(url);

                    matches.AddRange(lookupSvc.ParseMatchingNames(result, topLevelOnly));
                }
            }

            return matches;
        }

        public List<ExternalNameResult> GetByExternalDataUrl(string dataUrl, bool topLevelOnly)
        {
            List<ExternalNameResult> names = new List<ExternalNameResult>();

            foreach (ExternalLookupService svc in _extServices)
            {
                if (svc.LookupServiceClassName != null && svc.IDLookupEndpoint != null && dataUrl.StartsWith(svc.IDLookupEndpoint, StringComparison.CurrentCultureIgnoreCase))
                {
                    AbstractExternalLookupProvider lookupSvc = (AbstractExternalLookupProvider)Activator.CreateInstance(Type.GetType(svc.LookupServiceClassName),
                        new object[] { svc, _provRepository, _rankLookup });

                    string result = lookupSvc.DoNameLookup(dataUrl);

                    names.AddRange(lookupSvc.ParseMatchingNames(result, topLevelOnly));                    
                }
            }

            return names;
        }
    }
}
