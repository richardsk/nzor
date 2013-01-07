using System;
using System.Configuration;
using NUnit.Framework;
using NZOR.Admin.Data.Entities.Matching;
using NZOR.Admin.Data.Repositories.Matching;
using NZOR.Admin.Data.Sql.Repositories.Matching;

namespace NZOR.Admin.Data.Tests.Unit_Tests
{
    [TestFixture]
    class MatchRepositoryTest
    {
        private IMatchRepository _matchRepository;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            _matchRepository = new MatchRepository(connectionString);
        }

        // TODO: Replace these with real tests.

        [Test]
        public void CanSaveNewMatch()
        {
            var match = new Match();

            match.State = Entities.Entity.EntityState.Added;
            match.MatchId = Guid.NewGuid();
            match.SubmitterEmail = "test";

            _matchRepository.Save(match);
        }

        [Test]
        public void CanGetAndUpdateMatch()
        {
            TestFixtureSetUp();

            var match = _matchRepository.GetMatch(new Guid("4D1CF4B4-35B3-4E48-9C70-1235FA6C94D6"), true); //c5d26b06-2d0b-41a9-a6eb-a7e70e819af7

            match.State = Entities.Entity.EntityState.Modified;
            match.SubmitterEmail = "test updated";

            _matchRepository.Save(match);
        }
    }
}
