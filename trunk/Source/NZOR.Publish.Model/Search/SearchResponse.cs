using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Search
{
    public abstract class SearchResponse
    {
        /// <summary>
        /// The parsed query used to generate the result
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Comma separeted List of filter queries that are used as facets to refine the search (fieldname:text)
        /// </summary>
        public List<FilterQuery> FilterQueries { get; set; }

        /// <summary>
        /// The time in milliseconds for the search to complete
        /// </summary>
        public long SearchTime { get; set; }

        public int Total { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }

        public List<MatchingTerm> MatchingTerms { get; set; }

        public List<FilterField> FilterFields { get; set; }

        public SearchResponse()
        {
            FilterQueries = new List<FilterQuery>();
            MatchingTerms = new List<MatchingTerm>();
            FilterFields = new List<FilterField>();
        }
    }
}
