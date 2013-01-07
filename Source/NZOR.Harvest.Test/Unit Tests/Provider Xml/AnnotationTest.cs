using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Configuration;

using NZOR.Data.Repositories.Provider;
using NZOR.Data.Repositories.Common;
using NZOR.Data.Sql.Repositories.Provider;
using NZOR.Data.Sql.Repositories.Common;
using System.Xml.Linq;
using NZOR.Admin.Data.Repositories;
using NZOR.Admin.Data.Sql.Repositories;

namespace NZOR.Harvest.Test.Unit_Tests.Provider_Xml
{
    [TestFixture()]
    public class AnnotationTest
    {
        private INameRepository _nameRepository;
        private IReferenceRepository _referenceRepository;
        private IConceptRepository _conceptRepository;
        private ILookUpRepository _lookUpRepository;
        private ITaxonPropertyRepository _taxonPropRepository;
        private IAnnotationRepository _annotationRepository;
        private IProviderRepository _provRepository;

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
        public void CanLoadNameAnnotationFromXml()
        {
            TestFixtureSetUp();

            XDocument document = XDocument.Load(@"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-Col2010-TaxonName.xml");

            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Assert.That(importer.DataSources("NZOR_Test").Annotations.Count > 0);
            Assert.That(importer.DataSources("NZOR_Test").Annotations["testAnn1"].AnnotationText == "nom. inval.");

            importer.Save();
        }

        [Test]
        public void CanLoadConceptAnnotationFromXml()
        {
            TestFixtureSetUp();

            XDocument document = XDocument.Load(@"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-Col2010-TaxonConcept.xml");

            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Assert.That(importer.DataSources("NZOR_Test").Annotations.Count > 0);
            Assert.That(importer.DataSources("NZOR_Test").Annotations["testAnn2"].AnnotationText == "test annotation");
            Assert.That(importer.DataSources("NZOR_Test").Annotations["testAnn2"].ReferenceId != null);

            importer.Save();
        }


    }
}
