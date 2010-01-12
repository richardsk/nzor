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
        public void TestMatch()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Development\\NZOR\\Dev\\NZOR\\Integration\\Configuration\\IntegConfig.xml");

            DataSet pn = NZOR.Data.ProviderName.GetNameMatchData(new Guid("88441283-026F-4EB2-9925-00556C4D2ABE"));

            List<NZOR.Matching.INameMatcher> routines = NZOR.Integration.Integrator.LoadConfig(doc, 1);
            List<NZOR.Matching.NameMatch> matches = NZOR.Integration.Integrator.DoMatch(pn, routines);

            Assert.AreNotEqual(0, matches.Count);
        }
    }
}
