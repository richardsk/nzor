using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using NZOR.Publish.Publisher.Builders;
using NZOR.Publish.Model.Administration;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Publisher.SearchIndexes;

namespace NZOR.Publish.Publisher
{
    public class IndexBuilder
    {
        private string _outputFullFolderName = "";
        private string _connectionString = "";

        public IndexBuilder(string dbConnectionString, string outputFolderName)
        {
            this._connectionString = dbConnectionString;
            this._outputFullFolderName = outputFolderName;
        }

        public static string Serialize<T>(T records)
        {
            using (var writer = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(T));

                serializer.Serialize(writer, records);

                return writer.ToString();
            }
        }

        public void WriteXmlToOutputFolder(string fileName, string xml)
        {
            if (!Directory.Exists(_outputFullFolderName))
            {
                Directory.CreateDirectory(_outputFullFolderName);
            }

            File.WriteAllText(Path.Combine(_outputFullFolderName, fileName), xml);
        }

        public void BuildStatistics()
        {
            var builder = new StatisticsBuilder(_connectionString);

            builder.Build();

            string xml = Serialize<List<Statistic>>(builder.Statistics);
            WriteXmlToOutputFolder("Statistics.xml", xml);
        }

        public void BuildDeprecatedRecords()
        {
            var builder = new DeprecatedRecordsBuilder(_connectionString);

            builder.Build();

            string xml = Serialize<List<DeprecatedRecord>>(builder.DeprecatedRecords);
            WriteXmlToOutputFolder("DeprecatedRecords.xml", xml);
        }

        public void BuildProviders()
        {
            var builder = new ProvidersBuilder(_connectionString);

            builder.Build();

            string xml = Serialize<List<Provider>>(builder.Providers);
            WriteXmlToOutputFolder("Providers.xml", xml);
        }

        public void BuildGeographicSchemas()
        {
            var builder = new GeographicSchemasBuilder(_connectionString);

            builder.Build();

            string xml = Serialize<List<GeographicSchema>>(builder.GeographicSchemas);
            WriteXmlToOutputFolder("GeographicSchemas.xml", xml);
        }

        public void BuildTaxonProperties()
        {
            var builder = new TaxonPropertiesBuilder(_connectionString);

            builder.Build();

            string xml = Serialize<List<TaxonProperty>>(builder.TaxonProperties);
            WriteXmlToOutputFolder("TaxonProperties.xml", xml);
        }

        public void BuildTaxonRanks()
        {
            var builder = new TaxonRanksBuilder(_connectionString);

            builder.Build();

            string xml = Serialize<List<TaxonRank>>(builder.TaxonRanks);
            WriteXmlToOutputFolder("TaxonRanks.xml", xml);
        }

        public void BuildVocabularies()
        {
            var builder = new VocabulariesBuilder(_connectionString);

            builder.Build();

            string xml = Serialize<List<Vocabulary>>(builder.Vocabularies);
            WriteXmlToOutputFolder("Vocabularies.xml", xml);
        }

        public void BuildNames()
        {
            var namesBuilder = new NamesBuilder(_connectionString);

            namesBuilder.Build();

            var names = namesBuilder.Names;
            var namesIndexBuilder = new NamesIndexBuilder(names, Path.Combine(_outputFullFolderName, "Indexes"));

            namesIndexBuilder.CreateIndexes();
        }
    }
}
