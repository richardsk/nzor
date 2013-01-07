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
    public class TaxonConceptTest
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
        public void CanCreateConceptFromProviderXml()
        {
            Concept testConcept = GetTestConcept("Test Taxon Concept Id 1");

            Assert.That(testConcept, Is.Not.Null);
        }

        [Test]
        public void CanLoadAttributeValuesFromProviderXml()
        {
            Concept testConcept = GetTestConcept("Test Taxon Concept Id 1");

            Assert.That(testConcept.ProviderCreatedDate, Is.EqualTo(new DateTime(2010, 10, 6, 10, 1, 0)));
            Assert.That(testConcept.ProviderModifiedDate, Is.Null);
        }

        [Test]
        public void CanLoadNameValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("Test Taxon Concept Id 1");

            Assert.That(testConcept.ProviderNameId, Is.EqualTo("33937EA5-ED2A-4F2F-9881-EE0688C3F901"));
        }

        [Test]
        public void CanLinkNameValue()
        {
            XDocument document = XDocument.Load(BasicValidTaxonConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Concept concept = importer.DataSources("Col2010").Concepts.Values.SingleOrDefault(o => o.ProviderRecordId == "Test Taxon Concept Id 1");
            Name referencedName = importer.DataSources("Col2010").Names.Values.SingleOrDefault(o => o.ProviderRecordId == concept.ProviderNameId);

            Assert.That(concept, Is.Not.Null);
            Assert.That(referencedName, Is.Not.Null);

            Assert.That(concept.NameId, Is.EqualTo(referencedName.NameId));
        }

        [Test]
        public void CanLoadAccordingToValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("Test Taxon Concept Id 1");

            Assert.That(testConcept.ProviderReferenceId, Is.EqualTo("B51A9FF8-7C6C-4302-BE5A-9033F35CE8E4"));
        }

        [Test]
        public void CanLinkAccordingToValue()
        {
            XDocument document = XDocument.Load(BasicValidTaxonConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Concept concept = importer.DataSources("Col2010").Concepts.Values.SingleOrDefault(o => o.ProviderRecordId == "Test Taxon Concept Id 1");
            Reference referencedReference = importer.DataSources("Col2010").References.Values.SingleOrDefault(o => o.ProviderRecordId == concept.ProviderReferenceId);

            Assert.That(concept, Is.Not.Null);
            Assert.That(referencedReference, Is.Not.Null);

            Assert.That(concept.AccordingToReferenceId, Is.EqualTo(referencedReference.ReferenceId));
        }

        [Test]
        public void CanLoadRankValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("Test Taxon Concept Id 1");

            Assert.That(testConcept.TaxonRank, Is.EqualTo("Test Rank"));
        }

        [Test]
        public void CanLoadRelationshipsValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("Test Taxon Concept Id 1");

            Assert.That(testConcept.ConceptRelationships.Count, Is.EqualTo(4));
        }

        [Test]
        public void CanLoadRelationshipsAcceptedConceptValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("Test Taxon Concept Id 1");

            Assert.That(testConcept.ConceptRelationships.Single(
                o => o.ConceptRelationshipTypeId == _conceptRelationshipTypeLookUp.GetConceptRelationshipType("is synonym of").ConceptRelationshipTypeId).ProviderToRecordId,
                Is.EqualTo("Test Taxon Concept Id 3"));
            Assert.That(testConcept.ConceptRelationships.Single(
                o => o.ConceptRelationshipTypeId == _conceptRelationshipTypeLookUp.GetConceptRelationshipType("is synonym of").ConceptRelationshipTypeId).InUse,
                Is.True);
        }

        [Test]
        public void CanLoadRelationshipsParentConceptValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("Test Taxon Concept Id 1");

            Assert.That(testConcept.ConceptRelationships.Single(
                o => o.ConceptRelationshipTypeId == _conceptRelationshipTypeLookUp.GetConceptRelationshipType("is child of").ConceptRelationshipTypeId).ProviderToRecordId,
                Is.EqualTo("Test Taxon Concept Id 2"));
            Assert.That(testConcept.ConceptRelationships.Single(
                o => o.ConceptRelationshipTypeId == _conceptRelationshipTypeLookUp.GetConceptRelationshipType("is child of").ConceptRelationshipTypeId).InUse,
                Is.True);
        }

        [Test]
        public void CanLoadRelationshipsRelatedConceptValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("Test Taxon Concept Id 1");

            Assert.That(testConcept.ConceptRelationships.Single(
                o => o.ConceptRelationshipTypeId == _conceptRelationshipTypeLookUp.GetConceptRelationshipType("is anamorph of").ConceptRelationshipTypeId).ProviderToRecordId,
                Is.EqualTo("Test Taxon Concept Id 3"));
            Assert.That(testConcept.ConceptRelationships.Single(
                o => o.ConceptRelationshipTypeId == _conceptRelationshipTypeLookUp.GetConceptRelationshipType("is anamorph of").ConceptRelationshipTypeId).InUse,
                Is.False);
            Assert.That(testConcept.ConceptRelationships.Single(
                o => o.ConceptRelationshipTypeId == _conceptRelationshipTypeLookUp.GetConceptRelationshipType("is teleomorph of").ConceptRelationshipTypeId).ProviderToRecordId,
                Is.EqualTo("Test Taxon Concept Id 4"));
            Assert.That(testConcept.ConceptRelationships.Single(
                o => o.ConceptRelationshipTypeId == _conceptRelationshipTypeLookUp.GetConceptRelationshipType("is teleomorph of").ConceptRelationshipTypeId).InUse,
                Is.True);
        }

        [Test]
        public void CanLinkAccordingToRelationships()
        {
            XDocument document = XDocument.Load(BasicValidTaxonConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Concept testConcept = importer.DataSources("Col2010").Concepts.Values.SingleOrDefault(o => o.ProviderRecordId == "Test Taxon Concept Id 1");

            Assert.That(testConcept.ConceptRelationships.Single(
                o => o.ConceptRelationshipTypeId == _conceptRelationshipTypeLookUp.GetConceptRelationshipType("is child of").ConceptRelationshipTypeId).ToConceptId,
                Is.Not.EqualTo(Guid.Empty).And.Not.Null);
        }

        private Concept GetTestConcept(String providerRecordId)
        {
            XDocument document = XDocument.Load(BasicValidTaxonConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            return importer.DataSources("Col2010").Concepts.Values.SingleOrDefault(o => o.ProviderRecordId == providerRecordId);
        }
    }
}
