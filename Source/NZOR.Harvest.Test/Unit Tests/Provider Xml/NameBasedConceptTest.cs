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
using NZOR.Harvest.Parsers;

namespace NZOR.Harvest.Test.UnitTests.ProviderXml
{
    [TestFixture]
    public class NameBasedConceptTest
    {
        private INameRepository _nameRepository;
        private IReferenceRepository _referenceRepository;
        private IConceptRepository _conceptRepository;
        private ILookUpRepository _lookUpRepository;
        private ITaxonPropertyRepository _taxonPropRepository;
        private IAnnotationRepository _annotationRepository;
        private IProviderRepository _provRepository;

        private const String BasicValidVernacularConcept = @"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-Col2010-NameBasedConcept.xml";

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
        }

        [Test]
        public void CanLoadDataSourceFromProviderXml()
        {
            XDocument document = XDocument.Load(BasicValidVernacularConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Assert.That(importer.DataSources("Col2010"), Is.Not.Null);
            Assert.That(importer.DataSources("Col2010").Concepts.Values.Where(o => o.State == NZOR.Data.Entities.Entity.EntityState.Added).Count(), Is.EqualTo(5));
        }

        [Test]
        public void CanCreateConceptFromProviderXml()
        {
            Concept testConcept = GetTestConcept("484D859A-57D1-4D88-8B54-0003FEB4A527");

            Assert.That(testConcept, Is.Not.Null);
        }

        [Test]
        public void CanLoadNameValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("484D859A-57D1-4D88-8B54-0003FEB4A527");

            Assert.That(testConcept.ProviderNameId, Is.EqualTo("25CBFE9B-4428-4BDF-A3FF-6AA70AE55515"));
        }

        [Test]
        public void CanLinkNameValue()
        {
            XDocument document = XDocument.Load(BasicValidVernacularConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Concept concept = importer.DataSources("Col2010").Concepts.Values.SingleOrDefault(o => o.ProviderRecordId == "484D859A-57D1-4D88-8B54-0003FEB4A527");
            Name referencedName = importer.DataSources("Col2010").Names.Values.SingleOrDefault(o => o.ProviderRecordId == concept.ProviderNameId);

            Assert.That(concept, Is.Not.Null);
            Assert.That(referencedName, Is.Not.Null);

            Assert.That(concept.NameId, Is.EqualTo(referencedName.NameId));
        }

        [Test]
        public void CanLoadAccordingToValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("484D859A-57D1-4D88-8B54-0003FEB4A527");

            Assert.That(testConcept.ProviderReferenceId, Is.EqualTo("8C497F0A-EF2F-4792-80E4-D67B5C7856BA"));
        }

        [Test]
        public void CanLinkAccordingToValue()
        {
            XDocument document = XDocument.Load(BasicValidVernacularConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Concept concept = importer.DataSources("Col2010").Concepts.Values.SingleOrDefault(o => o.ProviderRecordId == "484D859A-57D1-4D88-8B54-0003FEB4A527");
            Reference referencedReference = importer.DataSources("Col2010").References.Values.SingleOrDefault(o => o.ProviderRecordId == concept.ProviderReferenceId);

            Assert.That(concept, Is.Not.Null);
            Assert.That(referencedReference, Is.Not.Null);

            Assert.That(concept.AccordingToReferenceId, Is.EqualTo(referencedReference.ReferenceId));
        }

        [Test]
        public void CanAcceptedLoadNameValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("484D859A-57D1-4D88-8B54-0003FEB4A527");

            Assert.That(testConcept.ProviderNameId, Is.EqualTo("794EA1E0-8729-4699-BFA7-893EFCC32C62"));
        }

        [Test]
        public void CanLinkAcceptedNameValue()
        {
            XDocument document = XDocument.Load(BasicValidVernacularConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Concept concept = importer.DataSources("Col2010").Concepts.Values.SingleOrDefault(o => o.ProviderRecordId == "484D859A-57D1-4D88-8B54-0003FEB4A527");
            Name referencedName = importer.DataSources("Col2010").Names.Values.SingleOrDefault(o => o.ProviderRecordId == concept.ProviderNameId);

            Assert.That(concept, Is.Not.Null);
            Assert.That(referencedName, Is.Not.Null);

            Assert.That(concept.NameId, Is.EqualTo(referencedName.NameId));
        }

        [Test]
        public void CanLoadParentNameValueFromProviderXml()
        {
            TestFixtureSetUp();
            Concept testConcept = GetTestConcept("484D859A-57D1-4D88-8B54-0003FEB4A527");

            Assert.That(testConcept.ProviderNameId, Is.EqualTo("445AA224-4417-4445-9C51-C159C7EEFCE0"));
        }

        [Test]
        public void CanParseTaxonNameUse()
        {
            TestFixtureSetUp();
            ConceptRelationshipTypeLookUp conceptRelationshipTypeLookUp = new ConceptRelationshipTypeLookUp(_lookUpRepository.GetConceptRelationshipTypes());
            TaxonNameUseParser parser = new TaxonNameUseParser(new Guid("175D49CD-0785-4008-BB56-04DF3E46DE13"), conceptRelationshipTypeLookUp);
            XElement nameUseElement = XElement.Parse("<TaxonNameUse><Name ref=\"794EA1E0-8729-4699-BFA7-893EFCC32C62\" /><AcceptedName ref=\"794EA1E0-8729-4699-BFA7-893EFCC32C62\" inUse=\"1\" /></TaxonNameUse>");
            parser.Parse(nameUseElement, new System.Collections.Generic.List<Concept>());
        }

        [Test]
        public void CanLinkParentNameValue()
        {
            XDocument document = XDocument.Load(BasicValidVernacularConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Concept concept = importer.DataSources("Col2010").Concepts.Values.SingleOrDefault(o => o.ProviderRecordId == "484D859A-57D1-4D88-8B54-0003FEB4A527");
            Name referencedName = importer.DataSources("Col2010").Names.Values.SingleOrDefault(o => o.ProviderRecordId == concept.ProviderNameId);

            Assert.That(concept, Is.Not.Null);
            Assert.That(referencedName, Is.Not.Null);

            Assert.That(concept.NameId, Is.EqualTo(referencedName.NameId));
        }

        private Concept GetTestConcept(String providerRecordId)
        {
            XDocument document = XDocument.Load(BasicValidVernacularConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            return importer.DataSources("Col2010").Concepts.Values.SingleOrDefault(o => o.ProviderRecordId == providerRecordId);
        }
    }
}
