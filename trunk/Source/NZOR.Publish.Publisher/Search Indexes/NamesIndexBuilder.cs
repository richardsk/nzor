using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Lucene.Net.Analysis.Ext;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using NZOR.Publish.Model.Names;
using NZOR.Publish.Model.Providers;

namespace NZOR.Publish.Publisher.SearchIndexes
{
    public class NamesIndexBuilder
    {
        private readonly List<Name> _names;

        private string _targetSearchIndexBaseFolderFullName;

        const string NamesSubFolderName = "Names";

        public NamesIndexBuilder(List<Name> names, string targetSearchIndexBaseFolderFullName)
        {
            _names = names;
            _targetSearchIndexBaseFolderFullName = targetSearchIndexBaseFolderFullName;
        }

        public void CreateIndexes()
        {
            CreateFolders();
            CreateNamesIndex();
        }

        private void CreateFolders()
        {
            if (!System.IO.Directory.Exists(_targetSearchIndexBaseFolderFullName))
            {
                System.IO.Directory.CreateDirectory(_targetSearchIndexBaseFolderFullName);
            }

            CreateSubFolder(NamesSubFolderName);
        }

        private void CreateSubFolder(string targetSubFolderName)
        {
            string targetSubFolderFullName = Path.Combine(_targetSearchIndexBaseFolderFullName, targetSubFolderName);

            if (!System.IO.Directory.Exists(targetSubFolderFullName))
            {
                System.IO.Directory.CreateDirectory(targetSubFolderFullName);
            }
        }

        private void CreateNamesIndex()
        {
            string namesSearchIndexFolderFullName = Path.Combine(_targetSearchIndexBaseFolderFullName, NamesSubFolderName);

            using (var directory = FSDirectory.Open(new DirectoryInfo(namesSearchIndexFolderFullName)))
            {
                var analyzer = new UnaccentedWordAnalyzer();

                using (IndexWriter indexWriter = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.LIMITED))
                {
                    foreach (Name name in _names)
                    {
                        var document = new Document();

                        document.Add(new Field("nameid", name.NameId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

                        if (name.ParentName != null)
                        {
                            document.Add(new Field("parentnameid", name.ParentName.NameId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        }

                        document.Add(new Field("class", name.Class, Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("governingcode", name.GoverningCode, Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("acceptednamefullname", name.AcceptedName != null ? name.AcceptedName.FullName : String.Empty, Field.Store.YES, Field.Index.ANALYZED));

                        document.Add(new Field("fullname", name.FullName, Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("fullnamesort", name.FullName.Replace(" ", ""), Field.Store.NO, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("partialname", name.PartialName, Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("partialnamelookup", name.PartialName.Replace(" ", ""), Field.Store.YES, Field.Index.ANALYZED));

                        document.Add(new Field("rank", name.Rank, Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new NumericField("ranksort", Field.Store.YES, true).SetIntValue(name.RankSortOrder));

                        NameLink nameAtRank;

                        nameAtRank = name.ClassificationHierarchy.FirstOrDefault(o => o.Rank.Equals("Kingdom", StringComparison.OrdinalIgnoreCase));
                        if (nameAtRank != null)
                        {
                            document.Add(new Field("kingdom", nameAtRank.PartialName, Field.Store.YES, Field.Index.ANALYZED));
                        }

                        foreach (var ancestorName in name.ClassificationHierarchy)
                        {
                            document.Add(new Field("ancestors", ancestorName.NameId.ToString(), Field.Store.NO, Field.Index.NOT_ANALYZED));
                        }

                        foreach (var biostatus in name.Biostatuses)
                        {
                            biostatus.Biome.ForEach(o => document.Add(new Field("biostatus", o, Field.Store.NO, Field.Index.ANALYZED)));
                            biostatus.EnvironmentalContext.ForEach(o => document.Add(new Field("biostatus", o, Field.Store.NO, Field.Index.ANALYZED)));
                            biostatus.GeoRegion.ForEach(o => document.Add(new Field("biostatus", o, Field.Store.NO, Field.Index.ANALYZED)));
                            biostatus.GeoSchema.ForEach(o => document.Add(new Field("biostatus", o, Field.Store.NO, Field.Index.ANALYZED)));
                            biostatus.Occurrence.ForEach(o => document.Add(new Field("biostatus", o, Field.Store.NO, Field.Index.ANALYZED)));
                            biostatus.Origin.ForEach(o => document.Add(new Field("biostatus", o, Field.Store.NO, Field.Index.ANALYZED)));
                        }

                        document.Add(new Field("year", name.Year, Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("country", name.Country, Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("status", name.Status, Field.Store.YES, Field.Index.ANALYZED));

                        document.Add(new Field("isrecombination", name.IsRecombination.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

                        if (name.ProviderNames != null)
                        {
                            var uniqueProviderCodes = name.ProviderNames.Select(o => o.ProviderCode).Distinct();
                            foreach (string providerCode in uniqueProviderCodes)
                            {
                                document.Add(new Field("providercode", providerCode, Field.Store.YES, Field.Index.ANALYZED));
                            }
                            foreach (ProviderNameLink pnl in name.ProviderNames)
                            {
                                document.Add(new Field("providerrecordid", pnl.ProviderRecordId, Field.Store.YES, Field.Index.NOT_ANALYZED));
                            }
                        }

                        document.Add(new NumericField("modifieddate", Field.Store.YES, true).SetIntValue(Convert.ToInt32(name.ModifiedDate.ToString("yyyyMMdd"))));

                        string xml = String.Empty;

                        using (var writer = new StringWriter())
                        {
                            var serializer = new XmlSerializer(typeof(Name));

                            serializer.Serialize(writer, name);
                            xml = writer.ToString();
                        }

                        document.Add(new Field("documentxml", xml, Field.Store.COMPRESS, Field.Index.NO, Field.TermVector.NO));

                        indexWriter.AddDocument(document);
                    }
                    indexWriter.Optimize();
                    indexWriter.Close();
                }
            }
        }
    }
}
