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
    public class NameClassLookUpTest
    {
        private NameClassLookUp _nameClassLookUp;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            LookUpRepository lookUpRepository = new LookUpRepository(connectionString);

            _nameClassLookUp = new NameClassLookUp(lookUpRepository.GetNameClasses());
        }

        [Test]
        public void CanFindASpecificNameClass()
        {
            TestFixtureSetUp();
            Assert.That(_nameClassLookUp.GetNameClass("Scientific Name").NameClassId, Is.EqualTo(new Guid("A5233111-61A0-4AE6-9C2B-5E8E71F1473A")));
        }
    }
}
