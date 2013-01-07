﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using NZOR.Data.LookUps.Common;
using NZOR.Data.Repositories.Common;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Data.Entities.Common;
using NZOR.Data.Entities.Provider;

namespace NZOR.Harvest.Test.Unit_Tests.Harvest
{
    [TestFixture()]
    public class HarvestTest
    {
        [Test()]
        public void CanGetTaxonRankNone()
        {
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            ILookUpRepository lr = new LookUpRepository(connectionString);
            TaxonRankLookUp trl = new TaxonRankLookUp(lr.GetTaxonRanks());

            TaxonRank tr = trl.GetTaxonRank("none", null);
            Assert.That(tr != null);

            tr = trl.GetTaxonRank("none", "ICBN");
            Assert.That(tr != null);
        }

        [Test()]
        [Ignore("Only needs running with the debugger")]
        public void TestHarvest()
        {
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            Harvester harvester = new Harvester();
            harvester.RunHarvesting(connectionString, true);
        }

        [Test()]
        //[Ignore("Only needs running with the debugger")]
        public void TestInitialHarvest()
        {
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            String provCnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Name_Cache"].ConnectionString;

            //NZOR.Data.Sql.Integration.ClearConsensusData(connectionString);
            
            Harvester harvester = new Harvester();

            //NZOR.Data.Sql.Integration.ClearProviderData(connectionString, new Guid("175D49CD-0785-4008-BB56-04DF3E46DE13"));
            harvester.RunInitialHarvest(Admin.Data.Entities.ProviderCode.NZFUNGI, provCnnStr, connectionString);

            //NZOR.Data.Sql.Integration.ClearProviderData(connectionString, new Guid("F6235951-CA30-4449-87F3-9159BEEBFB24"));
            harvester.RunInitialHarvest(Admin.Data.Entities.ProviderCode.NZFLORA, provCnnStr, connectionString);
            
            //NZOR.Data.Sql.Integration.ClearProviderData(connectionString, new Guid("C93F3E15-92DA-4E93-9DE0-416F937CC8E5"));            
            harvester.RunInitialHarvest(Admin.Data.Entities.ProviderCode.NZIB, provCnnStr, connectionString);

            //NZOR.Data.Sql.Integration.ClearProviderData(connectionString, new Guid("DC793129-7C07-4B4C-B496-B0D9AAE4620F"));            
            harvester.RunInitialHarvest(Admin.Data.Entities.ProviderCode.NZAC, provCnnStr, connectionString);

            //NZOR.Data.Sql.Integration.ClearProviderData(connectionString, new Guid("144D59E3-C85D-4173-B25A-41EE6008B6C6"));
            harvester.RunInitialHarvest(Admin.Data.Entities.ProviderCode.NZOR_Hosted, provCnnStr, connectionString);

            //harvester.RunInitialHarvest(Admin.Data.Entities.ProviderCode.NZOR_Test_2, provCnnStr, connectionString);

            
        }

        [Test()]
        //[Ignore("Only needs running with the debugger")]
        public void TestHarvestEndpoint()
        {
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            Harvester harvester = new Harvester();
            NZOR.Admin.Data.Repositories.IProviderRepository pr = new NZOR.Admin.Data.Sql.Repositories.ProviderRepository(connectionString);
            Admin.Data.Entities.Provider prov = pr.GetProvider(new Guid("91CBF580-E513-4CA4-89AF-724253A393AC")); //NZFUNGI E4E30DC8-C708-445F-86C3-235E0028F129"));
            List<Admin.Data.Entities.DataSourceEndpoint> epList = pr.GetDatasetEndpoints(new Guid("C93F3E15-92DA-4E93-9DE0-416F937CC8E5")); //NZFUNGI 175D49CD-0785-4008-BB56-04DF3E46DE13")); //2DD748D7-0CF4-4A74-8E01-3464F688603B")); //nzor test

            harvester.HarvestEndpoint(epList[0], prov, "NZIB", connectionString, true);

            //epList = pr.GetDatasetEndpoints(new Guid("F710B2D6-B492-4104-845B-49990AFB1ABB")); //nzor test 2

            //harvester.HarvestEndpoint(epList[0], connectionString);
        }

        //[Test]
        //public void Tmp()
        //{
        //    SortedList<string, Concept> cl = new SortedList<string, Concept>();
        //    Concept c = new Concept();
        //    cl.Add("21e66c87-bd2b-41e4-8b22-a225f344f7ae", c);
        //    Assert.True(cl.ContainsKey("21E66C87-BD2B-41E4-8B22-A225F344F7AE"));

        //}
    }
}
