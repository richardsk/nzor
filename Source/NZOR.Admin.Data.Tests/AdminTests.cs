using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NZOR.Admin.Data.Entities;

namespace NZOR.Admin.Data.Tests
{
    [TestFixture]    
    public class TestAdmin
    {
        [Test]
        public void CanGetProviders()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            Admin.Data.Sql.Repositories.ProviderRepository pr = new Sql.Repositories.ProviderRepository(cnnStr);

            Assert.That(pr.Providers.Count > 0);

            NZOR.Admin.Data.Entities.Provider prov = pr.GetProvider(new Guid("2FED116B-82FA-496C-8386-DE53722FD947"));

            Assert.That(prov, Is.Not.Null);
            Assert.That(prov.AttachmentPoints.Count > 0);
        }

        [Test]
        public void CanGetDataSourceEndpoint()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            Admin.Data.Sql.Repositories.ProviderRepository pr = new Sql.Repositories.ProviderRepository(cnnStr);
            List<Admin.Data.Entities.DataSourceEndpoint> dsList = pr.GetDatasetEndpoints(new Guid("175D49CD-0785-4008-BB56-04DF3E46DE13"));

            Assert.That(dsList.Count > 0);
            Assert.That(dsList[0].Schedule != null);

            string dt = dsList[0].LastHarvestDate.Value.ToString("s");
        }

        [Test]
        public void CanGetStatistics()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            Admin.Data.Repositories.IAdminRepository ar = new Admin.Data.Sql.Repositories.AdminRepository(cnnStr);

            Entities.NZORStatistics stats = ar.GetStatistics();

            Assert.That(stats, Is.Not.Null);
            Assert.That(stats.NZORNameCount > 0);
        }

        [Test]
        public void CanUpdateStatistics()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            Admin.Data.Repositories.IAdminRepository ar = new Admin.Data.Sql.Repositories.AdminRepository(cnnStr);

            ar.UpdateStatistics();
        }

        [Test]
        public void CanGetPendingNameRequests()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            Admin.Data.Repositories.IAdminRepository ar = new Admin.Data.Sql.Repositories.AdminRepository(cnnStr);

            List<NameRequest> nrList = ar.GetPendingNameRequests();
        }
    }
}
