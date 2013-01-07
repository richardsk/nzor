using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NZOR.Admin.Data.Sql.Repositories;
using NZOR.Admin.Data.Entities;
using NZOR.Server;
using NZOR.Admin.Data.Repositories;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Sql.Repositories.Common;

namespace NZOR.ExternalLookups.Test
{
    [TestFixture]
    public class ExternalLookupsTests
    {
        [TestCase()]
        public void CanAddNameRequest()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            AdminRepository ar = new AdminRepository(cnnStr);

            NameRequest nr = new NameRequest();
            nr.AddedBy = "System";
            nr.AddedDate = DateTime.Now;
            nr.FullName = "Euchiton brassii";
            nr.NameRequestId = Guid.NewGuid();
            nr.RequestDate = DateTime.Now;
            nr.ApiKey = "123";
            nr.RequesterEmail = "richardsk@landcareresearch.co.nz";
            nr.Status = NameRequest.Statuses.Pending;
            
            nr.State = Entity.EntityState.Added;

            ar.NameRequests.Add(nr);
            ar.Save();
        }

        [TestCase()]
        public void CanProcessNameRequest()
        {
            NameRequestProcessor nrp = new NameRequestProcessor();
            nrp.Run();

            do
            {
                System.Threading.Thread.Sleep(6000);  
            } 
            while (nrp.State == NameRequestProcessor.StateProcessing);

            nrp.Stop();
        }

        [TestCase]
        public void CanLookupName()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            ExternalLookupRepository ExLookupRep = new ExternalLookupRepository(cnnStr);
            IProviderRepository provRep = new ProviderRepository(cnnStr);
            LookUpRepository lr = new LookUpRepository(cnnStr);
            TaxonRankLookUp rankLookup = new TaxonRankLookUp(lr.GetTaxonRanks());

            ExternalLookupManager extMgr = new ExternalLookupManager(ExLookupRep.ListLookupServices(), provRep, rankLookup);

            List<ExternalNameResult> names = extMgr.GetMatchingNames("Lobesia botrana", false);
            Assert.That(names.Count > 0);

            names = extMgr.GetMatchingNames("Agaricus phalloides", false);
            Assert.That(names.Count > 0);

            names = extMgr.GetMatchingNames("Amanita phalloides (Vaill. ex Fr.) Link", true);
            Assert.That(names.Count > 0);

            names = extMgr.GetByExternalDataUrl("http://www.catalogueoflife.org/col/webservice?response=full&id=10657254", false);
            Assert.That(names.Count > 0);
        }

    }
}
