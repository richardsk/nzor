using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NZOR.Admin.Data.Entities;
using NZOR.Web.Admin.Models;
using NZOR.Data.Entities.Provider;

namespace NZOR.Web.Admin.Controllers
{
    public class AttachmentPointController : Controller
    {
        //
        // GET: /AttachmentPoint/

        public ActionResult Index()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            NZOR.Admin.Data.Repositories.IProviderRepository pr = new NZOR.Admin.Data.Sql.Repositories.ProviderRepository(cnnStr);
            List<AttachmentPoint> attPoints = pr.GetAllAttachmentPoints();
            List<AttachmentPointDataSource> attPointDS = pr.GetAttachmentPointDataSources();
            List<Provider> providers = pr.GetProviders();
            List<DataSource> dataSources = pr.GetAllDataSources();

            NZOR.Data.Sql.Repositories.Common.LookUpRepository lookupRep = new Data.Sql.Repositories.Common.LookUpRepository(cnnStr);
            NZOR.Data.LookUps.Common.TaxonRankLookUp rankLookup = new Data.LookUps.Common.TaxonRankLookUp(lookupRep.GetTaxonRanks());
            NZOR.Data.Repositories.Provider.INameRepository nameRep = new NZOR.Data.Sql.Repositories.Provider.NameRepository(cnnStr);

            AttachmentPointModel model = new AttachmentPointModel();
            model.AttachmentPointDetails = new List<AttachmentPointDetail>();
            foreach (AttachmentPoint ap in attPoints)
            {
                DataSource ds = providers.Single(p => p.ProviderId == ap.ProviderId).DataSources.Single(d => d.DataSourceId == ap.DataSourceId);
                AttachmentPointDetail apd = new AttachmentPointDetail();
                
                apd.AttPointDataSources = new List<AttachmentPointDSDetail>();
                attPointDS.Where(apds => apds.AttachmentPointId == ap.AttachmentPointId).ToList().ForEach(a =>
                    apd.AttPointDataSources.Add(new AttachmentPointDSDetail
                    {
                        DataSource = dataSources.Single(d => d.DataSourceId == a.DataSourceId).Name,
                        Ranking = a.Ranking
                    }));

                apd.Provider = providers.Single(p => p.ProviderId == ap.ProviderId).Name;
                apd.DataSource = ds.Name;
                apd.FullName = ap.FullName;
                apd.ProviderRecordId = ap.ProviderRecordId;
                
                Name pn = nameRep.GetNameByProviderId(ds.Code, ap.ProviderRecordId);
                if (pn == null)
                {
                    apd.FullName += " - PROVIDER NAME MISSING";
                }
                else
                {
                    apd.Rank = rankLookup.GetTaxonRank(pn.TaxonRankId).Name;
                    apd.SortOrder = rankLookup.GetTaxonRank(pn.TaxonRankId).SortOrder.Value;
                }

                model.AttachmentPointDetails.Add(apd);
            }

            return View(model);
        }

    }
}
