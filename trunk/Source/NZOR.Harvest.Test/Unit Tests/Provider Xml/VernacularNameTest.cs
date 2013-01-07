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
    public class VernacularNameTest
    {
        private INameRepository _nameRepository;
        private IReferenceRepository _referenceRepository;
        private IConceptRepository _conceptRepository;
        private ILookUpRepository _lookUpRepository;
        private ITaxonPropertyRepository _taxonPropRepository;
        private IAnnotationRepository _annRepository;
        private IProviderRepository _provRepository;

        private NameClassLookUp _nameClassLookUp;
        private NamePropertyTypeLookUp _namePropertyTypeLookUp;

        private const String BasicValidTaxonName = @"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider XML Data\NZOR-Test-Col2010-VernacularName.xml";

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            _nameRepository = new NameRepository(connectionString);
            _referenceRepository = new ReferenceRepository(connectionString);
            _conceptRepository = new ConceptRepository(connectionString);
            _lookUpRepository = new LookUpRepository(connectionString);
            _taxonPropRepository = new TaxonPropertyRepository(connectionString);
            _annRepository = new AnnotationRepository(connectionString);
            _provRepository = new ProviderRepository(connectionString);

            _nameClassLookUp = new NameClassLookUp(_lookUpRepository.GetNameClasses());
            _namePropertyTypeLookUp = new NamePropertyTypeLookUp(_lookUpRepository.GetNamePropertyTypes());
        }

        [Test]
        public void CanLoadDataSourceFromProviderXml()
        {
            XDocument document = XDocument.Load(BasicValidTaxonName);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Assert.That(importer.DataSources("Col2010"), Is.Not.Null);
            Assert.That(importer.DataSources("Col2010").Names.Count, Is.GreaterThan(0));
        }

        [Test]
        public void CanLoadAttributeValuesFromProviderXml()
        {
            Name testName = GetTestName("Test Vernacular ID");

            Assert.That(testName.ProviderCreatedDate, Is.EqualTo(new DateTime(2010, 10, 1, 10, 1, 25)));
            Assert.That(testName.ProviderModifiedDate, Is.EqualTo(new DateTime(2010, 10, 2, 10, 11, 25)));
        }

        [Test]
        public void CanLoadFullNameValueFromProviderXml()
        {
            Name testName = GetTestName("Test Vernacular ID");

            Assert.That(testName.FullName, Is.EqualTo("Test Vernacular Full Name"));
        }

        [Test]
        public void CanLoadPublishedInValuesFromProviderXml()
        {
            Name testName = GetTestName("Test Vernacular ID");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.PublishedIn).NamePropertyTypeId).Value,
                Is.EqualTo("Test PublishedIn"));
            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.PublishedIn).NamePropertyTypeId).ProviderRelatedId,
                Is.EqualTo("Test PublishedIn ref"));
        }

        [Test]
        public void CanLoadLanguageValueFromProviderXml()
        {
            Name testName = GetTestName("Test Vernacular ID");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.Language).NamePropertyTypeId).Value,
                Is.EqualTo("Test Language"));
        }

        [Test]
        public void CanLoadCountryValueFromProviderXml()
        {
            Name testName = GetTestName("Test Vernacular ID");

            Assert.That(testName.NameProperties.Single(
                o => o.NamePropertyTypeId == _namePropertyTypeLookUp.GetNamePropertyType(testName.NameClassId, NamePropertyTypeLookUp.Country).NamePropertyTypeId).Value,
                Is.EqualTo("Test Country"));
        }

        private Name GetTestName(String providerRecordId)
        {
            XDocument document = XDocument.Load(BasicValidTaxonName);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            return importer.DataSources("Col2010").Names.Values.SingleOrDefault(o => o.ProviderRecordId.Equals(providerRecordId, StringComparison.OrdinalIgnoreCase));
        }
    }
}
