using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Search
{
    /// <summary>
    /// Represents a term from the index that may be a 'Did You Mean?' term for a search.
    /// </summary>
    /// <remarks>
    /// Matching terms can be used as the basis for a new more specific query.
    /// </remarks>
    public class MatchingTerm
    {
        public string FieldName { get; set; }
        public string Text { get; set; }
        public int HitCount { get; set; }

        public MatchingTerm()
        {
            FieldName = String.Empty;
            Text = String.Empty;
            HitCount = 0;
        }
    }
}
