using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Data;

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
            Assert.AreNotEqual(Guid.Empty, NZOR.Data.ConceptRelationshipType.ParentRelationshipTypeID());
            Assert.AreNotEqual(Guid.Empty, NZOR.Data.ConceptRelationshipType.PreferredRelationshipTypeID());
            Assert.IsNotNull(NZOR.Data.SystemData.TaxonRankData.GenusRank());

        }

        [TestMethod]
        public void TestMatch()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Development\\NZOR\\Dev\\NZOR\\Integration\\Configuration\\IntegConfig.xml");
            //doc.Load("C:\\Development\\NZOR\\trunk\\NZOR\\Integration\\Configuration\\IntegConfig.xml");

            DataSet pn = NZOR.Data.ProviderName.GetNameMatchData(new Guid("0BAEEFF2-2BD4-4818-99B3-000365BF0DE3")); //118A1FE7-59E4-4C9B-83C4-01D71E6E5C00"));

            List<NZOR.Matching.INameMatcher> routines = NZOR.Integration.Integrator.LoadConfig(doc, 1);
            
            //test full match hierarchy/paths
            List<NZOR.Matching.NameMatch> matches = NZOR.Integration.Integrator.DoMatch(pn, routines);

            Assert.AreNotEqual(0, matches.Count);


            //test match parent
            pn = NZOR.Data.ProviderName.GetNameMatchData(new Guid("C8F0B9CB-2B22-4649-9380-880AD0BB826F")); //118A1FE7-59E4-4C9B-83C4-01D71E6E5C00"));            
            matches = NZOR.Integration.Integrator.DoMatch(pn, routines);
                
            Assert.AreNotEqual(0, matches.Count);
                        
        }

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
