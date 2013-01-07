using System;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using NZOR.Data.Entities.Provider;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Repositories.Common;
using NZOR.Data.Repositories.Provider;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Data.Sql.Repositories.Provider;
using NZOR.Admin.Data.Repositories;
using NZOR.Admin.Data.Sql.Repositories;

namespace NZOR.Harvest.Test.UnitTests.ProviderXml
{
    [TestFixture]
    public class TaxonNameTest
    {
        private INameRepository _nameRepository;
        private IReferenceRepository _referenceRepository;
        private IConceptRepository _conceptRepository;
        private ILookUpRepository _lookUpRepository;
        private ITaxonPropertyRepository _taxonPropRepository;
        private IAnnotationRepository _annotationRepository;
        private IProviderRepository _provRepository;

        private TaxonRankLookUp _taxonRankLookUp;
        private NameClassLookUp _nameClassLookUp;
        private NamePropertyTypeLookUp _namePropertyTypeLookUp;

        private const String BasicValidTaxonName = @"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider XML Data\NZOR-Test-Col2010-TaxonName.xml";

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
            
            _taxonRankLookUp = new TaxonRankLookUp(_lookUpRepository.GetTaxonRanks());
            _nameClassLookUp = new NameClassLookUp(_lookUpRepository.GetNameClasses());
            _namePropertyTypeLookUp = new NamePropertyTypeLookUp(_lookUpRepository.GetNamePropertyTypes());
        }

        [Test]
        public void CanLoadDataSourceFromProviderXml()
        {
            XDocument document = XDocument.Load(BasicValidTaxonName);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Assert.That(importer.DataSources("Col2010"), Is.Not.Null);
            Assert.That(importer.DataSources("Col2010").Names.Count, Is.GreaterThan(0));
        }

        [Test]
        public void CanCreateTaxonNameValuesFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName, Is.Not.Null);            
        }

        [Test]
        public void CanLoadAttributeValuesFromProviderXml()
        {
            TestFixtureSetUp();

            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.QualityCode).NamePropertyTypeId).Value,
                Is.EqualTo("Validated Secondary Source"));
            Assert.That(testName.ProviderCreatedDate, Is.EqualTo(new DateTime(2010, 10, 5, 10, 1, 25)));
            Assert.That(testName.ProviderModifiedDate, Is.EqualTo(new DateTime(2010, 10, 6, 10, 11, 25)));
         
        }

        [Test]
        public void CanLoadFullNameValueFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.FullName, Is.EqualTo("Hippidae"));
        }

        [Test]
        public void CanLoadRankValueFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.TaxonRankId, Is.EqualTo(_taxonRankLookUp.GetTaxonRank("Family", "ICBN").TaxonRankId));
            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.Rank).NamePropertyTypeId).Value,
                Is.EqualTo("Family"));
        }

        [Test]
        public void CanLoadIsRecombinationFromProviderXml()
        {
            TestFixtureSetUp();

            //is recomb element set
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");
            Assert.That(testName.IsRecombination, Is.EqualTo(true));

            //is recomb because of bas authors
            testName = GetTestName("49FF36C1-9D32-4F23-B017-54A880507F65");
            Assert.That(testName.IsRecombination, Is.EqualTo(true));
            
            //is recomb, dont remove author brackets (ICBN)
            testName = GetTestName("EBB47621-3E09-4DD9-86AF-4549E24FA3AD");
            Assert.That(testName.IsRecombination, Is.EqualTo(true));
            Assert.That(testName.GetNameProperty(NamePropertyTypeLookUp.Authors).Value.Contains("("));

            //is recomb, do remove author brackets (ICZN)
            testName = GetTestName("F72A51BA-9F80-489E-8324-52FAE83D250A");
            Assert.That(testName.IsRecombination, Is.EqualTo(true));
            Assert.That(!testName.GetNameProperty(NamePropertyTypeLookUp.Authors).Value.Contains("("));
            
        }

        [Test]
        public void CanLoadCanonicalNameValuesFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.Canonical).NamePropertyTypeId).Value,
                Is.EqualTo("Test Simple"));
        }

        [Test]
        public void CanLoadAuthorshipValueFromProviderXml()
        {
            TestFixtureSetUp();
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.Authors).NamePropertyTypeId).Value,
                Is.EqualTo("Test Authorship"));
        }
                
        [Test]
        public void CanLoadBasionymAuthorsValueFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.BasionymAuthors).NamePropertyTypeId).Value,
                Is.EqualTo("Test BasionymAuthors"));
        }

        [Test]
        public void CanLoadCombiningAuthorsValueFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.CombinationAuthors).NamePropertyTypeId).Value,
                Is.Empty);
        }

        [Test]
        public void CanLoadPublishedInValuesFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.PublishedIn).NamePropertyTypeId).Value,
                Is.EqualTo("Test PublishedIn"));
            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.PublishedIn).NamePropertyTypeId).ProviderRelatedId,
                Is.EqualTo("Test PublishedIn ref"));
        }

        [Test]
        public void CanLoadYearValueFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.Year).NamePropertyTypeId).Value,
                Is.EqualTo("Test Year"));
        }

        [Test]
        public void CanLoadMicroReferenceValueFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.MicroReference).NamePropertyTypeId).Value,
                Is.EqualTo("Test MicroReference"));
        }

        [Test]
        public void CanLoadTypeNameValuesFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.TypeName).NamePropertyTypeId).Value,
                Is.EqualTo("Test TypeName"));
            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.TypeName).NamePropertyTypeId).ProviderRelatedId,
                Is.EqualTo("Test TypeName ref"));
        }

        [Test]
        public void CanLoadOrthographyValueFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.Orthography).NamePropertyTypeId).Value,
                Is.EqualTo("Test Orthography"));
        }

        [Test]
        public void CanLoadBasionymValuesFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.Basionym).NamePropertyTypeId).Value,
                Is.EqualTo("Test Basionym"));
            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.Basionym).NamePropertyTypeId).ProviderRelatedId,
                Is.EqualTo("Test Basionym ref"));
        }

        [Test]
        public void CanLoadLaterHomonymValuesFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.LaterHomonymOf).NamePropertyTypeId).Value,
                Is.EqualTo("Test LaterHomonymOf"));
            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.LaterHomonymOf).NamePropertyTypeId).ProviderRelatedId,
                Is.EqualTo("Test LaterHomonymOf ref"));
        }

        [Test]
        public void CanLoadReplacementForValuesFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.BlockedName).NamePropertyTypeId).Value,
                Is.EqualTo("Test BlockedName"));
            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.BlockedName).NamePropertyTypeId).ProviderRelatedId,
                Is.EqualTo("Test BlockedName ref"));

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.RecombinedName).NamePropertyTypeId).Value,
                Is.EqualTo("Test RecombinedName"));
            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.RecombinedName).NamePropertyTypeId).ProviderRelatedId,
                Is.EqualTo("Test RecombinedName ref"));
        }

        [Test]
        public void CanLoadNomenclaturalStatusValueFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.NomenclaturalStatus).NamePropertyTypeId).Value,
                Is.EqualTo("Test NomenclaturalStatus"));
        }

        [Test]
        public void CanLoadNomenclaturalCodeValueFromProviderXml()
        {
            Name testName = GetTestName("33937EA5-ED2A-4F2F-9881-EE0688C3F901");

            Assert.That(testName.GoverningCode, Is.EqualTo("Test"));
        }

        private Name GetTestName(String providerRecordId)
        {
            XDocument document = XDocument.Load(BasicValidTaxonName);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            return importer.DataSources("NZOR_Test").Names.Values.SingleOrDefault(o => o.ProviderRecordId.Equals(providerRecordId, StringComparison.OrdinalIgnoreCase));
        }
    }
}
