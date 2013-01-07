using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NZOR.Publish.Model.Search
{
    /// <summary>
    /// Represents an index field that can be used as a facet for filtering search results.
    /// </summary>
    [DebuggerDisplay("{FieldName}")]
    public class FilterField
    {
        public string FieldName { get; set; }
        public string DisplayName { get; set; }

        public List<FilterTerm> FilterTerms { get; set; }

        public FilterField()
        {
            FieldName = String.Empty;
            DisplayName = String.Empty;

            FilterTerms = new List<FilterTerm>();
        }
    }
}
