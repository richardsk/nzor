using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NZOR.Data.Test;
using System.Configuration;
using System.Xml.Linq;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Repositories.Provider;
using NZOR.Data.Repositories.Common;
using NZOR.Data.Entities.Provider;
using NZOR.Data.Sql.Repositories.Provider;
using NZOR.Data.Sql.Repositories.Common;
using Database.Test.Helper;
using NZOR.Admin.Data.Repositories;
using NZOR.Admin.Data.Sql.Repositories;

namespace NZOR.Harvest.Test.UnitTests.Database
{
    [TestFixture]
    public class ConceptTest
    {
        private INameRepository _nameRepository;
        private IReferenceRepository _referenceRepository;
        private IConceptRepository _conceptRepository;
        private ILookUpRepository _lookUpRepository;
        private ITaxonPropertyRepository _taxonPropRepository;
        private IAnnotationRepository _annotationRepository;
        private IProviderRepository _provRepository;

        private ConceptRelationshipTypeLookUp _conceptRelationshipTypeLookUp;

        private const String BasicValidTaxonConcept = @"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-Col2010-TaxonConcept.xml";

        private DatabaseTestHelper _databaseTestHelper;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            _nameRepository = new NameRepository(connectionString);
            _referenceRepository = new ReferenceRepository(connectionString);
            _conceptRepository = new ConceptRepository(connectionString);
            _lookUpRepository = new LookUpRepository(connectionString);
            _taxonPropRepository = new TaxonPropertyRepository(connectionString);
            _annotationRepository = new AnnotationRepository(connectionString);
            _provRepository = new ProviderRepository(connectionString);

            _conceptRelationshipTypeLookUp = new ConceptRelationshipTypeLookUp(_lookUpRepository.GetConceptRelationshipTypes());

            _databaseTestHelper = new DatabaseTestHelper(connectionString);

            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.ResetProviderConcept));
        }

        [Test]
        public void CanSaveConcept()
        {
            TestFixtureSetUp();

            XDocument document = XDocument.Load(BasicValidTaxonConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Concept testConcept = importer.DataSources("Col2010").Concepts.Values.SingleOrDefault(o => o.ProviderRecordId == "Test Taxon Concept Id 1");

            Assert.That(_databaseTestHelper.Single(table: "provider.Concept", target: new { ConceptID = testConcept.ConceptId.ToString() }), Is.Empty);

            importer.Save();

            Assert.That(_databaseTestHelper.Single(table: "provider.Concept", target: new { ConceptID = testConcept.ConceptId.ToString() }), Is.Not.Null);
        }
    }
}
