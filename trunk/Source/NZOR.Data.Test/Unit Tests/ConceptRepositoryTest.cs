using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Database.Test.Helper;
using NUnit.Framework;
using NZOR.Data.Entities.Common;
using NZOR.Data.Entities.Provider;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Repositories.Common;
using NZOR.Data.Repositories.Provider;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Data.Sql.Repositories.Provider;
using NZOR.Admin.Data.Repositories;
using NZOR.Admin.Data.Lookups;
using NZOR.Admin.Data.Sql.Repositories;
using NZOR.Admin.Data.Entities;

namespace NZOR.Data.Test.UnitTests
{
    [TestFixture]
    public class ConceptRepositoryTest
    {
        private IConceptRepository _conceptRepository;
        private ILookUpRepository _lookUpRepository;
        private IProviderRepository _provRepository;

        private ConceptRelationshipTypeLookUp _conceptRelationshipTypeLookUp;
        private ConceptApplicationTypeLookUp _conceptApplicationTypeLookUp;
        private ProviderLookUp _providerLookUp;

        private DatabaseTestHelper _databaseTestHelper;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            _conceptRepository = new ConceptRepository(connectionString);
            _lookUpRepository = new LookUpRepository(connectionString);
            _provRepository = new ProviderRepository(connectionString);

            _conceptRelationshipTypeLookUp = new ConceptRelationshipTypeLookUp(_lookUpRepository.GetConceptRelationshipTypes());
            _conceptApplicationTypeLookUp = new ConceptApplicationTypeLookUp(_lookUpRepository.GetConceptApplicationTypes());
            _providerLookUp = new ProviderLookUp(_provRepository.GetProviders());

            _databaseTestHelper = new DatabaseTestHelper(connectionString);

            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.ResetDatabase));
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertProviderData));
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertBaseProviderNameData));
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertBaseProviderReferenceData));
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertBaseProviderConceptData));
        }

        [SetUp]
        public void TestSetUp()
        {
            _conceptRepository.Concepts.Clear();
        }

        [Test]
        public void CanSaveNewConcept()
        {
            Concept concept = new Concept();
            Provider provider = _providerLookUp.GetProvider("NZFLORA");
            DataSource dataSource = provider.DataSources.Where(o => o.Code == "NZFLORA").SingleOrDefault();

            concept.ConceptId = Guid.NewGuid();

            var record = _databaseTestHelper.Single(table: "provider.Concept", target: new { ConceptID = concept.ConceptId.ToString() });

            Assert.That(record, Is.Empty);

            concept.NameId = new Guid("4DDCF563-6D19-4926-9911-88EEB6B841EE");
            concept.DataSourceId = dataSource.DataSourceId;

            concept.State = Entities.Entity.EntityState.Added;

            _conceptRepository.Concepts.Add(concept);

            _conceptRepository.Save();

            record = _databaseTestHelper.Single(table: "provider.Concept", target: new { ConceptID = concept.ConceptId.ToString() });

            Assert.That(record, Is.Not.Null);
        }

        //[Test]
        //public void CanSaveNewConceptWithChildObjects()
        //{
        //    Concept concept = new Concept();
        //    Provider provider = _providerLookUp.GetProvider("NZFLORA");
        //    DataSource dataSource = provider.DataSources.Where(o => o.Code == "NZFLORA").SingleOrDefault();

        //    concept.ConceptId = Guid.NewGuid();

        //    Assert.That(_databaseTestHelper.GetRecordValues("provider.Concept", "ConceptID", concept.ConceptId.ToString()), Is.Null);
        //    Assert.That(_databaseTestHelper.GetRecordValues("provider.ConceptRelationship", "FromConceptID", concept.ConceptId.ToString()), Is.Null);
        //    Assert.That(_databaseTestHelper.GetRecordValues("provider.ConceptApplication", "ConceptID", concept.ConceptId.ToString()), Is.Null);

        //    concept.NameId = new Guid("4DDCF563-6D19-4926-9911-88EEB6B841EE");
        //    concept.DataSourceId = dataSource.DataSourceId;

        //    ConceptRelationship conceptRelationship = new ConceptRelationship();

        //    conceptRelationship.FromConceptId = concept.ConceptId;
        //    conceptRelationship.ConceptRelationshipTypeId = _conceptRelationshipTypeLookUp.GetConceptRelationshipType("is child of").ConceptRelationshipTypeId;

        //    concept.ConceptRelationships.Add(conceptRelationship);

        //    ConceptApplication conceptApplication = new ConceptApplication();

        //    conceptApplication.FromConceptId = concept.ConceptId;
        //    conceptApplication.ConceptApplicationTypeId = _conceptApplicationTypeLookUp.GetConceptApplicationType("is vernacular for").ConceptApplicationTypeId;
        //    conceptApplication.Gender = "male";
        //    conceptApplication.GeoRegion = "Opotiki";

        //    concept.ConceptApplications.Add(conceptApplication);

        //    concept.State = Entities.Entity.EntityState.Added;

        //    _conceptRepository.Concepts.Add(concept);

        //    _conceptRepository.Save();

        //    Assert.That(_databaseTestHelper.GetRecordValues("provider.Concept", "ConceptID", concept.ConceptId.ToString()), Is.Not.Null);
        //    Assert.That(_databaseTestHelper.GetRecordValues("provider.ConceptRelationship", "FromConceptID", concept.ConceptId.ToString()), Is.Not.Null);
        //    Assert.That(_databaseTestHelper.GetRecordValues("provider.ConceptApplication", "ConceptID", concept.ConceptId.ToString()), Is.Not.Null);
        //}

        //[Test]
        //public void CanUpdateExistingConcept()
        //{
        //    _databaseTestHelper.ExecuteSqlScript(DatabaseTestHelper.SqlScript.ResetDatabase);
        //    _databaseTestHelper.ExecuteSqlScript(DatabaseTestHelper.SqlScript.InsertProviderData);
        //    _databaseTestHelper.ExecuteSqlScript(DatabaseTestHelper.SqlScript.InsertBaseProviderNameData);
        //    _databaseTestHelper.ExecuteSqlScript(DatabaseTestHelper.SqlScript.InsertBaseProviderReferenceData);
        //    _databaseTestHelper.ExecuteSqlScript(DatabaseTestHelper.SqlScript.InsertBaseProviderConceptData);

        //    Concept concept = new Concept();
        //    Provider provider = _providerLookUp.GetProvider("NZFLORA");
        //    DataSource dataSource = provider.DataSources.Where(o => o.Code == "NZFLORA").SingleOrDefault();

        //    concept.ConceptId = new Guid("EF038BB2-C61F-43EB-B811-AB88FBB169FC");
        //    concept.NameId = new Guid("4DDCF563-6D19-4926-9911-88EEB6B841EE");
        //    concept.DataSourceId = dataSource.DataSourceId;

        //    Dictionary<String, Object> databaseValues = _databaseTestHelper.GetRecordValues("provider.Concept", "ConceptID", concept.ConceptId.ToString());

        //    Assert.That(databaseValues, Is.Not.Null);
        //    Assert.That(databaseValues["Orthography"], Is.EqualTo("Orthography"));

        //    concept.Orthography = "Updated";

        //    concept.State = Entities.Entity.EntityState.Modified;

        //    _conceptRepository.Concepts.Add(concept);

        //    _conceptRepository.Save();

        //    databaseValues = _databaseTestHelper.GetRecordValues("provider.Concept", "ConceptID", concept.ConceptId.ToString());

        //    Assert.That(databaseValues["Orthography"], Is.EqualTo("Updated"));
        //}

        //[Test]
        //public void CanLookUpExistingConcepts()
        //{
        //    _databaseTestHelper.ExecuteSqlScript(DatabaseTestHelper.SqlScript.ResetDatabase);
        //    _databaseTestHelper.ExecuteSqlScript(DatabaseTestHelper.SqlScript.InsertProviderData);
        //    _databaseTestHelper.ExecuteSqlScript(DatabaseTestHelper.SqlScript.InsertBaseProviderNameData);
        //    _databaseTestHelper.ExecuteSqlScript(DatabaseTestHelper.SqlScript.InsertBaseProviderReferenceData);
        //    _databaseTestHelper.ExecuteSqlScript(DatabaseTestHelper.SqlScript.InsertBaseProviderConceptData);

        //    Assert.That(_conceptRepository.GetConcepts(new Guid("F6235951-CA30-4449-87F3-9159BEEBFB24")).Count, Is.EqualTo(1));
        //}
    }
}
