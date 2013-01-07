using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Sql.Repositories.Common;
using System.Configuration;

namespace NZOR.Data.Test.UnitTests
{
    [TestFixture]
    public class TaxonRankLookUpTest
    {
        private TaxonRankLookUp _taxonRankLookUp;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            LookUpRepository lookUpRespository = new LookUpRepository(connectionString);

            _taxonRankLookUp = new TaxonRankLookUp(lookUpRespository.GetTaxonRanks());
        }

        [Test]
        public void GetFindASpecificTaxonRank()
        {
            TestFixtureSetUp();

            Assert.That(_taxonRankLookUp.GetTaxonRank("species", "ICBN").Name, Is.EqualTo("species"));
            Assert.That(_taxonRankLookUp.GetTaxonRank("species", null).Name, Is.EqualTo("species"));
            Assert.That(_taxonRankLookUp.GetTaxonRank("section", "ICZN").Name, Is.EqualTo("section"));
            Assert.That(_taxonRankLookUp.GetTaxonRank("subsection", "ICZN").SortOrder, Is.EqualTo(1880));
        }

        [Test]
        public void NonExistentTaxonRankReturnsNull()
        {
            Assert.That(_taxonRankLookUp.GetTaxonRank("xyz", "ICBN"), Is.Null);
        }
    }
}
