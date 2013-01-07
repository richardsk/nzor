using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NZOR.Publish.Data.Repositories;
using NZOR.Publish.Model.Search;
using NZOR.Publish.Model.Names;
using NZOR.Publish.Publisher.SearchIndexes;
using System.IO;
using System.Xml.Serialization;

namespace NZOR.Publish.Data.Test
{
    [TestFixture]
    public class NameRepositoryTest
    {
        private NameRepository _nameRepository;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            string nameXml = GetResource("NZOR.Publish.Data.Test.Data.TestNames.xml");
            var names = new List<Name>();

            using (var reader = new StringReader(nameXml))
            {
                var serializer = new XmlSerializer(typeof(List<Name>));

                names = serializer.Deserialize(reader) as List<Name>;
            }

            string namesIndexFolderFullName = System.IO.Path.GetTempPath();

            var builder = new NamesIndexBuilder(names, namesIndexFolderFullName);

            builder.CreateIndexes();

            _nameRepository = new NameRepository(Path.Combine(namesIndexFolderFullName, @"Names"));
        }

        [Test]
        public void BiostatusSearchIsCaseInsensitive()
        {
            var names = _nameRepository.Where(null, null, "wild", null, null, 1, 1);

            Assert.That(names.Total, Is.GreaterThan(0));

            names = _nameRepository.Where(null, null, "Wild", null, null, 1, 1);

            Assert.That(names.Total, Is.GreaterThan(0));
        }

        [Test]
        public void CanRetrieveNameLookupDetailsForScientificName()
        {
            TestFixtureSetUp();

            var names = _nameRepository.LookupNames("apt", 10);

            Assert.That(names, Is.Not.Null);
            Assert.That(names, Has.Count.EqualTo(2));
        }

        [Test]
        public void CanRetrieveNameLookupDetailsForVernaculars()
        {
            var names = _nameRepository.LookupNames("fly", 10);

            Assert.That(names, Is.Not.Null);
        }

        [Test]
        public void CanRetrieveAllNamesWithNoCriteria()
        {
            var names = _nameRepository.Where(null, null, null, null, null, 0, 10);

            Assert.That(names.Total, Is.EqualTo(15));
        }

        [Test]
        public void CanRetrieveNamesModifiedAfterDate()
        {
            var names = _nameRepository.Where(null, null, null, null, new DateTime(2013, 1, 1), 0, 10);

            Assert.That(names.Total, Is.EqualTo(1));
        }

        [Test]
        public void CanRetrieveNamesWithSpecificAncestor()
        {
            TestFixtureSetUp();
            var names = _nameRepository.Where(null, new Guid("99846ed3-8a8c-4337-a792-a75699348a77"), null, null, null, 0, 10);

            Assert.That(names.Total, Is.EqualTo(8));
        }

        [Test]
        public void CanRetrieveNamesWithBiostatus()
        {
            var names = _nameRepository.Where(null, null, "Wild", null, null, 0, 10);

            Assert.That(names.Total, Is.EqualTo(12));
        }

        [Test]
        public void CanRetrieveNamesWithStatus()
        {
            var names = _nameRepository.Where(null, null, null, "Current", null, 0, 10);

            Assert.That(names.Total, Is.EqualTo(11));
        }

        [Test]
        public void CanFacetSearchResults()
        {
            var searchRequest = new SearchRequest
            {
                Query = "*copros*",
                Filter = "rank:genus"
            };

            var searchResponse = _nameRepository.Search(searchRequest);

            Assert.That(searchResponse, Is.Not.Null);
            Assert.That(searchResponse.Results, Has.Count.EqualTo(1));
        }

        [Test]
        public void CanSearchWithWildcardCriteria()
        {
            TestFixtureSetUp();
            var searchRequest = new SearchRequest
                {
                    Query = "*copros*"
                };

            var searchResponse = _nameRepository.Search(searchRequest);

            Assert.That(searchResponse, Is.Not.Null);
            Assert.That(searchResponse.Total, Is.EqualTo(8));
        }

        [Test]
        public void CanSearchWithSpecialCharacters()
        {
            var searchRequest = new SearchRequest
            {
                Query = "amanita  [ined.]"
            };

            var searchResponse = _nameRepository.Search(searchRequest);

            Assert.That(searchResponse, Is.Not.Null);
        }

        [Test]
        public void CanSortSearchResults()
        {
            var searchRequest = new SearchRequest
            {
                Query = "*amanita*",
                OrderBy = "fullnamesort"
            };

            var searchResponse = _nameRepository.Search(searchRequest);

            Assert.That(searchResponse, Is.Not.Null);
        }

        [Test]
        public void CanGetName()
        {
            var name = new Name();

            name.ProviderNames.Add(new Model.Providers.ProviderNameLink { ProviderCode = "A23" });
            name.ProviderNames.Add(new Model.Providers.ProviderNameLink { ProviderCode = "A23" });
            name.ProviderNames.Add(new Model.Providers.ProviderNameLink { ProviderCode = "B23" });

            var providerCodes = name.ProviderNames.Select(o => o.ProviderCode).Distinct();
        }

        [Test]
        public void CanSearchUsingDiacritics()
        {
            var searchRequest = new SearchRequest
            {
                Query = "*leucothoës*",
            };

            var searchResponse = _nameRepository.Search(searchRequest);

            Assert.That(searchResponse.Total, Is.EqualTo(1));

            searchRequest = new SearchRequest
           {
               Query = "*leucothoes*",
           };

            searchResponse = _nameRepository.Search(searchRequest);

            Assert.That(searchResponse.Total, Is.EqualTo(1));
        }

        [Test]
        public void CanSearchWithComplicatedQuery()
        {
            var searchRequest = new SearchRequest
               {
                   Query = " coprosma~ AND serrulata~ AND hook~ AND Buch*~  ",
               };

            var searchResponse = _nameRepository.Search(searchRequest);

            Assert.That(searchResponse, Is.Not.Null);
        }

        private static string GetResource(string name)
        {
            string resource = String.Empty;
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(name))
            {
                using (var streamReader = new System.IO.StreamReader(stream))
                {
                    resource = streamReader.ReadToEnd();
                }
            }

            return resource;
        }
    }
}

