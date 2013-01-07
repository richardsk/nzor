using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NZOR.Admin.Data.Repositories;
using NZOR.Admin.Data.Sql.Repositories;
using System.Configuration;
using NZOR.Admin.Data.Entities;

namespace NZOR.Admin.Data.Tests.Unit_Tests
{
    [TestFixture]
    public class ProviderTest
    {
        private IProviderRepository _provRepository;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            _provRepository = new ProviderRepository(connectionString);
        }

        [Test]
        public void CanGetProviders()
        {
            List<Provider> provs = _provRepository.GetProviders();
            Assert.That(provs.Count > 0);
        }

        [Test]
        public void CanGetProviderByCode()
        {
            Provider prov = _provRepository.GetProviderByCode("NZFLORA");
            Assert.That(prov != null);
        }
    }
}
