using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using NZOR.Admin.Data.Entities;
using NZOR.Data.Entities.Common;
using NZOR.Data.Entities.Provider;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Repositories.Common;
using NZOR.Data.Repositories.Provider;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Data.Sql.Repositories.Provider;
using Database.Test.Helper;
using NZOR.Admin.Data.Repositories;
using NZOR.Admin.Data.Sql.Repositories;

namespace NZOR.Data.Test.UnitTests
{
    [TestFixture]
    public class NameRepositoryTest
    {
        private INameRepository _nameRepository;
        private ILookUpRepository _lookUpRepository;
        private IProviderRepository _provRepository;

        private NZOR.Data.Repositories.Consensus.INameRepository _consensusNameRepository;

        private TaxonRankLookUp _taxonRankLookUp;
        private NameClassLookUp _nameClassLookUp;        
        private NamePropertyTypeLookUp _namePropertyTypeLookUp;

        private DatabaseTestHelper _databaseTestHelper;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            _nameRepository = new NameRepository(connectionString);
            _lookUpRepository = new LookUpRepository(connectionString);
            _provRepository = new ProviderRepository(connectionString);

            _consensusNameRepository = new NZOR.Data.Sql.Repositories.Consensus.NameRepository(connectionString);

            _taxonRankLookUp = new TaxonRankLookUp(_lookUpRepository.GetTaxonRanks());
            _nameClassLookUp = new NameClassLookUp(_lookUpRepository.GetNameClasses());
            _namePropertyTypeLookUp = new NamePropertyTypeLookUp(_lookUpRepository.GetNamePropertyTypes());

            _databaseTestHelper = new DatabaseTestHelper(connectionString);
        }

        [SetUp]
        public void TestSetUp()
        {
            _nameRepository.Names.Clear();
        }

        [Test]
        public void CanSaveNewName()
        {
            TestFixtureSetUp();

            Name name = new Name();
            Provider provider = _provRepository.GetProviderByCode("NZFLORA");
            DataSource dataSource = provider.DataSources.Where(o => o.Code == "NZFLORA").SingleOrDefault();

            name.NameId = Guid.NewGuid();

            Assert.That(_databaseTestHelper.Single(table: "provider.Name", target: new { NameID = name.NameId.ToString() }), Is.Empty);

            name.TaxonRankId = _taxonRankLookUp.GetTaxonRank("species", "ICBN").TaxonRankId;
            name.NameClassId = _nameClassLookUp.GetNameClass("Scientific Name").NameClassId;
            name.DataSourceId = dataSource.DataSourceId;

            name.FullName = "New Name";
            name.ProviderRecordId = Guid.NewGuid().ToString();

            NamePropertyType rankNamePropertyType = _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, "Rank");
            name.NameProperties.Add(new NameProperty() { NamePropertyTypeId = rankNamePropertyType.NamePropertyTypeId, Value = "species" });

            name.State = Entities.Entity.EntityState.Added;

            _nameRepository.Names.Add(name);

            _nameRepository.Save();

            Assert.That(_databaseTestHelper.Single(table: "provider.Name", target: new { NameID = name.NameId.ToString() }), Is.Not.Null);
            Assert.That(_databaseTestHelper.Single(table: "provider.NameProperty", target: new { NameID = name.NameId.ToString() }), Is.Not.Null);
        }

        [Test]
        public void CanSaveNewConsensusName()
        {
            TestFixtureSetUp();

            Entities.Consensus.Name name = new Entities.Consensus.Name();

            name.NameId = Guid.NewGuid();

            Assert.That(_databaseTestHelper.Single(table: "consensus.Name", target: new { NameID = name.NameId.ToString() }), Is.Empty);

            name.TaxonRankId = _taxonRankLookUp.GetTaxonRank("species", "ICBN").TaxonRankId;
            name.NameClassId = _nameClassLookUp.GetNameClass("Scientific Name").NameClassId;

            name.FullName = "New Name";

            NamePropertyType rankNamePropertyType = _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, "Rank");
            name.NameProperties.Add(new Entities.Consensus.NameProperty() { NamePropertyTypeId = rankNamePropertyType.NamePropertyTypeId, Value = "species" });

            name.State = Entities.Entity.EntityState.Added;

            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            Sql.Repositories.Consensus.NameRepository nr = new Sql.Repositories.Consensus.NameRepository(connectionString);

            nr.Names.Add(name);

            nr.Save();

            Assert.That(_databaseTestHelper.Single(table: "consensus.Name", target: new { NameID = name.NameId.ToString() }), Is.Not.Null);
            Assert.That(_databaseTestHelper.Single(table: "consensus.NameProperty", target: new { NameID = name.NameId.ToString() }), Is.Not.Null);
        }

        [Test]
        public void CanUpdateExistingName()
        {
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.ResetDatabase));
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertProviderData));
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertBaseProviderNameData));

            Name name = new Name();
            Provider provider = _provRepository.GetProviderByCode("NZFLORA");
            DataSource dataSource = provider.DataSources.Where(o => o.Code == "NZFLORA").SingleOrDefault();

            name.NameId = new Guid("D3362FD9-0D7D-4DF4-A762-7D3560A20803");
            name.TaxonRankId = _taxonRankLookUp.GetTaxonRank("species", "ICBN").TaxonRankId;
            name.NameClassId = _nameClassLookUp.GetNameClass("Scientific Name").NameClassId;
            name.DataSourceId = dataSource.DataSourceId;

            Dictionary<String, Object> databaseValues = _databaseTestHelper.Single(table: "provider.Name", target: new { NameID = name.NameId.ToString() });

            Assert.That(databaseValues, Is.Not.Null);
            Assert.That(databaseValues["FullName"], Is.EqualTo("Test Existing Provider Name 1"));

            name.FullName = "Updated";

            name.State = Entities.Entity.EntityState.Modified;

            _nameRepository.Names.Add(name);

            _nameRepository.Save();

            databaseValues = _databaseTestHelper.Single(table: "provider.Name", target: new { NameID = name.NameId.ToString() });

            Assert.That(databaseValues["FullName"], Is.EqualTo("Updated"));
        }

        [Test]
        [ExpectedException(typeof(System.Data.SqlClient.SqlException))]
        public void CanNotInsertMultipleNamesWithSameProviderRecordId()
        {
            TestFixtureSetUp();

            Name name1 = new Name();
            Name name2 = new Name();
            Provider provider = _provRepository.GetProviderByCode("NZFLORA");
            DataSource dataSource = provider.DataSources.Where(o => o.Code == "NZFLORA").SingleOrDefault();

            name1.NameId = Guid.NewGuid();
            name1.TaxonRankId = _taxonRankLookUp.GetTaxonRank("species", "ICBN").TaxonRankId;
            name1.NameClassId = _nameClassLookUp.GetNameClass("Scientific Name").NameClassId;
            name1.DataSourceId = dataSource.DataSourceId;

            name1.FullName = "New Name";
            name1.ProviderRecordId = "The Same";

            name2.NameId = Guid.NewGuid();
            name2.TaxonRankId = _taxonRankLookUp.GetTaxonRank("species", "ICBN").TaxonRankId;
            name2.NameClassId = _nameClassLookUp.GetNameClass("Scientific Name").NameClassId;
            name2.DataSourceId = dataSource.DataSourceId;
            
            name2.FullName = "New Name";
            name2.ProviderRecordId = "The Same";

            name1.State = Entities.Entity.EntityState.Added;
            name2.State = Entities.Entity.EntityState.Added;

            _nameRepository.Names.Add(name1);
            _nameRepository.Names.Add(name2);

            _nameRepository.Save();
        }

        [Test]
        public void CanLookUpExistingNames()
        {
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.ResetDatabase));
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertProviderData));
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertBaseProviderNameData));

            Assert.That(_nameRepository.GetNames(new Guid("F6235951-CA30-4449-87F3-9159BEEBFB24")).Count, Is.EqualTo(3));
        }

        [Test]
        public void CanSearchConsensusNames()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            List<SearchField> fields = new List<SearchField>();
            SearchField f = new SearchField();
            f.SearchColumn = SearchField.SearchFieldColumn.FullName;
            f.SearchText = "pla";
            fields.Add(f);

            f = new SearchField();
            f.SearchColumn = SearchField.SearchFieldColumn.Authors;
            f.SearchText = "L.";

            int total = 0;
            List<NZOR.Data.Entities.Consensus.NameProfile> names = _consensusNameRepository.SearchNames(fields, 5, 50, SearchField.OrderByField.FullName, out total);

        }
        
        [Test()]
        public void CanGetNameProfile()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            NZOR.Data.Sql.Repositories.Consensus.NameRepository nr = new Sql.Repositories.Consensus.NameRepository(connectionString);
            NZOR.Data.Entities.Consensus.NameProfile np = nr.GetNameProfile(new Guid("E14648A8-8D57-4CC1-B50B-0007A947B52B"));

            np = nr.GetNameProfile(new Guid("84e64a9b-6df7-4766-8288-9d8053b2c266"));
        }

        [Test()]
        public void CanRefreshAllNames()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            List<String> errMsgs = NZOR.Data.Sql.Integration.RefreshAllNames(connectionString);
                        
        }

        [Test()]
        public void CanRefreshName()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            
            NZOR.Data.Sql.Integration.RefreshConsensusName(new Guid("0E8FD921-4A9A-4872-B1ED-8760A7E8A1E4"), connectionString, GetAttachmentPoints(connectionString));
        }

        private List<Admin.Data.Entities.AttachmentPoint> GetAttachmentPoints(string cnnStr)
        {
            Admin.Data.Sql.Repositories.ProviderRepository pRep = new Admin.Data.Sql.Repositories.ProviderRepository(cnnStr);

            List<Admin.Data.Entities.AttachmentPoint> attPoints = pRep.GetAllAttachmentPoints();

            return attPoints;
        }

        [Test()]
        public void CanRefreshNameFullNames()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            NZOR.Data.Sql.Integration.UpdateConsensusFullNameValues(connectionString);
        }

        [Test()]
        public void CanRefreshNameFullName()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("c6822bda-87ef-4dd6-9f90-283e35d46444"));
            NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("A451A5B9-1A6B-4CD4-A34E-6692D997A827"));
            
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("F38E12BF-0BE7-4F13-B739-E2BC1B763AE0"));
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("2DA6A6D3-AE5D-4C1C-A6D9-831CAD07D61F"));
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("E3F0DA4B-CCD9-43E5-A4C6-764EA062D4C5"));
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("44759A5D-F63C-4B22-87D2-A1139A16A689"));
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("F5E02815-A1B5-45AB-9A46-F507F185FE66"));
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("CED9FA07-3C08-4184-A999-7F067630B30F"));
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("91F11659-AD2F-4903-89A0-286D3953327D"));
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("052C7F74-CC88-4A33-AB6E-38C01F872D48"));
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("339A4EC3-B3AB-4204-B440-44CD48A0E3AC"));
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("7FEBA46C-2709-4092-9AFC-55CB70BA82D1"));
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("54A48E47-A7EC-4E56-A3BC-8F883F4CC7EC"));
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("897C357F-7137-4384-A403-1D651DC00B42"));
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("E72304F5-C8FB-4CCD-BAD9-30B33B548C37"));
            //NZOR.Data.Sql.Integration.UpdateConsensusFullName(connectionString, new Guid("5B69F069-A3D9-4933-9F7C-CB62313A1C6C"));
                      
          
        }
    }
}
