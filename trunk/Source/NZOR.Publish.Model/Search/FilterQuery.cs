using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Search
{
    /// <summary>
    /// Represents a field that is used to filter the search results for a faceted search.
    /// </summary>
    public class FilterQuery
    {
        public string FieldName { get; set; }
        public string DisplayName { get; set; }
        public string Text { get; set; }

        public FilterQuery()
        {
            FieldName = String.Empty;
            DisplayName = String.Empty;
            Text = String.Empty;
        }
    }
}
