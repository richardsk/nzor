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
    public class VernacularConceptTest
    {
        private INameRepository _nameRepository;
        private IReferenceRepository _referenceRepository;
        private IConceptRepository _conceptRepository;
        private ILookUpRepository _lookUpRepository;
        private ITaxonPropertyRepository _taxonPropRepository;
        private IAnnotationRepository _annotationRepository;
        private IProviderRepository _provRepository;
        
        private const String BasicValidVernacularConcept = @"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-Col2010-VernacularConcept.xml";

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
            Assert.That(importer.DataSources("Col2010").Concepts.Values.Where(o => o.State == NZOR.Data.Entities.Entity.EntityState.Added).Count(), Is.EqualTo(3));
        }

        [Test]
        public void CanCreateConceptFromProviderXml()
        {
            Concept testConcept = GetTestConcept("13610");

            Assert.That(testConcept, Is.Not.Null);
        }

        [Test]
        public void CanLoadAttributeValuesFromProviderXml()
        {
            Concept testConcept = GetTestConcept("13610");

            Assert.That(testConcept.ProviderCreatedDate, Is.EqualTo(new DateTime(2010, 10, 3, 16, 36, 0)));
            Assert.That(testConcept.ProviderModifiedDate, Is.EqualTo(new DateTime(2010, 11, 3, 16, 36, 0)));
        }

        [Test]
        public void CanLoadNameValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("13610");

            Assert.That(testConcept.ProviderNameId, Is.EqualTo("C8FC98AC-E9A1-48D8-B210-35ED0AC53174"));
        }

        [Test]
        public void CanLinkNameValue()
        {
            XDocument document = XDocument.Load(BasicValidVernacularConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Concept concept = importer.DataSources("Col2010").Concepts.Values.SingleOrDefault(o => o.ProviderRecordId == "13610");
            Name referencedName = importer.DataSources("Col2010").Names.Values.SingleOrDefault(o => o.ProviderRecordId == concept.ProviderNameId);

            Assert.That(concept, Is.Not.Null);
            Assert.That(referencedName, Is.Not.Null);

            Assert.That(concept.NameId, Is.EqualTo(referencedName.NameId));
        }

        [Test]
        public void CanLoadAccordingToValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("13610");

            Assert.That(testConcept.ProviderReferenceId, Is.EqualTo("5FF18BDB-9BE4-4D13-ABF4-FE24AFE21DBA"));
        }

        [Test]
        public void CanLinkAccordingToValue()
        {
            XDocument document = XDocument.Load(BasicValidVernacularConcept);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Concept concept = importer.DataSources("Col2010").Concepts.Values.SingleOrDefault(o => o.ProviderRecordId == "13610");
            Reference referencedReference = importer.DataSources("Col2010").References.Values.SingleOrDefault(o => o.ProviderRecordId == concept.ProviderReferenceId);

            Assert.That(concept, Is.Not.Null);
            Assert.That(referencedReference, Is.Not.Null);

            Assert.That(concept.AccordingToReferenceId, Is.EqualTo(referencedReference.ReferenceId));
        }

        [Test]
        public void CanLoadRankValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("13610");

            Assert.That(testConcept.TaxonRank, Is.EqualTo("Test Rank"));
        }

        [Test]
        public void CanLoadApplicationsValueFromProviderXml()
        {
            Concept testConcept = GetTestConcept("13610");

            Assert.That(testConcept.ConceptApplications.Count, Is.EqualTo(1));
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
