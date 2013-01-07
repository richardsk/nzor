using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;
using NZOR.Data;
using NZOR.Matching;
using NZOR.Data.DataSets;
using NZOR.Data.Entities.Common;

using NZOR.Data.Repositories.Provider;
using NZOR.Data.Sql.Repositories.Provider;
using NZOR.Harvest;
using NZOR.Admin.Data.Repositories;
using NZOR.Admin.Data.Sql.Repositories;

namespace NZOR.Integration.Test.UnitTests
{
    [TestFixture]
    public class MatchingTest
    {
        [Test]
        public void TestMultipleParentMatch()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Development\\NZOR\\Source\\NZOR.Integration\\Configuration\\IntegConfig.xml");

            NZOR.Integration.MatchProcessor.LoadConfig(doc);

            ConfigSet cs = NZOR.Integration.MatchProcessor.GetMatchSet(1); //set number 1

            //
            //PREREQUISITE:  Need to run NZOR.Harvest.Test CanImportBaseDataSet and CanImportMultipleParentConcepts first
            //

            MatchData md = new MatchData();
            Data.Repositories.Provider.INameRepository npr = new Data.Sql.Repositories.Provider.NameRepository(cnnStr);

            Data.Entities.Provider.Name provName = npr.GetNameByProviderId("NZOR_Test", "8D03B9C1-DE3E-4DD8-99C4-01035CE70FC2");
            //name with multiple parents, no matches so should fail
            DsIntegrationName.ProviderNameRow pn = Data.Sql.Integration.GetNameMatchData(cnnStr, provName.NameId, null);
            MatchResult res = NZOR.Integration.MatchProcessor.DoMatch(md, pn, cs.Routines, true, cnnStr);

            Assert.That(res.Status, Is.EqualTo(LinkStatus.DataFail));

            //name with multiple parents and a single match so should Match
            provName = npr.GetNameByProviderId("NZOR_Test", "240AC4B2-AF94-4EFA-9115-DC70BD625BB8");
            pn = Data.Sql.Integration.GetNameMatchData(cnnStr, provName.NameId, null);
            res = NZOR.Integration.MatchProcessor.DoMatch(md, pn, cs.Routines, true, cnnStr);

            Assert.That(res.Status, Is.EqualTo(LinkStatus.Matched));

        }

        [Test]
        public void TestParseName()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            NZOR.Data.Sql.Repositories.Common.LookUpRepository lr = new Data.Sql.Repositories.Common.LookUpRepository(cnnStr);
            NZOR.Data.LookUps.Common.TaxonRankLookUp trl = new Data.LookUps.Common.TaxonRankLookUp(lr.GetTaxonRanks());

            NZOR.Data.Entities.Common.NameParseResult npr = Matching.NameParser.ParseName("Luzula banksiana E. Mey", trl);

            npr = Matching.NameParser.ParseName("Pylaiella littoralis", trl);
            npr = Matching.NameParser.ParseName("Luzula banksiana J. de Not.", trl);
            npr = Matching.NameParser.ParseName("Botryosphaeria dothidea (Moug.) Ces. & de Not.", trl);
            npr = Matching.NameParser.ParseName("Xylaria schreuderiana Van der Byl", trl);
        }

        [Test]
        public void TestGetFullName()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            Data.Sql.Repositories.Common.LookUpRepository lr = new Data.Sql.Repositories.Common.LookUpRepository(cnnStr);
            Data.LookUps.Common.TaxonRankLookUp trl = new Data.LookUps.Common.TaxonRankLookUp(lr.GetTaxonRanks());

            NZOR.Data.Entities.Common.NameParseResult npr = Matching.NameParser.ParseName("Luzula banksiana E. Mey", trl);

            string fullName = npr.GetFullName(trl, true, "ICBN");
            Assert.That(fullName == "Luzula banksiana E. Mey");

            fullName = npr.GetFullName(trl, false, "ICBN");
            Assert.That(fullName == "Luzula banksiana");

            npr = Matching.NameParser.ParseName("Luzula banksiana var. picta E. Mey", trl);
            fullName = npr.GetFullName(trl, true, "ICBN");
            Assert.That(fullName == "Luzula banksiana var. picta E. Mey");


            npr = Matching.NameParser.ParseName("(Salix matsudana X S. alba) X S. alba", trl);
            fullName = npr.GetFullName(trl, false, "ICBN");
            Assert.That(fullName == "(Salix matsudana X S. alba) X S. alba");

            npr = Matching.NameParser.ParseName("Salix matsudana × alba", trl);
            fullName = npr.GetFullName(trl, false, "ICBN");
            Assert.That(fullName == "Salix matsudana × alba");

            npr = Matching.NameParser.ParseName("Asplenium bulbiferum G.Forst. × Asplenium cimmeriorum Brownsey & De Lange", trl);
            fullName = npr.GetFullName(trl, false, "ICBN");
            Assert.That(fullName == "Asplenium bulbiferum × Asplenium cimmeriorum");
            fullName = npr.GetFullName(trl, true, "ICBN");
            Assert.That(fullName == "Asplenium bulbiferum G.Forst. × Asplenium cimmeriorum Brownsey & De Lange");

            npr = Matching.NameParser.ParseName("Algidia chiltoni oconnori Forster, 1954", trl);
            fullName = npr.GetFullName(trl, true, "ICZN");
            Assert.That(fullName == "Algidia chiltoni oconnori Forster");
        }

        [Test]
        public void TestMatchRule()
        {            
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Development\\NZOR\\Source\\NZOR.Integration\\Configuration\\IntegConfig.xml");

            NZOR.Integration.MatchProcessor.LoadConfig(doc);

            Matching.ConfigSet ruleSet = MatchProcessor.GetMatchSet(1);

            Matching.INameMatcher nm = ruleSet.GetRoutine(typeof(Matching.NamesWithPartialCanonical).ToString());

        }

        [Test]
        public void TestSuccessMatch()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Development\\NZOR\\Source\\NZOR.Integration\\Configuration\\IntegConfig.xml");
            
            NZOR.Integration.MatchProcessor.LoadConfig(doc);

            ConfigSet cs = NZOR.Integration.MatchProcessor.GetMatchSet(3);
            
            MatchData md = new MatchData();

            DsIntegrationName.ProviderNameRow pn = Data.Sql.Integration.GetNameMatchData(cnnStr, new Guid("74FFE18E-447B-4E05-A2D2-03C94CC6A772"), null); //0CE76217-3F48-4491-A340-883BA586A7C4, 6B922009-30DD-4853-836F-398BA4B2C972
            MatchResult res = NZOR.Integration.MatchProcessor.DoMatch(null, pn, cs.Routines, true, cnnStr);

            Assert.AreNotEqual(0, res.Matches.Count());
            
            
        }

        [Test]
        public void TestStackedNameMatch()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            Data.Sql.Repositories.Common.LookUpRepository lr = new Data.Sql.Repositories.Common.LookUpRepository(cnnStr);
            Data.LookUps.Common.TaxonRankLookUp trl = new Data.LookUps.Common.TaxonRankLookUp(lr.GetTaxonRanks());

            string name = "Amanita muscaria Lam.";
            NameParseResult npr = NameParser.ParseName(name, trl);

            System.Data.DataTable res = NZOR.Data.Sql.Matching.GetStackedNameMatches(cnnStr, npr, true, false);
            Assert.That(res.Rows.Count == 1);

            npr = Matching.NameParser.ParseName("Pylaiella littoralis", trl);

            res = NZOR.Data.Sql.Matching.GetStackedNameMatches(cnnStr, npr, true, false);
            Assert.That(res.Rows.Count == 1);
            
            npr = Matching.NameParser.ParseName("Xylaria schreuderiana Van der Byl", trl);

            res = NZOR.Data.Sql.Matching.GetStackedNameMatches(cnnStr, npr, true, false);
            Assert.That(res.Rows.Count == 1);

            npr = Matching.NameParser.ParseName("Antrodiella Poria undata sensu G. Cunn.", trl);

            res = NZOR.Data.Sql.Matching.GetStackedNameMatches(cnnStr, npr, true, false);
            Assert.That(res.Rows.Count == 1);

            npr = Matching.NameParser.ParseName("Phalacrocorax carbo novaehollandiae", trl);

            res = NZOR.Data.Sql.Matching.GetStackedNameMatches(cnnStr, npr, true, false);
            //Assert.That(res.Rows.Count == 1);


            npr = Matching.NameParser.ParseName("Apteryx australis", trl);
            res = NZOR.Data.Sql.Matching.GetStackedNameMatches(cnnStr, npr, true, false);
            Assert.That(res.Rows.Count == 1);

            npr = Matching.NameParser.ParseName("Apteryx australis australis", trl);
            res = NZOR.Data.Sql.Matching.GetStackedNameMatches(cnnStr, npr, true, false);
            Assert.That(res.Rows.Count == 0);
        }

        [Test()]
        public void TestSubspeciesMatch()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            INameRepository nameRepository = new NameRepository(cnnStr);
            IReferenceRepository referenceRepository = new ReferenceRepository(cnnStr);
            IConceptRepository conceptRepository = new ConceptRepository(cnnStr);
            NZOR.Data.Repositories.Common.ILookUpRepository lookUpRepository = new NZOR.Data.Sql.Repositories.Common.LookUpRepository(cnnStr);
            ITaxonPropertyRepository taxonPropRepository = new TaxonPropertyRepository(cnnStr);
            IAnnotationRepository annRepository = new AnnotationRepository(cnnStr);
            IProviderRepository provRepository = new ProviderRepository(cnnStr);

            XDocument document = XDocument.Load(@"C:\Development\NZOR\Source\NZOR.Integration.Test\Resources\Import Data\Integration-BaseDataSet.xml");
            Importer importer = new Importer(nameRepository, referenceRepository, conceptRepository, lookUpRepository, taxonPropRepository, annRepository, provRepository, @"C:\Development\NZOR\Schema\NZOR_Provider.xsd");
            importer.Import(document);
            importer.Save();

            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Development\\NZOR\\Source\\NZOR.Integration\\Configuration\\IntegConfig.xml");

            NZOR.Integration.MatchProcessor.LoadConfig(doc);

            ConfigSet cs = NZOR.Integration.MatchProcessor.GetMatchSet(4); //subsp
            

            NZOR.Data.Entities.Provider.Name provName = nameRepository.GetNameByProviderId("NZOR_Test", "A45F8E5B-3498-4CC7-8D7F-00A61BA1CF88");

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                NZOR.Data.Sql.Integration.UpdateProviderStackedNameData(provName.NameId, cnn);
            }


            NZOR.Data.Entities.Integration.DataForIntegration data = new Data.Entities.Integration.DataForIntegration(Data.Entities.Integration.IntegrationDatasetType.SingleNamesList);
            NZOR.Data.Sql.Integration.GetConsensusNameDataForIntegration(cnnStr, ref data);
            

            DsIntegrationName.ProviderNameRow tmppn = Data.Sql.Integration.GetNameMatchData(cnnStr, provName.NameId, null);

            data.References = new DsIntegrationReference();
            data.SingleNamesSet = new DsIntegrationName();
            data.SingleNamesSet.ProviderName.ImportRow(tmppn);


            String dataFilePath = @"C:\Development\NZOR\Source\NZOR.Integration\data.dat";

            NZOR.Data.Sql.Integration.SaveDataFile(data, dataFilePath);

            Admin.Data.Sql.Repositories.ProviderRepository pr = new Admin.Data.Sql.Repositories.ProviderRepository(cnnStr);
            List<Admin.Data.Entities.AttachmentPoint> attPoints = pr.GetAllAttachmentPoints();

            MemoryIntegrationProcessor processor = new MemoryIntegrationProcessor();
            processor.MaxThreads = 15;
            processor.RunIntegration(@"C:\Development\NZOR\Source\NZOR.Integration\Configuration\IntegConfig.xml", dataFilePath, -1, attPoints);

            while (processor.Progress < 100)
            {
                System.Threading.Thread.Sleep(2000);
            }

            //save data from file to DB
            data = NZOR.Data.Sql.Integration.LoadDataFile(dataFilePath); //results


            NZOR.Data.Sql.Integration.SaveIntegrationData(cnnStr, data, false);
            

            MatchData md = new MatchData(false, false, data, null, null);

            provName = nameRepository.GetNameByProviderId("NZOR_Test", "23F6CF13-BF2A-423A-8C5F-BA1CDCAAC66D");
            DsIntegrationName.ProviderNameRow pn = Data.Sql.Integration.GetNameMatchData(cnnStr, provName.NameId, null); 
            MatchResult res2 = NZOR.Integration.MatchProcessor.DoMatch(md, pn, cs.Routines, true, cnnStr);


            Assert.AreEqual(res2.Matches.Count, 1);

        }


        [Test]
        public void TestMatch()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            
            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Development\\NZOR\\Source\\NZOR.Integration\\Configuration\\IntegConfig.xml");
            //doc.Load("C:\\Development\\NZOR\\trunk\\NZOR\\Integration\\Configuration\\IntegConfig.xml");

            NZOR.Integration.MatchProcessor.LoadConfig(doc);

            ConfigSet cs = NZOR.Integration.MatchProcessor.GetMatchSet(1); //set number 1

            //
            //PREREQUISITE:  Need to run Insert_Test_Data_1.sql first
            //

            MatchData md = new MatchData();
            
            //test Asterales test provider name (E6AB7DCC-45CD-43B1-A353-DC62BE296847)
            DsIntegrationName.ProviderNameRow pn = Data.Sql.Integration.GetNameMatchData(cnnStr, new Guid("E6AB7DCC-45CD-43B1-A353-DC62BE296847"), null);
            MatchResult res = NZOR.Integration.MatchProcessor.DoMatch(md, pn, cs.Routines, true, cnnStr);

            Assert.AreNotEqual(0, res.Matches.Count());

            Guid batchId = Guid.NewGuid();

            Admin.Data.Sql.Repositories.ProviderRepository pr = new Admin.Data.Sql.Repositories.ProviderRepository(cnnStr);
            List<Admin.Data.Entities.AttachmentPoint> attPoints = pr.GetAllAttachmentPoints();

            //insert name
            IntegratorThread it = new NZOR.Integration.IntegratorThread(md, false, batchId, -1);
            Guid provNameId = new Guid("E6AB7DCC-45CD-43B1-A353-DC62BE296847");
            IntegrationData data = new IntegrationData(provNameId, "Asterales", new List<Guid>(), cs, true, cnnStr, batchId, attPoints);
            it.AddNameData(data);
            it.ProcessNameProc(null);

            Assert.AreEqual(NZOR.Data.LinkStatus.Matched, it.NameResult(provNameId).Status);

            //test Family Testaceae below Asterales (C6A58A2E-315E-4EDD-91C0-8663A8584C69)
            it = new IntegratorThread(md, false, batchId, -1);
            provNameId = new Guid("C6A58A2E-315E-4EDD-91C0-8663A8584C69");
            data = new IntegrationData(provNameId, "Testaceae Smith", new List<Guid>(), cs, true, cnnStr, batchId, attPoints);
            it.AddNameData(data);
            it.ProcessNameProc(null);

            Assert.AreEqual(NZOR.Data.LinkStatus.Matched, it.NameResult(provNameId).Status);

            //test genus integration (3CF39BEE-E713-4063-9CA5-5EB05D6CE8F1)
            it = new IntegratorThread(md, false, batchId, -1);
            provNameId = new Guid("3CF39BEE-E713-4063-9CA5-5EB05D6CE8F1");
            data = new IntegrationData(provNameId, "Testgenus", new List<Guid>(), cs, true, cnnStr, batchId, attPoints);
            it.AddNameData(data);
            it.ProcessNameProc(null);

            Assert.That(NZOR.Data.LinkStatus.Matched, Is.EqualTo(it.NameResult(provNameId).Status));

            //test species  (10A906E5-0CAB-4524-9BFC-FCD728D19060)
            it = new IntegratorThread(md, false, batchId, -1);
            provNameId = new Guid("10A906E5-0CAB-4524-9BFC-FCD728D19060");
            data = new IntegrationData(provNameId, "Testgenus testsp Smith", new List<Guid>(), cs, true, cnnStr, batchId, attPoints);
            it.AddNameData(data);
            it.ProcessNameProc(null);

            Assert.AreEqual(NZOR.Data.LinkStatus.Matched, it.NameResult(provNameId).Status);



            //test full match hierarchy/paths
            //pn = NZOR.Data.ProviderName.GetNameMatchData(new Guid("0BAEEFF2-2BD4-4818-99B3-000365BF0DE3")); //118A1FE7-59E4-4C9B-83C4-01D71E6E5C00")); 
            //res = NZOR.Integration.Integrator.DoMatch(pn, routines);

            //Assert.AreNotEqual(0, res.Matches.Count());


            //test match parent
            //pn = NZOR.Data.ProviderName.GetNameMatchData(new Guid("C8F0B9CB-2B22-4649-9380-880AD0BB826F")); //118A1FE7-59E4-4C9B-83C4-01D71E6E5C00"));            
            //res = NZOR.Integration.Integrator.DoMatch(pn, routines);

            //Assert.AreNotEqual(0, res.Matches.Count());

        }

        [Test]
        public void TestDataMatch()
        {

            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Development\\NZOR\\Source\\NZOR.Integration\\Configuration\\IntegConfig.xml");
            
            NZOR.Integration.MatchProcessor.LoadConfig(doc);

            ConfigSet cs = NZOR.Integration.MatchProcessor.GetMatchSet(1); //set number 1

            NZOR.Data.Entities.Integration.DataForIntegration data = new Data.Entities.Integration.DataForIntegration(Data.Entities.Integration.IntegrationDatasetType.SingleNamesList);
            NZOR.Data.Sql.Integration.GetConsensusNameDataForIntegration(cnnStr, ref data);

            MatchData md = new MatchData(false, false, data, null, null);
            
            DsIntegrationName.ProviderNameRow tmppn = Data.Sql.Integration.GetNameMatchData(cnnStr, new Guid("4BC7A6A1-C734-4B63-969D-80997DDD60EB"), null);

            MatchResult tmpres = NZOR.Integration.MatchProcessor.DoMatch(md, tmppn, cs.Routines, false, cnnStr);
        }

        private System.Timers.Timer _timer = new System.Timers.Timer();

        [Test]
        public void TestLevenshtein()
        {
            Assert.AreEqual(0, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", "this is a test"));
            Assert.AreEqual(1, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", "this is a test too"));
            Assert.AreEqual(4, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", "this is not going to pass"));
            Assert.AreEqual(4, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", "none the same"));
            Assert.AreEqual(4, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", ""));
            Assert.AreEqual(1, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", "this is another test"));
            Assert.AreEqual(2, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", "this is test a"));
            Assert.AreEqual(2, NZOR.Matching.Utility.LevenshteinDistanceWords("this is\na test", "this is test a"));
            Assert.AreEqual(2, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", " this  is test a"));

        }
    }
}
