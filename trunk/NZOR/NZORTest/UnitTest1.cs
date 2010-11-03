using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

using NZOR.Integration;
using NZOR.Data;

namespace NZORTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            //
            // TODO: Add constructor logic here
            //

        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        [TestMethod]
        public void TestSystemData()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnStr);
            cnn.Open();

            Assert.AreNotEqual(Guid.Empty, NZOR.Data.ConceptRelationshipType.ParentRelationshipTypeID(cnn));
            Assert.AreNotEqual(Guid.Empty, NZOR.Data.ConceptRelationshipType.PreferredRelationshipTypeID(cnn));
            Assert.IsNotNull(NZOR.Data.SystemData.TaxonRankData.GenusRank(cnn));

            cnn.Close();
        }

        [TestMethod]
        public void TestMatch()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnStr);
            cnn.Open();

            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Development\\NZOR\\Dev\\NZOR\\Integration\\Configuration\\IntegConfig.xml");
            //doc.Load("C:\\Development\\NZOR\\trunk\\NZOR\\Integration\\Configuration\\IntegConfig.xml");
            
            NZOR.Integration.MatchProcessor.LoadConfig(doc);

            ConfigSet cs = NZOR.Integration.MatchProcessor.GetMatchSet(1); //set number 1
            
            //
            //PREREQUISITE:  Need to run Insert_Test_Data_1.sql first
            //

            //test Asterales test provider name (E6AB7DCC-45CD-43B1-A353-DC62BE296847)
            DsIntegrationName pn = NZOR.Data.ProviderName.GetNameMatchData(cnn, new Guid("E6AB7DCC-45CD-43B1-A353-DC62BE296847")); 
            NZOR.Data.MatchResult res = NZOR.Integration.MatchProcessor.DoMatch(pn, cs.Routines, true, cnn);

            Assert.AreNotEqual(0, res.Matches.Count());

            //insert name
            IntegratorThread it = new NZOR.Integration.IntegratorThread();
            Guid provNameId = new Guid("E6AB7DCC-45CD-43B1-A353-DC62BE296847");
            IntegrationData data = new IntegrationData(provNameId, "Asterales", Guid.Empty, cs, true, cnnStr);
            it.AddNameData(data);
            it.ProcessName(null);

            Assert.AreEqual(NZOR.Data.LinkStatus.Matched, it.Result(provNameId).Status);

            //test Family Testaceae below Asterales (C6A58A2E-315E-4EDD-91C0-8663A8584C69)
            it = new IntegratorThread();
            provNameId = new Guid("C6A58A2E-315E-4EDD-91C0-8663A8584C69");
            data = new IntegrationData(provNameId, "Testaceae Smith", Guid.Empty, cs, true, cnnStr);
            it.AddNameData(data);
            it.ProcessName(null);

            Assert.AreEqual(NZOR.Data.LinkStatus.Matched, it.Result(provNameId).Status);

            //test genus integration (3CF39BEE-E713-4063-9CA5-5EB05D6CE8F1)
            it = new IntegratorThread();
            provNameId = new Guid("3CF39BEE-E713-4063-9CA5-5EB05D6CE8F1");
            data = new IntegrationData(provNameId, "Testgenus", Guid.Empty, cs, true, cnnStr);
            it.AddNameData(data);
            it.ProcessName(null);

            Assert.AreEqual(NZOR.Data.LinkStatus.Matched, it.Result(provNameId).Status);

            //test species  (10A906E5-0CAB-4524-9BFC-FCD728D19060)
            it = new IntegratorThread();
            provNameId = new Guid("10A906E5-0CAB-4524-9BFC-FCD728D19060");
            data = new IntegrationData(provNameId, "Testgenus testsp Smith", Guid.Empty, cs, true, cnnStr);
            it.AddNameData(data);
            it.ProcessName(null);

            Assert.AreEqual(NZOR.Data.LinkStatus.Matched, it.Result(provNameId).Status);



            //test full match hierarchy/paths
            //pn = NZOR.Data.ProviderName.GetNameMatchData(new Guid("0BAEEFF2-2BD4-4818-99B3-000365BF0DE3")); //118A1FE7-59E4-4C9B-83C4-01D71E6E5C00")); 
            //res = NZOR.Integration.Integrator.DoMatch(pn, routines);

            //Assert.AreNotEqual(0, res.Matches.Count());


            //test match parent
            //pn = NZOR.Data.ProviderName.GetNameMatchData(new Guid("C8F0B9CB-2B22-4649-9380-880AD0BB826F")); //118A1FE7-59E4-4C9B-83C4-01D71E6E5C00"));            
            //res = NZOR.Integration.Integrator.DoMatch(pn, routines);

            //Assert.AreNotEqual(0, res.Matches.Count());

            cnn.Close();
        }

        private System.Timers.Timer _timer = new System.Timers.Timer();
        
        [TestMethod]
        public void TestLevenshtein()
        {
            Assert.AreEqual(0, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", "this is a test"));
            Assert.AreEqual(1, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", "this is a test too"));
            Assert.AreEqual(4, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", "this is not going to pass"));
            Assert.AreEqual(4, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", "none the same"));
            Assert.AreEqual(4, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", ""));
            Assert.AreEqual(1, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", "this is another test"));
            Assert.AreEqual(2, NZOR.Matching.Utility.LevenshteinDistanceWords("this is a test", "this is test a"));
        }
    }
}
