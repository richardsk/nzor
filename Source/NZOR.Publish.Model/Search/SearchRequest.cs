using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Search
{
    public class SearchRequest
    {
        private const int DefaultPageSize = 10;

        /// <summary>
        /// Submitted query
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// Comma separated list of query filter field names
        /// </summary>
        public string Filter { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        /// <summary>
        /// Comma separated list of order by fields (can optionally include direction ASC (default) or DESC)
        /// </summary>
        public string OrderBy { get; set; }

        public SearchRequest()
        {
            Query = String.Empty;
            Filter = String.Empty;

            Page = 0;
            PageSize = DefaultPageSize;
            OrderBy = String.Empty;
        }
    }
}
