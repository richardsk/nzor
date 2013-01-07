using System;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using NZOR.Data.Repositories.Common;
using NZOR.Data.Repositories.Provider;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Data.Sql.Repositories.Provider;
using NZOR.Harvest.Transformers;
using NZOR.Admin.Data.Sql.Repositories;

namespace NZOR.Harvest.Test.UnitTests.ProviderXml
{
    [TestFixture]
    public class OaiTransformationTest
    {
        [Test]
        public void CanTransformAndLoadOaiData()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            ILookUpRepository lookUpRepository = new LookUpRepository(connectionString);

            XDocument oaiInputDocument = XDocument.Load(@"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Oai Data\NZOR-Test-NZFLORA-Oai.xml");
            XDocument document;

            OaiTransformer transformer = new OaiTransformer();

            document = transformer.Transform(oaiInputDocument, Utility.OaiTransformUrl);

            INameRepository nameRepository = new NameRepository(connectionString);
            IReferenceRepository referenceRepository = new ReferenceRepository(connectionString);
            IConceptRepository conceptRepository = new ConceptRepository(connectionString);
            ITaxonPropertyRepository taxonPropRepository = new TaxonPropertyRepository(connectionString);
            IAnnotationRepository annRepository = new AnnotationRepository(connectionString);
            ProviderRepository provRepository = new ProviderRepository(connectionString);

            Importer importer = new Importer(nameRepository, referenceRepository, conceptRepository, lookUpRepository, taxonPropRepository, annRepository, provRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Assert.That(importer.DataSources("NZFLORA"), Is.Not.Null);
            Assert.That(importer.DataSources("NZFLORA").Names.Count(), Is.EqualTo(1));
        }
    }
}
