using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;
using Database.Test.Helper;
using NUnit.Framework;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Repositories.Common;
using NZOR.Data.Repositories.Provider;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Data.Sql.Repositories.Provider;
using NZOR.Data.Test;
using NZOR.Admin.Data.Repositories;
using NZOR.Admin.Data.Sql.Repositories;

namespace NZOR.Harvest.Test.UnitTests.ProviderXml
{
    [TestFixture]
    public class ImporterTest
    {
        private INameRepository _nameRepository;
        private IReferenceRepository _referenceRepository;
        private IConceptRepository _conceptRepository;
        private ILookUpRepository _lookUpRepository;
        private ITaxonPropertyRepository _taxonPropRepository;
        private IAnnotationRepository _annotationRepository;
        private IProviderRepository _providerRepository; 

        private NameClassLookUp _nameClassLookUp;
        private TaxonRankLookUp _taxonRankLookUp;

        private DatabaseTestHelper _databaseTestHelper;

        private const String BasicValidSource = @"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-NZFLORA-InitialDataSet.xml";
        private const String BaseSource = @"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-BaseDataSet.xml";
        private const String UpdateSource = @"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-Update.xml";
        private const String MultipleParentSource = @"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-MultipleParentConcepts.xml";
        private const String ParentageChangeSource = @"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-UpdateToLoseParentage.xml";
        private const String DeprecatedSource = @"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Oai Data\NZOR-Test-DeprecatedRecord.xml";

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
            _providerRepository = new ProviderRepository(connectionString);

            _nameClassLookUp = new NameClassLookUp(_lookUpRepository.GetNameClasses());
            _taxonRankLookUp = new TaxonRankLookUp(_lookUpRepository.GetTaxonRanks());

            _databaseTestHelper = new DatabaseTestHelper(connectionString);
        }

        [Test]
        [Ignore("Only needs running with the debugger")]
        public void CanSaveLargeDataSet()
        {
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.ResetDatabase));
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertProviderData));
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertBaseConsensusNameData));

            String dsConnectionString = ConfigurationManager.ConnectionStrings["PlantName_Cache"].ConnectionString;
            XDocument document = LoadLargeDatasetDocument(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.GetNZORTestLargeDataset), dsConnectionString);

            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _providerRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Assert.That(importer.DataSources("NZOR_Test"), Is.Not.Null);
            Assert.That(importer.DataSources("NZOR_Test").References.Count, Is.GreaterThan(0));

            importer.Save();
        }
        
        private XDocument LoadLargeDatasetDocument(String sql, String connectionString)
        {
            XDocument document = null;
            DatabaseTestHelper helper = new DatabaseTestHelper(connectionString);
            String xml = helper.GetXmlAsString(sql);

            document = XDocument.Parse(xml);

            return document;
        }

        [Test]
        public void CanImportBaseDataSet()
        {
            TestFixtureSetUp();

            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.ResetDatabase), 600);
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertProviderData));
            _databaseTestHelper.ExecuteSql(DatabaseTestSql.GetSql(DatabaseTestSql.SqlScript.InsertBaseConsensusNameData));
            
            XDocument document = XDocument.Load(BaseSource);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _providerRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Assert.That(importer.DataSources("NZOR_Test"), Is.Not.Null);

            Assert.That(_databaseTestHelper.Count(table: "provider.Name", target: null), Is.EqualTo(0), @"Database must be empty.");

            importer.Save();

            Assert.That(_databaseTestHelper.Count(table: "provider.Name", target: null), Is.EqualTo(16));

            var nameRecord = _databaseTestHelper.Single(table: "provider.Name", target: new { ProviderRecordID = "39BECB7F-180A-4415-8684-5EAA441795F0" });

            Assert.That(nameRecord.FullName, Is.EqualTo("Asterales"));
            Assert.That(nameRecord.NameClassID, Is.EqualTo(_nameClassLookUp.GetNameClass(NameClassLookUp.ScientificName).NameClassId));
            Assert.That(nameRecord.TaxonRankID, Is.EqualTo(_taxonRankLookUp.GetTaxonRank("Order", "ICBN").TaxonRankId));
            Assert.That(nameRecord.GoverningCode, Is.EqualTo("ICBN"));

            Assert.That(_databaseTestHelper.Count(table: "provider.NameProperty", target: new { NameID = nameRecord.NameID }), Is.EqualTo(2));

            nameRecord = _databaseTestHelper.Single(table: "provider.Name", target: new { ProviderRecordID = "856C1CCF-3AF5-4C41-B1D1-9B01B94D7005" });

            Assert.That(nameRecord.FullName, Is.EqualTo("Chamaemelum nobile (L.) All."));

            Assert.That(_databaseTestHelper.Count(table: "provider.Concept", target: null), Is.EqualTo(16));

            var conceptRecord = _databaseTestHelper.Single(table: "provider.Concept", target: new { ProviderRecordID = "C8F1A295-76E5-4969-ADA6-A2E21B352EA1" });

            Assert.That(conceptRecord.ProviderReferenceID, Is.EqualTo("12139D15-401B-4FED-ADD6-02DBA365A530"));

            conceptRecord = _databaseTestHelper.Single(table: "provider.Concept", target: new { ProviderNameID = "B9E3460B-AFF5-440B-AFE2-37C59F4E2779" });

            Assert.That(_databaseTestHelper.Count(table: "provider.ConceptRelationship", target: new { FromConceptID = conceptRecord.ConceptID }), Is.EqualTo(1));
        }

        [Test]
        public void CanImportUpdate()
        {
            TestFixtureSetUp();

            XDocument document = XDocument.Load(UpdateSource);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _providerRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Assert.That(importer.DataSources("NZOR_Test"), Is.Not.Null);

            importer.Save();
        }

        [Test]
        public void CanImportDeprecatedRecord()
        {
            TestFixtureSetUp();

            XDocument document = XDocument.Load(DeprecatedSource);

            DateTime dateBeforeDep = DateTime.Now;

            //get deprecated records OAI status=deleted
            
            IEnumerable<XElement> deprecated = from h in document.Descendants(XName.Get("header", "http://www.openarchives.org/OAI/2.0/")) where h.Attribute("status") != null && h.Attribute("status").Value == "deleted" select h;
            
            List<Data.Entities.Integration.DeprecatedRecord> depRecords = new List<Data.Entities.Integration.DeprecatedRecord>();
            foreach (XElement depEl in deprecated)
            {
                XElement idEl = depEl.Element(XName.Get("identifier", "http://www.openarchives.org/OAI/2.0/"));
                XElement specEl = depEl.Element(XName.Get("setSpec", "http://www.openarchives.org/OAI/2.0/"));

                if (idEl != null && specEl != null)
                {
                    Data.Entities.Integration.DeprecatedRecord dr = new Data.Entities.Integration.DeprecatedRecord();
                    dr.ProviderCode = "NZOR_TEST";
                    dr.DataSourceCode = "NZOR_Test";
                    dr.ProviderRecordId = idEl.Value;
                    
                    switch (specEl.Value)
                    {
                        case "TaxonName": 
                        case "VernacularName":
                            dr.RecordType = Admin.Data.Entities.NZORRecordType.Name;
                            break;
                        case "TaxonNameUse":
                        case "VernacularUse":
                        case "TaxonConcept":
                        case "VernacularConcept":
                            dr.RecordType = Admin.Data.Entities.NZORRecordType.Concept;
                            break;
                        case "Publication":
                            dr.RecordType = Admin.Data.Entities.NZORRecordType.Reference;
                            break;
                        case "Biostatus":
                            dr.RecordType = Admin.Data.Entities.NZORRecordType.TaxonProperty;
                            break;
                        default:
                            dr.RecordType = Admin.Data.Entities.NZORRecordType.None;
                            break;
                    }

                    depRecords.Add(dr);
                }
            }

            string intConfig = System.Configuration.ConfigurationManager.AppSettings["Integration Config File"];
            System.Xml.XmlDocument config = new System.Xml.XmlDocument();
            config.Load(intConfig);

            NZOR.Integration.UpdateProcessor proc = new Integration.UpdateProcessor();
            proc.ProcessUpdatedProviderData(dateBeforeDep, config, depRecords);
            
        }

        [Test]
        public void CanImportParentageChange()
        {
            TestFixtureSetUp();

            XDocument document = XDocument.Load(ParentageChangeSource);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _providerRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Assert.That(importer.DataSources("NZOR_Test"), Is.Not.Null);

            importer.Save();
        }

        [Test]
        public void CanProcessImportChanges()
        {
            string intConfig = System.Configuration.ConfigurationManager.AppSettings["Integration Config File"];
            System.Xml.XmlDocument config = new System.Xml.XmlDocument();
            config.Load(intConfig);

            NZOR.Integration.UpdateProcessor proc = new Integration.UpdateProcessor();
            proc.ProcessUpdatedProviderData(new DateTime(2012,11,12,5,0,0), config, null);
        }


        [Test]
        public void CanImportMultipleParentConcepts()
        {
            XDocument document = XDocument.Load(MultipleParentSource);
            Importer importer = new Importer(_nameRepository, _referenceRepository, _conceptRepository, _lookUpRepository, _taxonPropRepository, _annotationRepository, _providerRepository, Utility.ValidationSchemaUrl);

            importer.Import(document);

            Assert.That(importer.DataSources("NZOR_Test"), Is.Not.Null);

            importer.Save();
        }
    }
}
