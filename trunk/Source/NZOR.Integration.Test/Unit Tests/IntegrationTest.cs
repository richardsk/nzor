using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Xml;

using NZOR.Integration;
using NZOR.Data.Repositories.Provider;
using NZOR.Data.Sql.Repositories.Provider;
using NZOR.Data.Entities.Integration;
using NZOR.Harvest;
using NZOR.Admin.Data.Sql.Repositories;
using NZOR.Admin.Data.Entities;

namespace NZOR.Integration.Test.Unit_Tests
{
    [TestFixture]
    public class IntegrationTest
    {

        //[Test]
        //public void TestIntegrateVersion3()
        //{
        //    string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
        //    string adminCnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

        //    Database.Test.Helper.DatabaseTestHelper hlp = new Database.Test.Helper.DatabaseTestHelper(cnnStr);
        //    hlp.ExecuteSql(NZOR.Data.Test.DatabaseTestSql.GetSql(NZOR.Data.Test.DatabaseTestSql.SqlScript.InsertTestConsensusNameData));

        //    hlp = new Database.Test.Helper.DatabaseTestHelper(adminCnnStr);
        //    hlp.ExecuteSql(NZOR.Data.Test.DatabaseTestSql.GetSql(NZOR.Data.Test.DatabaseTestSql.SqlScript.InsertTestAdminData));


        //    //NZOR.Data.Sql.Integration.UpdateProviderStackedNameData(cnnStr);

        //    //use grouped data for integration
        //    Data.Entities.Integration.DataForIntegration data = NZOR.Data.Sql.Integration.GetGroupedDataForIntegration(Data.Entities.Integration.IntegrationDataGroup.FirstCharacterOfTaxonName, cnnStr, adminCnnStr, null);

        //    String dataFilePath = @"C:\Development\NZOR\Source\NZOR.Integration\data.dat";
                        
        //    NZOR.Data.Sql.Integration.SaveDataFile(data, dataFilePath);

        //    IntegrationProcessor3 processor = new IntegrationProcessor3();
        //    processor.RunIntegration(@"C:\Development\NZOR\Source\NZOR.Integration\Configuration\IntegConfig.xml", dataFilePath, 1000);

        //    while (processor.Progress < 100)
        //    {
        //        System.Threading.Thread.Sleep(2000);
        //    }

        //    //save data from file to DB
        //    data = NZOR.Data.Sql.Integration.LoadDataFile(dataFilePath); //results

        //    NZOR.Data.Sql.Integration.SaveIntegrationData(cnnStr, adminCnnStr, data, false);
            
        //}

        [Test()]
        [Ignore("Only needs running with the debugger")]
        public void TestClearData()
        {
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            NZOR.Data.Sql.Integration.ClearConsensusData(connectionString);
            NZOR.Data.Sql.Integration.ClearProviderData(connectionString);
        }

        [Test]
        [Ignore("Only needs running with the debugger")]
        public void TestProcessProviderConflict()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            Data.Sql.Integration.ProcessProviderConceptConflicts(cnnStr);
        }

        [Test]
        [Ignore("Only needs running with the debugger")]
        public void TestConceptConflict()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            Data.Repositories.Provider.IConceptRepository pcr = new Data.Sql.Repositories.Provider.ConceptRepository(cnnStr);
            Data.Repositories.Consensus.IConceptRepository ccr = new Data.Sql.Repositories.Consensus.ConceptRepository(cnnStr);

            List<Data.Entities.Provider.Concept> concepts = pcr.GetProviderConceptsByName(new Guid("25EFD607-F7EC-4743-A913-9FEF97E2EA23"));

            ConsensusValueResult cvr = Data.Sql.Integration.GetConsensusValue(concepts, new Guid("6A11B466-1907-446F-9229-D604579AA155"), ccr);

            if (cvr.HasMajority)
            {
                //set to consval
                //Guid? accToId = null;
                //if (cvr.Value == row["NameTo1"]) accToId = accToId1;
                //if (cvr.Value == row["NameTo2"]) accToId = accToId2;

                //ccr.SetInUseConcept(nameId, relTypeId, (Guid?)accToId, (Guid?)cvr.Value);
            }
            else
            {
                Guid dsId1 = new Guid("175D49CD-0785-4008-BB56-04DF3E46DE13");
                Guid dsId2 = new Guid("F6235951-CA30-4449-87F3-9159BEEBFB24");

                //conflict
                Guid prefDsId = Data.Sql.Integration.GetRankedDataSource(cnnStr, new Guid("25EFD607-F7EC-4743-A913-9FEF97E2EA23"), dsId1, dsId2);

                if (prefDsId != Guid.Empty)
                {

                    //get value to use
                    //object val = null;
                    //object seq = null;
                    //object accToId = null;
                    //if (dsId1 == prefDsId)
                    //{
                    //    val = row["NameTo1"];
                    //    seq = row["Sequence1"];
                    //    accToId = accToId1;
                    //}
                    //else if (dsId2 == prefDsId)
                    //{
                    //    val = row["NameTo2"];
                    //    seq = row["Sequence2"];
                    //    accToId = accToId2;
                    //}
                    //else //no preferred - take first - TODO check
                    //{
                    //    val = row["NameTo1"];
                    //    seq = row["Sequence1"];
                    //    accToId = accToId1;
                    //}

                    ccr.SetInUseConcept(new Guid("25EFD607-F7EC-4743-A913-9FEF97E2EA23"), new Guid("6A11B466-1907-446F-9229-D604579AA155"), 
                        new Guid("55742CA4-42BC-46A9-A3C9-DDE52D5BF0D0"), new Guid("fd23fe24-41b2-4e28-94f5-2729e314d5a7"));
                }
            }
        }
        
        [Test]
        [Ignore("Only needs running with the debugger")]
        public void TestInitialIntegration()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            XmlDocument config = new XmlDocument();
            config.Load(@"C:\Development\NZOR\Source\NZOR.Integration\Configuration\IntegConfig.xml");

            NZOR.Data.Sql.Integration.UpdateProviderStackedNameData(cnnStr);

            SQLIntegrationProcessor processor = new SQLIntegrationProcessor();
            //processor.MaxThreads = 10;            
            processor.RunInitialIntegration(config, false);

        }

        [Test()]
        //[Ignore("Only needs running with the debugger")]
        public void TestUpdateIntegration()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            XmlDocument config = new XmlDocument();
            config.Load(@"C:\Development\NZOR\Source\NZOR.Integration\Configuration\IntegConfig.xml");

            NZOR.Data.Sql.Integration.UpdateProviderStackedNameData(cnnStr);

            SQLIntegrationProcessor processor = new SQLIntegrationProcessor();

            processor.RunUpdateIntegration(config);
        }

        [Test()]
        public void TestRemoveNoProviderNames()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            Data.Sql.Integration.RemoveNamesWithNoProvider(cnnStr);
        }

        [Test()]
        public void TestPostIntegration()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            Data.Sql.Integration.PostIntegrationCleanup(cnnStr);
        }

        [Test()]
        //[Ignore("Only needs running with the debugger")]
        public void TestRemainingIntegration()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            NZOR.Data.Sql.Integration.UpdateProviderStackedNameData(cnnStr);

            XmlDocument config = new XmlDocument();
            config.Load(@"C:\Development\NZOR\Source\NZOR.Integration\Configuration\IntegConfig.xml");

            IntegrationProcessor proc = new IntegrationProcessor();
            proc.RunIntegration(config, -1);

            //check for conflicts and use preferred provider ranking
            Data.Sql.Integration.ProcessProviderDataConflicts(cnnStr);

            //concept conflicts
            Data.Sql.Integration.ProcessProviderConceptConflicts(cnnStr);
        }

        //[Test]
        //public void TestIntegrateNames5()
        //{
        //    string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
        //    string adminCnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

        //    XmlDocument config = new XmlDocument();
        //    config.Load(@"C:\Development\NZOR\Source\NZOR.Integration\Configuration\IntegConfig.xml");

        //    IntegrationProcessor5 processor = new IntegrationProcessor5();

        //    MatchProcessor.LoadConfig(config);

        //    processor.ConnectionString = cnnStr;
            
        //    //Data.DataSets.DsDistinctName names = processor.GetDistinctNames(true);

        //    processor.RunIntegration(config);
            
        //}

        [Test]
        public void CanMatchNamesSimple()
        {
            //string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            
            //NZOR.Data.Entities.Common.NameParseResult npr = Matching.NameParser.ParseName("Luzula banksiana E. Mey");

            //SimpleMatchProcessor smp = new SimpleMatchProcessor();
            //List<Matching.MatchResult> mrList = smp.DoNameMatching(cnnStr, new Data.Entities.Common.NameParseResultCollection() { npr });

            //foreach (Matching.MatchResult mr in mrList)
            //{
            //    Assert.That(mr.MatchedId != "");
            //}
        }

        [Test()]
        public void CanRefreshAnnotations()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            Data.Sql.Integration.RefreshNameAnnotations(cnnStr, new Guid("8BAC44C1-333F-4FE0-8671-0005A38B33CF"));

            Data.Sql.Integration.RefreshConceptAnnotations(cnnStr, new Guid("77E1644F-11F2-4976-931F-0001BFE00FC1"));
        }

        [Test]
        public void CanMatchCsv()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            CSVProcessor proc = new CSVProcessor();

            Mapping.IntegrationMapping im = new Mapping.IntegrationMapping();
            
            NZOR.Integration.Mapping.ColumnMapping cm = new Integration.Mapping.ColumnMapping(0, "ProviderRecordId", Integration.Mapping.ColumnMapping.ColumnMapType.Column,
                    Integration.Mapping.NZORIntegrationField.NZORFields(false, new Guid("A5233111-61A0-4AE6-9C2B-5E8E71F1473A"), true)["ProviderRecordId"], "");
            im.AddMapping(cm);

            cm = new Integration.Mapping.ColumnMapping(1, "FullName", Integration.Mapping.ColumnMapping.ColumnMapType.Column,
                    Integration.Mapping.NZORIntegrationField.NZORFields(false, new Guid("A5233111-61A0-4AE6-9C2B-5E8E71F1473A"), true)["FullName"], "");
            im.AddMapping(cm);
        
            im.HasColumnHeaders = true;
            im.NameClassID = new Guid("A5233111-61A0-4AE6-9C2B-5E8E71F1473A");

            proc.ParseSimpleProviderData(cnnStr, @"C:\Development\NZOR\FungalHostWorstCasetest.csv", im);
        }

        [Test]
        public void CanMatchSimilarVernaculars()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            string commonName = "Spiny seadragon";

            List<DataRow> matches = new List<DataRow>();

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select fullname, nameid from consensus.name where nameclassid = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5'";

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (NZOR.Matching.Utility.LevenshteinWordsPercent(dr["fullname"].ToString(), commonName) > 95)
                        {
                            matches.Add(dr);
                        }
                    }
                }
            }

            int matchCount = matches.Count;

            NZOR.Matching.NamesWithSimilarFullName nwsf = new Matching.NamesWithSimilarFullName();
            nwsf.DBConnectionString = cnnStr;
            nwsf.Threshold = 95;
            string cmt = "";
            Data.DataSets.DsIntegrationName dsn = new Data.DataSets.DsIntegrationName();
            Data.DataSets.DsIntegrationName.ProviderNameRow pn = dsn.ProviderName.NewProviderNameRow();
            pn.FullName = "Spiny seadragon";
            pn.NameClassID = new Guid("05BCC19C-27E8-492C-8ADD-EC5F73325BC5");
            pn.NameID = Guid.NewGuid();

            Data.DataSets.DsNameMatch nm = nwsf.GetMatchingNames(pn, ref cmt);
        }


        [Test]
        [Ignore("Only needs running with the debugger")]
        public void TestIntegrateNames2()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            //Database.Test.Helper.DatabaseTestHelper hlp = new Database.Test.Helper.DatabaseTestHelper(cnnStr);
            //hlp.ExecuteSql(NZOR.Data.Test.DatabaseTestSql.GetSql(NZOR.Data.Test.DatabaseTestSql.SqlScript.InsertTestConsensusNameData));

            //hlp = new Database.Test.Helper.DatabaseTestHelper(adminCnnStr);
            //hlp.ExecuteSql(NZOR.Data.Test.DatabaseTestSql.GetSql(NZOR.Data.Test.DatabaseTestSql.SqlScript.InsertTestAdminData));


            //NZOR.Data.Sql.Integration.UpdateProviderStackedNameData(cnnStr);

            Data.Entities.Integration.DataForIntegration data = NZOR.Data.Sql.Integration.GetAllDataForIntegration(cnnStr, null);

            String dataFilePath = @"C:\Development\NZOR\Source\NZOR.Integration\data.dat";

            NZOR.Data.Sql.Integration.SaveDataFile(data, dataFilePath);
            
           Admin.Data.Sql.Repositories.ProviderRepository pr = new Admin.Data.Sql.Repositories.ProviderRepository(cnnStr);
           List<Admin.Data.Entities.AttachmentPoint> attPoints = pr.GetAllAttachmentPoints();

            MemoryIntegrationProcessor processor = new MemoryIntegrationProcessor();
            processor.MaxThreads = 15;
            processor.RunIntegration(@"C:\Development\NZOR\Source\NZOR.Integration\Configuration\IntegConfig.xml", dataFilePath, 4000, attPoints);

            while (processor.Progress < 100)
            {
                System.Threading.Thread.Sleep(2000);
            }

            //save data from file to DB
            data = NZOR.Data.Sql.Integration.LoadDataFile(dataFilePath); //results

            NZOR.Data.Sql.Integration.SaveIntegrationData(cnnStr, data, false);

        }

        [Test]
        [Ignore("Only needs running with the debugger")]
        public void TestUnintegrateNames()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            //import data first?
            INameRepository nameRepository = new NameRepository(cnnStr);
            Data.Entities.Provider.Name pn = nameRepository.GetNameByProviderId("NZOR_Test", "FE087528-E01F-4706-AADE-BC3377AD5457");

            if (pn == null || !pn.ConsensusNameId.HasValue)
            {
                IReferenceRepository referenceRepository = new ReferenceRepository(cnnStr);
                IConceptRepository conceptRepository = new ConceptRepository(cnnStr);
                NZOR.Data.Repositories.Common.ILookUpRepository lookUpRepository = new NZOR.Data.Sql.Repositories.Common.LookUpRepository(cnnStr);
                ITaxonPropertyRepository taxonPropRepository = new TaxonPropertyRepository(cnnStr);
                IAnnotationRepository annRepo = new AnnotationRepository(cnnStr);
                ProviderRepository provRepository = new ProviderRepository(cnnStr);

                XDocument document = XDocument.Load(@"C:\Development\NZOR\Source\NZOR.Integration.Test\Resources\Import Data\Integration-BaseDataSet.xml");
                Importer importer = new Importer(nameRepository, referenceRepository, conceptRepository, lookUpRepository, taxonPropRepository, annRepo, provRepository, @"C:\Development\NZOR\Schema\NZOR_Provider.xsd");
                importer.Import(document);
                importer.Save();

                //integrate
                Admin.Data.Repositories.IProviderRepository pr = new Admin.Data.Sql.Repositories.ProviderRepository(cnnStr);
                Admin.Data.Entities.DataSource ds = pr.GetDataSourceByCode("NZOR_Test_2");

                List<Data.Entities.Provider.Name> names = nameRepository.GetNames(ds.DataSourceId);
                using (SqlConnection cnn = new SqlConnection(cnnStr))
                {
                    cnn.Open();
                    foreach (Data.Entities.Provider.Name provName in names)
                    {
                        NZOR.Data.Sql.Integration.UpdateProviderStackedNameData(provName.NameId, cnn);
                    }
                }

                List<Admin.Data.Entities.AttachmentPoint> attPoints = pr.GetAllAttachmentPoints();

                Data.Entities.Integration.DataForIntegration data = Data.Sql.Integration.GetAllDataForIntegration(cnnStr, ds.DataSourceId);
                NZOR.Data.Sql.Integration.SaveDataFile(data, @"C:\Development\NZOR\Source\NZOR.Integration\data.dat");
                Integration.MemoryIntegrationProcessor processor = new MemoryIntegrationProcessor();
                processor.MaxThreads = 25;
                processor.RunIntegration(@"C:\Development\NZOR\Source\NZOR.Integration\Configuration\IntegConfig.xml", @"C:\Development\NZOR\Source\NZOR.Integration\data.dat", -1, attPoints);
                while (processor.Progress < 100)
                {
                    System.Threading.Thread.Sleep(2000);
                }

                data = NZOR.Data.Sql.Integration.LoadDataFile(@"C:\Development\NZOR\Source\NZOR.Integration\data.dat"); //results

                NZOR.Data.Sql.Integration.SaveIntegrationData(cnnStr, data, false);
                
            }


            //test unintegration
            pn = nameRepository.GetNameByProviderId("NZOR_Test", "FE087528-E01F-4706-AADE-BC3377AD5457");
            bool canUnintegrate = nameRepository.CanUnintegrateName(pn);
            Assert.That(canUnintegrate, Is.False);

            pn = nameRepository.GetNameByProviderId("NZOR_Test", "053A6CA7-2EA3-4E55-817D-47C66971D643");
            canUnintegrate = nameRepository.CanUnintegrateName(pn);
            Assert.That(canUnintegrate, Is.False);

            pn = nameRepository.GetNameByProviderId("NZOR_Test", "C4DC0AA4-DEA7-4A56-B82F-35CBF03A2A9B");
            canUnintegrate = nameRepository.CanUnintegrateName(pn);
            Assert.That(canUnintegrate, Is.False);

            pn = nameRepository.GetNameByProviderId("NZOR_Test", "5BCE967B-9094-4F8E-8E6B-5BB06AF4DF2E");
            canUnintegrate = nameRepository.CanUnintegrateName(pn);
            Assert.That(canUnintegrate, Is.False);

            pn = nameRepository.GetNameByProviderId("NZOR_Test", "43A9E9D0-7EAD-4491-B17A-955E7709FCDB");
            canUnintegrate = nameRepository.CanUnintegrateName(pn);
            Assert.That(canUnintegrate, Is.False);

            pn = nameRepository.GetNameByProviderId("NZOR_Test", "9865E8CC-EB5C-447F-8961-03A47C0A5B35");
            canUnintegrate = nameRepository.CanUnintegrateName(pn);
            Assert.That(canUnintegrate, Is.True);
        }

        [Test]
        public void TestNameRefresh()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            //get the first name at rank 'order' to refresh
            string sql = @"select top 1 NameID from consensus.Name where TaxonRankID = '2B1966D4-720B-4F58-9C01-1280E1BB0DAB'";
            Guid NameId = Guid.Empty;

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    NameId = (Guid)cmd.ExecuteScalar();
                }
            }

            Admin.Data.Sql.Repositories.ProviderRepository pRep = new Admin.Data.Sql.Repositories.ProviderRepository(cnnStr);

            List<Admin.Data.Entities.AttachmentPoint> attPoints = pRep.GetAllAttachmentPoints();

            NZOR.Data.Sql.Integration.RefreshConsensusName(NameId, cnnStr, attPoints);

            //refresh a vernacular
            sql = @"select top 1 NameID from consensus.Name where NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5'";
            NameId = Guid.Empty;

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    NameId = (Guid)cmd.ExecuteScalar();
                }
            }

            NZOR.Data.Sql.Integration.RefreshConsensusName(NameId, cnnStr, attPoints);

            //CACB00E1-BD72-45C5-BC50-B13CCF546E0D
            //B3CF8453-0207-4A06-806E-99D697A87868
            NZOR.Data.Sql.Integration.RefreshConsensusName(new Guid("CACB00E1-BD72-45C5-BC50-B13CCF546E0D"), cnnStr, attPoints);
        }

        public void CanRemoveBrokeredNames()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            UpdateProcessor up = new UpdateProcessor();
            AdminRepository ar = new AdminRepository(cnnStr);
            List<NameRequest> reqs = ar.GetNameRequestsByApiKey("123");
            foreach (NameRequest nr in reqs)
            {
                List<BrokeredName> bnList = ar.GetBrokeredNamesForNameRequest(nr.NameRequestId);
                foreach (BrokeredName bn in bnList)
                {
                    up.RemoveBrokeredName(bn, cnnStr);
                }

                nr.Status = "Pending";
                nr.State = Entity.EntityState.Modified;
                ar.NameRequests.Add(nr);
            }
            ar.Save();
        }
    }
}
