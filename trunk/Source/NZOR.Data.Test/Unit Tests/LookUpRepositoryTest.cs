using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NZOR.Data.Sql.Repositories.Common;
using System.Configuration;

namespace NZOR.Data.Test.UnitTests
{
    [TestFixture]
    public class LookUpRepositoryTest
    {
        private LookUpRepository _lookUpRepository;
        
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            _lookUpRepository = new LookUpRepository(connectionString);
        }

        [Test]
        public void CanGetTaxonRanks()
        {
            Assert.That(_lookUpRepository, Is.Not.Null);

            Assert.That(_lookUpRepository.GetTaxonRanks().Count, Is.EqualTo(50));
        }


    }
}
