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
    public class TaxonPropertyTest
    {
        private INameRepository _nameRepository;
        private IReferenceRepository _referenceRepository;
        private IConceptRepository _conceptRepository;
        private ILookUpRepository _lookUpRepository;
        private ITaxonPropertyRepository _taxonPropRepository;
        private IAnnotationRepository _annotationRepository;
        private IProviderRepository _provRepository;

        private ConceptRelationshipTypeLookUp _conceptRelationshipTypeLookUp;

        private const String BasicValidTaxonConcept = @"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-Col2010-TaxonProperty.xml";

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
        }

        [Test]
        public void CanLoadDataSourceFromProviderXml()
        {
            XDocument document = XDocument.Load(BasicValidTaxonConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Assert.That(importer.DataSources("Col2010"), Is.Not.Null);
            Assert.That(importer.DataSources("Col2010").Concepts.Count, Is.GreaterThan(0));
        }

        [Test]
        public void CanCreateTaxonPropertyFromProviderXml()
        {
            TaxonProperty tp = GetTestTaxonProperty("Test Taxon Property Id 1");

            Assert.That(tp, Is.Not.Null);
        }

        [Test]
        public void CanLoadAttributeValuesFromProviderXml()
        {
            TaxonProperty tp = GetTestTaxonProperty("Test Taxon Property Id 1");

            Assert.That(tp.ProviderCreatedDate, Is.EqualTo(new DateTime(2002, 12, 3, 14, 44, 49))); 
            Assert.That(tp.ProviderModifiedDate, Is.Null);
        }

        [Test]
        public void CanLoadNameValueFromProviderXml()
        {
            TaxonProperty tp = GetTestTaxonProperty("Test Taxon Property Id 1");

            Assert.That(tp.ProviderNameId, Is.EqualTo("33937EA5-ED2A-4F2F-9881-EE0688C3F901"));
        }

        [Test]
        public void CanLinkNameValue()
        {
            XDocument document = XDocument.Load(BasicValidTaxonConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            TaxonProperty tp = importer.DataSources("Col2010").TaxonProperties.Values.SingleOrDefault(o => o.ProviderRecordId == "Test Taxon Property Id 1");
            Name referencedName = importer.DataSources("Col2010").Names.Values.SingleOrDefault(o => o.ProviderRecordId == tp.ProviderNameId);

            Assert.That(tp, Is.Not.Null);
            Assert.That(referencedName, Is.Not.Null);

            Assert.That(tp.NameId, Is.EqualTo(referencedName.NameId));
        }

        [Test]
        public void CanLoadAccordingToValueFromProviderXml()
        {
            TaxonProperty tp = GetTestTaxonProperty("Test Taxon Property Id 1");

            Assert.That(tp.ProviderReferenceId, Is.EqualTo("B51A9FF8-7C6C-4302-BE5A-9033F35CE8E4"));
        }

        [Test]
        public void CanLinkAccordingToValue()
        {
            XDocument document = XDocument.Load(BasicValidTaxonConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            TaxonProperty tp = importer.DataSources("Col2010").TaxonProperties.Values.SingleOrDefault(o => o.ProviderRecordId == "Test Taxon Property Id 1");
            Reference referencedReference = importer.DataSources("Col2010").References.Values.SingleOrDefault(o => o.ProviderRecordId == tp.ProviderReferenceId);

            Assert.That(tp, Is.Not.Null);
            Assert.That(referencedReference, Is.Not.Null);

            Assert.That(tp.ReferenceId, Is.EqualTo(referencedReference.ReferenceId));
        }
        
        private TaxonProperty GetTestTaxonProperty(String providerRecordId)
        {
            XDocument document = XDocument.Load(BasicValidTaxonConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            return importer.DataSources("Col2010").TaxonProperties.Values.SingleOrDefault(o => o.ProviderRecordId == providerRecordId);
        }
    }
}
