using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Admin.Data.Entities;
using NZOR.Admin.Data.Sql.Repositories;
using System.Configuration;
using NZOR.Matching.Batch.Helpers;
using NZOR.ExternalLookups;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Data.LookUps.Common;

namespace NZOR.Matching.Batch
{
    public class BrokeredNameRequestProcessor
    {
        private string _cnnStr = "";
        private ExternalLookupManager _extMgr = null;

        public BrokeredNameRequestProcessor(string connectionString)
        {
            _cnnStr = connectionString;

            LookUpRepository lr = new NZOR.Data.Sql.Repositories.Common.LookUpRepository(_cnnStr);
            TaxonRankLookUp rankLookup = new TaxonRankLookUp(lr.GetTaxonRanks());

            var externalLookupRepository = new Admin.Data.Sql.Repositories.ExternalLookupRepository(_cnnStr);
            List<ExternalLookupService> extLookups = externalLookupRepository
               .ListLookupServices()
               .Where(o => !o.DataFormat.Equals("HTML", StringComparison.OrdinalIgnoreCase))
               .ToList();

            ProviderRepository provRepository = new ProviderRepository(_cnnStr);

            _extMgr = new ExternalLookupManager(extLookups, provRepository, rankLookup);
        }

        public void ProcessBrokeredNameRequests(string email, string apiKey, string namesData)
        {
            AdminRepository adminRep = new AdminRepository(_cnnStr);
                        
            List<NameMatchResult> results = new List<NameMatchResult>();

            // Parse the input data.
            var inputDataParser = new InputDataParser();
            inputDataParser.IDColumnName = "DataUrl";
            inputDataParser.FullNameColumnName = "FullName";
            List<SubmittedName> submittedNames = inputDataParser.ParseInputData(namesData);

            foreach (var submittedName in submittedNames)
            {
                NameRequest exnr = adminRep.GetNameRequestByFullName(submittedName.ScientificName);
                if (exnr == null)
                {
                    //get by external data url
                    //only broker if we can resolve this name
                    List<ExternalNameResult> names = _extMgr.GetByExternalDataUrl(submittedName.Id, true);

                    if (names.Count > 0)
                    {
                        foreach (ExternalNameResult enr in names)
                        {
                            NameRequest nr = new NameRequest();
                            nr.ApiKey = apiKey;
                            nr.AddedBy = "System";
                            nr.AddedDate = DateTime.Now;
                            nr.FullName = enr.Name.FullName;
                            nr.NameRequestId = Guid.NewGuid();
                            nr.RequestDate = DateTime.Now;
                            nr.RequesterEmail = email;
                            nr.ExternalLooksupServiceId = enr.LookupService.ExternalLookupServiceId;
                            nr.ExternalLookupDataUrl = enr.DataUrl;
                            nr.Status = NameRequest.Statuses.Pending;

                            nr.State = Entity.EntityState.Added;

                            adminRep.NameRequests.Add(nr);
                        }
                    }
                }
            }

            adminRep.Save();
        }
    }
}
