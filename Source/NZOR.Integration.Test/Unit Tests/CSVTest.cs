using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;

using NZOR.Integration;
using NZOR.Data.DataSets;
using NZOR.Data.Entities.Common;
using NZOR.Integration.Mapping;
using NZOR.Matching;

namespace NZOR.Integration.Test.Unit_Tests
{
    [TestFixture]
    public class CSVTest
    {
        [Test]
        public void TestCSVMatch()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            Mapping.IntegrationMapping im = new Mapping.IntegrationMapping();
            im.HasColumnHeaders = true;
            im.NameClassID = new Guid("A5233111-61A0-4AE6-9C2B-5E8E71F1473A");

            im.AddMapping(new Mapping.ColumnMapping(0, "NameId", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "NameId"), ""));
            im.AddMapping(new Mapping.ColumnMapping(3, "FullName", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "FullName"), ""));
            im.AddMapping(new Mapping.ColumnMapping(4, "TaxonRank", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "TaxonRank"), ""));
            im.AddMapping(new Mapping.ColumnMapping(6, "Authors", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "Authors"), ""));
            im.AddMapping(new Mapping.ColumnMapping(20, "GoverningCode", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "GoverningCode"), ""));
            im.AddMapping(new Mapping.ColumnMapping(5, "Canonical", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "Canonical"), ""));
            im.AddMapping(new Mapping.ColumnMapping(28, "YearOfPublication", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "YearOfPublication"), ""));
            im.AddMapping(new Mapping.ColumnMapping(12, "MicroReference", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "MicroReference"), ""));
            im.AddMapping(new Mapping.ColumnMapping(10, "PublishedIn", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "PublishedIn"), ""));
            im.AddMapping(new Mapping.ColumnMapping(43, "ParentId", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "ParentID"), ""));
            im.AddMapping(new Mapping.ColumnMapping(42, "PreferredNameId", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "PreferredNameID"), ""));

            NZOR.Integration.CSVProcessor processor = new CSVProcessor();

            List<String> errors = new List<string>();
            NZOR.Data.Entities.Integration.DataForIntegration data = processor.GetDataForIntegration(cnnStr, "Resources\\PlantNames.csv", im, ref errors);

            Assert.That(errors.Count == 0);
            
            string f = @"C:\Development\NZOR\Source\NZOR.Integration\data.dat";
            NZOR.Data.Sql.Integration.SaveDataFile(data, f);
            
           Admin.Data.Sql.Repositories.ProviderRepository pr = new Admin.Data.Sql.Repositories.ProviderRepository(cnnStr);
           List<Admin.Data.Entities.AttachmentPoint> attPoints = pr.GetAllAttachmentPoints();

            Integration.MemoryIntegrationProcessor proc = new MemoryIntegrationProcessor();
            proc.RunIntegration("C:\\Development\\NZOR\\Source\\NZOR.Integration\\Configuration\\IntegConfig.xml", f, 0, attPoints);

            while (proc.Progress < 100) 
            {
                Console.WriteLine(proc.StatusText + " : " + proc.Progress.ToString() + "% complete.");
                System.Threading.Thread.Sleep(5000);
            }

            foreach (MatchResult res in proc.Results)
            {
                Assert.That(res.MatchedId, Is.Not.Null);
            }
            
        }
    }
}
