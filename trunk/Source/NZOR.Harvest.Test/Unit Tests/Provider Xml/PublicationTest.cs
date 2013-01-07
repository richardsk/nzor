using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
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
    public class ReferenceTest
    {
        private INameRepository _nameRepository;
        private IReferenceRepository _referenceRepository;
        private IConceptRepository _conceptRepository;
        private ILookUpRepository _lookUpRepository;
        private ITaxonPropertyRepository _taxonPropRepository;
        private IAnnotationRepository _annotationRepository;
        private IProviderRepository _provRepository;

        private ReferenceTypeLookUp _referenceTypeLookUp;
        private ReferencePropertyTypeLookUp _referencePropertyTypeLookUp;

        private const String BasicValidPublication = @"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-Col2010-Publication.xml";

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

            _referenceTypeLookUp = new ReferenceTypeLookUp(_lookUpRepository.GetReferenceTypes());
            _referencePropertyTypeLookUp = new ReferencePropertyTypeLookUp(_lookUpRepository.GetReferencePropertyTypes());
        }

        [Test]
        public void CanLoadDataSourceFromProviderXml()
        {
            XDocument document = XDocument.Load(BasicValidPublication);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Assert.That(importer.DataSources("Col2010"), Is.Not.Null);
            Assert.That(importer.DataSources("Col2010").References.Count, Is.GreaterThan(0));
        }

        [Test]
        public void CanCreateReferenceFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference, Is.Not.Null);
        }

        [Test]
        public void CanLoadAttributeValuesFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Single(
                 o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.ParentReferenceID).ReferencePropertyTypeId).Value,
                 Is.EqualTo("Test Parent Reference Id"));
            Assert.That(testReference.ReferenceTypeId, Is.EqualTo(_referenceTypeLookUp.GetReferenceType("Book").ReferenceTypeId));
            Assert.That(testReference.ProviderCreatedDate, Is.EqualTo(new DateTime(2010, 10, 5, 10, 1, 25)));
            Assert.That(testReference.ProviderModifiedDate, Is.EqualTo(new DateTime(2010, 10, 6, 10, 11, 25)));
        }

        [Test]
        public void CanLoadCitationValueFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Single(
                 o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Citation).ReferencePropertyTypeId).Value,
                 Is.EqualTo("Catalogue of Life"));
        }

        [Test]
        public void CanLoadIdentifierValuesFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Where(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Identifier).ReferencePropertyTypeId).Count(),
                Is.EqualTo(2));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Identifier).ReferencePropertyTypeId && o.Value == "Test Identifier 1"),
                Is.Not.Null);
        }

        [Test]
        public void CanLoadSimpleAuthorValuesFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Author).ReferencePropertyTypeId).Value,
                Is.EqualTo("Test Simple Author"));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Author).ReferencePropertyTypeId).SubType,
                Is.EqualTo("Simple"));
        }

        [Test]
        public void CanLoadExtendedAuthorValuesFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 2");

            Assert.That(testReference.ReferenceProperties.SingleOrDefault(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Author).ReferencePropertyTypeId && o.SubType == "Simple"),
                Is.Null);
            Assert.That(testReference.ReferenceProperties.Count(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Author).ReferencePropertyTypeId),
                Is.EqualTo(2));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Author).ReferencePropertyTypeId && o.SubType == "primary").Value,
                Is.EqualTo("Test Author 1"));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Author).ReferencePropertyTypeId && o.SubType == "secondary").Value,
                Is.EqualTo("Test Author 2"));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Author).ReferencePropertyTypeId && o.Sequence == 2).Value,
                Is.EqualTo("Test Author 2"));
        }

        [Test]
        public void CanLoadDateValuesFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Where(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Date).ReferencePropertyTypeId).Count(),
                Is.EqualTo(2));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Date).ReferencePropertyTypeId && o.SubType == "of publication").Value,
                Is.EqualTo("2010"));
        }

        [Test]
        public void CanLoadTitleValuesFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Where(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Title).ReferencePropertyTypeId).Count(),
                Is.EqualTo(1));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Title).ReferencePropertyTypeId && o.SubType == "full").Value,
                Is.EqualTo("Catalogue of Life"));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Title).ReferencePropertyTypeId && o.SubType == "full").Level,
                Is.EqualTo(1));
        }

        [Test]
        public void CanLoadSimpleEditorValuesFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Single(
                          o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Editor).ReferencePropertyTypeId).Value,
                          Is.EqualTo("Test Simple Editor"));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Editor).ReferencePropertyTypeId).SubType,
                Is.EqualTo("Simple"));
        }

        [Test]
        public void CanLoadExtendedEditorValuesFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 2");

            Assert.That(testReference.ReferenceProperties.SingleOrDefault(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Editor).ReferencePropertyTypeId && o.SubType == "Simple"),
                Is.Null);
            Assert.That(testReference.ReferenceProperties.Count(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Editor).ReferencePropertyTypeId),
                Is.EqualTo(3));
            Assert.That(testReference.ReferenceProperties.Count(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Editor).ReferencePropertyTypeId && o.Value == "Test Editor 1"),
                Is.EqualTo(1));
            Assert.That(testReference.ReferenceProperties.Count(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Editor).ReferencePropertyTypeId && o.Value == "Test Editor 2"),
                Is.EqualTo(1));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Editor).ReferencePropertyTypeId && o.Sequence == 2).Value,
                Is.EqualTo("Test Editor 2"));
        }

        [Test]
        public void CanLoadVolumeValueFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Volume).ReferencePropertyTypeId).Value,
                Is.EqualTo("Test Volume"));
        }

        [Test]
        public void CanLoadIssueValueFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Issue).ReferencePropertyTypeId).Value,
                Is.EqualTo("Test Issue"));
        }

        [Test]
        public void CanLoadEditionValueFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Edition).ReferencePropertyTypeId).Value,
                Is.EqualTo("Test Edition"));
        }

        [Test]
        public void CanLoadPageValuesFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Where(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Page).ReferencePropertyTypeId).Count(),
                Is.EqualTo(2));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Page).ReferencePropertyTypeId && o.SubType == "start").Value,
                Is.EqualTo("22"));
        }

        [Test]
        public void CanLoadPublisherValuesFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Publisher).ReferencePropertyTypeId).Value,
                Is.EqualTo("Test Publisher Name"));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.PlaceOfPublication).ReferencePropertyTypeId).Value,
                Is.EqualTo("Test Publisher City"));
        }

        [Test]
        public void CanLoadSimpleKeywordValuesFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Keyword).ReferencePropertyTypeId).Value,
                Is.EqualTo("Test Simple Keywords"));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Keyword).ReferencePropertyTypeId).SubType,
                Is.EqualTo("Simple"));
        }

        [Test]
        public void CanLoadExtendedKeywordValuesFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 2");

            Assert.That(testReference.ReferenceProperties.SingleOrDefault(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Keyword).ReferencePropertyTypeId && o.SubType == "Simple"),
                Is.Null);
            Assert.That(testReference.ReferenceProperties.Count(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Keyword).ReferencePropertyTypeId),
                Is.EqualTo(3));
            Assert.That(testReference.ReferenceProperties.Count(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Keyword).ReferencePropertyTypeId && o.Value == "Test Keyword 1"),
                Is.EqualTo(1));
            Assert.That(testReference.ReferenceProperties.Count(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Keyword).ReferencePropertyTypeId && o.Value == "Test Keyword 2"),
                Is.EqualTo(1));
            Assert.That(testReference.ReferenceProperties.Single(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Keyword).ReferencePropertyTypeId && o.Sequence == 2).Value,
                Is.EqualTo("Test Keyword 2"));
        }

        [Test]
        public void CanLoadLinkValuesFromProviderXml()
        {
            Reference testReference = GetTestReference("Test Id 1");

            Assert.That(testReference.ReferenceProperties.Where(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Link).ReferencePropertyTypeId).Count(),
                Is.EqualTo(3));
            Assert.That(testReference.ReferenceProperties.Any(
                o => o.ReferencePropertyTypeId == _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Link).ReferencePropertyTypeId && o.Value == "Test Link 1"),
                Is.True);
        }

        private Reference GetTestReference(String providerRecordId)
        {
            XDocument document = XDocument.Load(BasicValidPublication);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            return importer.DataSources("Col2010").References.Values.SingleOrDefault(o => o.ProviderRecordId == providerRecordId);
        }
    }
}
