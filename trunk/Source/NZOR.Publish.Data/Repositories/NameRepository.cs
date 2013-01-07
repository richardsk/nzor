using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Lucene.Net.Analysis.Ext;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using NZOR.Publish.Data.Faceting;
using NZOR.Publish.Model.Names;
using NZOR.Publish.Model.Search;
using NZOR.Publish.Model.Search.Names;
using System.Text;
using System.Globalization;

namespace NZOR.Publish.Data.Repositories
{
    public class NameRepository
    {
        private const int MaximumMatchingTerms = 10;
        private const int MaximumPageSize = 100;

        private readonly string _nameIndexFolderFullName;
        private static List<FacetField> _facetFields;

        public NameRepository(string nameIndexFolderFullName)
        {
            _nameIndexFolderFullName = nameIndexFolderFullName;

            BuildFacetFields();
        }

        public int Count()
        {
            using (var directory = FSDirectory.Open(new DirectoryInfo(_nameIndexFolderFullName)))
            {
                using (IndexReader indexReader = IndexReader.Open(directory, true))
                {
                    return indexReader.NumDocs();
                }
            }
        }

        public class WhereNameResult
        {
            public int Total { get; private set; }
            public List<Name> Names { get; private set; }

            public WhereNameResult(int total, List<Name> names)
            {
                Total = total;
                Names = names;
            }
        }

        public Name GetByProviderId(string providerRecordId)
        {
            Name name = null;
            
            using (var directory = FSDirectory.Open(new DirectoryInfo(_nameIndexFolderFullName)))
            {
                using (var indexReader = IndexReader.Open(directory, true))
                {
                    using (var searcher = new IndexSearcher(indexReader))
                    {
                        var query = new BooleanQuery();

                        var parentQuery = new TermQuery(new Term("providerrecordid", providerRecordId));
                        query.Add(parentQuery, BooleanClause.Occur.MUST);

                        var hits = searcher.Search(query, null, 1);
                        
                        if (hits.TotalHits == 1)
                        {
                            var document = searcher.Doc(hits.ScoreDocs[0].doc);

                            string xml = document.Get("documentxml");

                            using (var reader = new StringReader(xml))
                            {
                                var serializer = new XmlSerializer(typeof(Name));

                                name = serializer.Deserialize(reader) as Name;
                            }
                        }
                    }
                }
            }

            return name;
        }

        public WhereNameResult Where(Guid? parentNameId, Guid? ancestorNameId, string biostatus, string status, DateTime? fromModifiedDate, int skip, int take)
        {
            int total = 0;
            var names = new List<Name>();
            int convertedFromModifiedDate = 0;

            if (fromModifiedDate.HasValue)
            {
                convertedFromModifiedDate = Convert.ToInt32(fromModifiedDate.Value.ToString("yyyyMMdd"));
            }

            using (var directory = FSDirectory.Open(new DirectoryInfo(_nameIndexFolderFullName)))
            {
                using (var indexReader = IndexReader.Open(directory, true))
                {
                    using (var searcher = new IndexSearcher(indexReader))
                    {
                        var query = new BooleanQuery();

                        if (parentNameId.HasValue)
                        {
                            var parentQuery = new TermQuery(new Term("parentnameid", parentNameId.ToString()));
                            query.Add(parentQuery, BooleanClause.Occur.MUST);
                        }

                        if (ancestorNameId.HasValue)
                        {
                            var ancestorsQuery = new TermQuery(new Term("ancestors", ancestorNameId.ToString()));
                            query.Add(ancestorsQuery, BooleanClause.Occur.MUST);
                        }

                        if (!String.IsNullOrWhiteSpace(biostatus))
                        {
                            var biostatusQuery = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "biostatus", new UnaccentedWordAnalyzer()).Parse(biostatus);
                            //  var biostatusQuery = new TermQuery(new Term("biostatus", biostatus));
                            query.Add(biostatusQuery, BooleanClause.Occur.MUST);
                        }

                        if (!String.IsNullOrWhiteSpace(status))
                        {
                            var statusQuery = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "status", new UnaccentedWordAnalyzer()).Parse(status);
                            query.Add(statusQuery, BooleanClause.Occur.MUST);
                        }

                        var fromModifiedDateQuery = NumericRangeQuery.NewIntRange("modifieddate", convertedFromModifiedDate, null, true, true);
                        query.Add(fromModifiedDateQuery, BooleanClause.Occur.MUST);

                        // The number of documents to retrieve must be greater than zero.
                        int topTotalHits = Math.Max(skip + take, 1);

                        var hits = searcher.Search(query, null, topTotalHits);
                        total = hits.TotalHits;

                        for (int index = skip; index < Math.Min(topTotalHits, hits.TotalHits); index++)
                        {
                            var document = searcher.Doc(hits.ScoreDocs[index].doc);

                            string xml = document.Get("documentxml");

                            using (var reader = new StringReader(xml))
                            {
                                var serializer = new XmlSerializer(typeof(Name));

                                names.Add(serializer.Deserialize(reader) as Name);
                            }
                        }
                    }
                }
            }

            return new WhereNameResult(total, names);
        }

        public Name SingleOrDefault(Guid id)
        {
            Name name = null;

            using (var directory = FSDirectory.Open(new DirectoryInfo(_nameIndexFolderFullName)))
            {
                using (IndexReader indexReader = IndexReader.Open(directory, true))
                {
                    using (Searcher searcher = new IndexSearcher(indexReader))
                    {
                        var query = new TermQuery(new Term("nameid", id.ToString()));

                        TopDocs hits = searcher.Search(query, 1);

                        if (hits.TotalHits > 0)
                        {
                            var document = searcher.Doc(hits.ScoreDocs[0].doc);

                            string xml = document.Get("documentxml");

                            using (StringReader reader = new StringReader(xml))
                            {
                                var serializer = new XmlSerializer(typeof(Name));

                                name = serializer.Deserialize(reader) as Name;
                            }
                        }
                    }
                }
            }

            return name;
        }

        /// <summary>
        /// Return a list of partial names based on the given query.
        /// </summary>
        /// <param name="query">Text to use for the prefix query</param>
        /// <param name="take">The number of name strings to return</param>
        /// <returns></returns>
        public List<string> LookupNames(string query, int take)
        {
            List<string> names = new List<string>(take);

            if (!String.IsNullOrWhiteSpace(query) && take > 0)
            {
                using (var directory = FSDirectory.Open(new DirectoryInfo(_nameIndexFolderFullName)))
                {
                    using (IndexReader indexReader = IndexReader.Open(directory, true))
                    {
                        using (Searcher searcher = new IndexSearcher(indexReader))
                        {
                            PrefixQuery prefixQuery = new PrefixQuery(new Term("partialnamelookup", query.ToLower().Replace(" ", String.Empty)));

                            SortField sortField = new SortField("partialnamelookup", SortField.STRING);
                            Sort sort = new Sort(sortField);

                            TopDocs hits = searcher.Search(prefixQuery, null, take, sort);

                            foreach (var hit in hits.ScoreDocs)
                            {
                                var document = searcher.Doc(hit.doc);

                                names.Add(document.Get("partialname"));
                            }
                        }
                    }
                }
            }

            return names;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchRequest"></param>
        public NamesSearchResponse Search(SearchRequest searchRequest)
        {
            var searchResponse = new NamesSearchResponse();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            searchResponse.Page = Math.Max(searchRequest.Page, 1);
            searchResponse.PageSize = Math.Min(searchRequest.PageSize, MaximumPageSize);

            if (!String.IsNullOrEmpty(searchRequest.Query))
            {

                using (var directory = FSDirectory.Open(new DirectoryInfo(_nameIndexFolderFullName)))
                {
                    using (var indexReader = IndexReader.Open(directory, true))
                    {
                        using (var searcher = new IndexSearcher(indexReader))
                        {
                            var analyzer = new UnaccentedWordAnalyzer();
                            var queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "fullname", analyzer);

                            queryParser.SetAllowLeadingWildcard(true);
                            queryParser.SetDefaultOperator(QueryParser.Operator.AND);

                            var parsedQuery = ParseQuery(queryParser, RemoveDiacritics(searchRequest.Query));

                            searcher.SetDefaultFieldSortScoring(true, false);

                            DetermineMatchingTerms(searchResponse, parsedQuery, indexReader);
                            var sort = GetSort(searchRequest.OrderBy);
                            searchResponse.FilterQueries = ParseFilterQueries(searchRequest.Filter);
                            var filter = GetFilter(searchResponse.FilterQueries);
                            int top = searchResponse.Page * searchResponse.PageSize;

                            var hits = searcher.Search(parsedQuery, filter, top, sort);

                            int startIndex = (searchResponse.Page - 1) * searchResponse.PageSize;
                            int endIndex = Math.Min(startIndex + searchResponse.PageSize, hits.TotalHits);

                            for (int index = startIndex; index < endIndex; index++)
                            {
                                var document = searcher.Doc(hits.ScoreDocs[index].doc);
                                var searchResult = new NameSearchResult();

                                string xml = document.Get("documentxml");

                                using (StringReader reader = new StringReader(xml))
                                {
                                    var serializer = new XmlSerializer(typeof(Name));

                                    searchResult.Name = serializer.Deserialize(reader) as Name;
                                }

                                searchResult.Score = (decimal)hits.ScoreDocs[index].score;

                                searchResponse.Results.Add(searchResult);
                            }

                            AddFilterFields(indexReader, searcher, parsedQuery, searchResponse);

                            searchResponse.Query = parsedQuery.ToString();
                            searchResponse.Total = hits.TotalHits;
                        }
                    }
                }
            }

            stopwatch.Stop();

            searchResponse.SearchTime = stopwatch.ElapsedMilliseconds;

            return searchResponse;
        }

        /// <summary>
        /// Remove diacritics and accents from the input string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string RemoveDiacritics(string input)
        {
            string formD = input.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < formD.Length; i++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(formD[i]);

                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(formD[i]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        /// <summary>
        /// Parses a given search query. If the query causes a parser error then it is reparsed using escaping. The query is lower cased.
        /// </summary>
        /// <param name="queryParser"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <remarks>
        /// A subset of the special characters are removed before the query is parsed.
        /// </remarks>
        private Query ParseQuery(QueryParser queryParser, string query)
        {
            Query parsedQuery;
            string safeQuery;
            const string CharactersToReplace = @"[\[\]]";

            try
            {
                safeQuery = Regex.Replace(query, CharactersToReplace, " ");

                parsedQuery = queryParser.Parse(safeQuery);
            }
            catch (ParseException)
            {
                safeQuery = QueryParser.Escape(query);

                parsedQuery = queryParser.Parse(safeQuery);
            }

            return parsedQuery;
        }

        /// <summary>
        /// Builds the facet fields structure for performing the faceting on the search results.
        /// </summary>
        private void BuildFacetFields()
        {
            using (var directory = FSDirectory.Open(new DirectoryInfo(_nameIndexFolderFullName)))
            {
                using (var indexReader = IndexReader.Open(directory, true))
                {
                    _facetFields = new List<FacetField>();

                    _facetFields.Add(new FacetField() { FieldName = "rank", DisplayName = "Taxon Rank" });
                    _facetFields.Add(new FacetField() { FieldName = "governingcode", DisplayName = "Governing Code" });
                    _facetFields.Add(new FacetField() { FieldName = "status", DisplayName = "Name Status" });
                    _facetFields.Add(new FacetField() { FieldName = "class", DisplayName = "Class" });
                    _facetFields.Add(new FacetField() { FieldName = "providercode", DisplayName = "Provider" });
                    _facetFields.Add(new FacetField() { FieldName = "kingdom", DisplayName = "Kingdom" });

                    foreach (FacetField facetField in _facetFields)
                    {
                        // Get a list of the unique term values.
                        var terms = indexReader.Terms(new Term(facetField.FieldName));
                        var termValues = new List<string>();

                        do
                        {
                            if (!terms.Term().Field().Equals(facetField.FieldName, StringComparison.OrdinalIgnoreCase))
                            {
                                break;
                            }

                            if (!String.IsNullOrWhiteSpace(terms.Term().Text()))
                            {
                                termValues.Add(terms.Term().Text());
                            }
                        } while (terms.Next());

                        foreach (string value in termValues)
                        {
                            var facet = new Facet();

                            facet.Value = value;

                            var termsFilter = new FieldCacheTermsFilter(facetField.FieldName, new string[] { facet.Value });
                            var ids = termsFilter.GetDocIdSet(indexReader);

                            facet.BitSet = new OpenBitSetDISI(ids.Iterator(), 1000000);

                            facetField.Facets.Add(facet);
                        }
                    }
                }
            }
        }

        private void DetermineMatchingTerms(SearchResponse searchResponse, Query query, IndexReader indexReader)
        {
            FilteredTermEnum termEnum = null;

            if (query.GetType() == typeof(WildcardQuery))
            {
                WildcardQuery matchingTermsQuery = (WildcardQuery)query;

                termEnum = matchingTermsQuery.GetEnum(indexReader);
            }
            else if (query.GetType() == typeof(FuzzyQuery))
            {
                FuzzyQuery matchingTermsQuery = (FuzzyQuery)query;

                termEnum = matchingTermsQuery.GetEnum(indexReader);
            }
            else if (query.GetType() == typeof(PrefixQuery))
            {
                PrefixQuery matchingTermsQuery = (PrefixQuery)query;

                termEnum = matchingTermsQuery.GetEnum(indexReader);
            }

            int count = 0;

            if (termEnum != null)
            {
                do
                {
                    if (termEnum.Term() != null)
                    {
                        searchResponse.MatchingTerms.Add(new MatchingTerm()
                        {
                            FieldName = termEnum.Term().Field(),
                            Text = termEnum.Term().Text(),
                            HitCount = termEnum.DocFreq()
                        });
                    }

                    count++;
                } while (termEnum.Next() && count < MaximumMatchingTerms);
            }

            if (termEnum != null) { termEnum.Close(); }
        }

        private Sort GetSort(string orderBy)
        {
            Sort sort = null;
            var sortFields = new List<SortField>();

            if (!String.IsNullOrEmpty(orderBy))
            {
                List<string> orderByFields = orderBy.Split(new Char[] { ',', ';' }).ToList();
                SortField sortField = null;

                foreach (string orderByField in orderByFields)
                {
                    string orderByFieldName = orderByField.Trim().ToLower();
                    bool isReverse = orderByField.EndsWith("desc", StringComparison.OrdinalIgnoreCase);

                    if (isReverse)
                    {
                        orderByFieldName = orderByFieldName.Substring(0, orderByFieldName.Length - 4).Trim();
                    }

                    int type = SortField.STRING;

                    if (orderByFieldName.Equals("ranksort", StringComparison.OrdinalIgnoreCase)) { type = SortField.INT; }
                    if (orderByFieldName.Equals("score", StringComparison.OrdinalIgnoreCase)) { type = SortField.SCORE; }

                    sortField = new SortField(orderByFieldName, type, isReverse);

                    sortFields.Add(sortField);
                }
            }
            else
            {
                sortFields.Add(SortField.FIELD_SCORE);
            }

            sort = new Sort(sortFields.ToArray());

            return sort;
        }

        private Filter GetFilter(List<FilterQuery> filterQueries)
        {
            if (filterQueries.Any())
            {
                var booleanFilter = new BooleanFilter();

                foreach (var filterQuery in filterQueries)
                {
                    var filter = new FieldCacheTermsFilter(filterQuery.FieldName, new string[] { filterQuery.Text.ToLower() });
                    var clause = new BooleanFilterClause(filter, BooleanClause.Occur.MUST);

                    booleanFilter.Add(clause);
                }

                return booleanFilter;
            }
            else
            {
                return null;
            }
        }

        private void AddFilterFields(IndexReader indexReader, Searcher searcher, Query query, SearchResponse response)
        {
            var searchQueryFilter = new QueryWrapperFilter(query);
            var docIdSet = searchQueryFilter.GetDocIdSet(indexReader);

            if (docIdSet.Iterator() != null)
            {
                var searchBitSet = new OpenBitSetDISI(docIdSet.Iterator(), 1000000);

                foreach (var facetField in _facetFields)
                {
                    var filterField = new FilterField() { FieldName = facetField.FieldName, DisplayName = facetField.DisplayName };
                    response.FilterFields.Add(filterField);

                    foreach (var facet in facetField.Facets)
                    {
                        OpenBitSet combinedBitSet = (OpenBitSet)searchBitSet.Clone();

                        combinedBitSet.And(facet.BitSet);

                        FilterTerm filterTerm;

                        if (combinedBitSet.Cardinality() > 0)
                        {
                            filterTerm = new FilterTerm() { Text = facet.Value, HitCount = (int)combinedBitSet.Cardinality() };
                            filterField.FilterTerms.Add(filterTerm);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parse the filter into separate filter queries. Use the display name from the facet fields if possible for the display name
        /// of the query filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private List<FilterQuery> ParseFilterQueries(string filter)
        {
            var filterQueries = new List<FilterQuery>();

            if (!String.IsNullOrWhiteSpace(filter))
            {
                List<string> filters = filter.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                filters.ForEach(o =>
                {
                    string[] filterParts = o.Split(new char[] { ':' });

                    if (filterParts.Length == 2)
                    {
                        string fieldName = filterParts[0];
                        string displayName = String.Empty;
                        string text = filterParts[1];
                        FacetField facetField = _facetFields.SingleOrDefault(f => f.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));

                        if (facetField != null)
                        {
                            displayName = facetField.DisplayName ?? String.Empty;
                        }

                        filterQueries.Add(new FilterQuery() { FieldName = fieldName, DisplayName = displayName, Text = text });
                    }
                });
            }

            return filterQueries;
        }
    }
}
