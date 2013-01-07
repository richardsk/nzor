using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NZOR.Data.Entities.Common;
using NZOR.Data.Entities.Provider;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Data.Sql.Repositories.Provider;
using NZOR.Data.Repositories.Common;
using NZOR.Data.Repositories.Provider;
using Database.Test.Helper;
using NZOR.Admin.Data.Lookups;
using NZOR.Admin.Data.Repositories;
using NZOR.Admin.Data.Sql.Repositories;
using NZOR.Admin.Data.Entities;

namespace NZOR.Data.Test.UnitTests
{
    [TestFixture]
    public class ReferenceRepositoryTest
    {
        private IReferenceRepository _referenceRepository;
        private ILookUpRepository _lookUpRepository;
        private IProviderRepository _provRepository;
 
        private ProviderLookUp _providerLookUp;
        private ReferenceTypeLookUp _referenceTypeLookUp;
        private ReferencePropertyTypeLookUp _referencePropertyTypeLookUp;

        private DatabaseTestHelper _databaseTestHelper;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            _referenceRepository = new ReferenceRepository(connectionString);
            _lookUpRepository = new LookUpRepository(connectionString);

            _provRepository = new ProviderRepository(connectionString);
            _providerLookUp = new ProviderLookUp(_provRepository.GetProviders());

            _referenceTypeLookUp = new ReferenceTypeLookUp(_lookUpRepository.GetReferenceTypes());
            _referencePropertyTypeLookUp = new ReferencePropertyTypeLookUp(_lookUpRepository.GetReferencePropertyTypes());

            _databaseTestHelper = new DatabaseTestHelper(connectionString);
        }

        [SetUp]
        public void TestSetUp()
        {
            _referenceRepository.References.Clear();
        }

        [Test]
        public void CanGetDataSourceReference()
        {
            Reference pr = _referenceRepository.GetDataSourceReference(new Guid("2DD748D7-0CF4-4A74-8E01-3464F688603B"), new DateTime(2010, 11, 3));
            if (pr == null)
            {
                pr = _referenceRepository.CreateDataSourceReference(new Guid("2DD748D7-0CF4-4A74-8E01-3464F688603B"), new DateTime(2010, 11, 3));
            }
        }

        [Test]
        public void CanSaveNewReference()
        {
            Reference reference = new Reference();
            Provider provider = _providerLookUp.GetProvider("NZFLORA");
            DataSource dataSource = provider.DataSources.Where(o => o.Code == "NZFLORA").SingleOrDefault();

            reference.ReferenceId = Guid.NewGuid();

            Assert.That(_databaseTestHelper.Single(table: "provider.Reference", target: new { ReferenceID = reference.ReferenceId.ToString() }), Is.Null);

            reference.ReferenceTypeId = _referenceTypeLookUp.GetReferenceType("Book").ReferenceTypeId;
            reference.DataSourceId = dataSource.DataSourceId;

            reference.ProviderRecordId = Guid.NewGuid().ToString();

            ReferenceProperty referenceProperty = new ReferenceProperty();

            referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType("Citation").ReferencePropertyTypeId;

            referenceProperty.Value = "Test Value";

            reference.ReferenceProperties.Add(referenceProperty);

            reference.State = Entities.Entity.EntityState.Added;

            _referenceRepository.References.Add(reference);

            _referenceRepository.Save();

            Assert.That(_databaseTestHelper.Single(table: "provider.Reference", target: new { ReferenceID = reference.ReferenceId.ToString() }), Is.Not.Null);
            Assert.That(_databaseTestHelper.Single(table: "provider.ReferenceProperty", target: new { ReferenceID = reference.ReferenceId.ToString() }), Is.Not.Null);
        }

        [Test]
        public void CanUpdateExistingReference()
        {
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.ResetDatabase));
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertProviderData));
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertBaseProviderReferenceData));

            Reference reference = new Reference();
            Provider provider = _providerLookUp.GetProvider("NZFLORA");
            DataSource dataSource = provider.DataSources.Where(o => o.Code == "NZFLORA").SingleOrDefault();

            reference.ReferenceId = new Guid("DB972AB0-4806-4F45-91A5-5B89FD994E25");

            Dictionary<String, Object> databaseValues = _databaseTestHelper.Single(table: "provider.Reference", target: new { ReferenceID = reference.ReferenceId.ToString() });

            Assert.That(databaseValues, Is.Not.Null);
            Assert.That(databaseValues["ProviderModifiedDate"], Is.Null);

            reference.ReferenceTypeId = _referenceTypeLookUp.GetReferenceType("Book").ReferenceTypeId;
            reference.DataSourceId = dataSource.DataSourceId;

            reference.ProviderModifiedDate = new DateTime(2010, 11, 3);

            reference.State = Entities.Entity.EntityState.Modified;

            _referenceRepository.References.Add(reference);

            _referenceRepository.Save();

            databaseValues = _databaseTestHelper.Single(table: "provider.Reference", target: new { ReferenceID = reference.ReferenceId.ToString() });

            Assert.That(databaseValues["ProviderModifiedDate"], Is.EqualTo(new DateTime(2010, 11, 3)));
        }
    }
}
