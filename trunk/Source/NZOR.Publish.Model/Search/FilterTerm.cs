using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Search
{
    /// <summary>
    /// Filter terms are used with faceted searching to filter the results.
    /// </summary>
    public class FilterTerm
    {
        public string Text { get; set; }

        public int HitCount { get; set; }

        public FilterTerm()
        {
            Text = String.Empty;

            HitCount = 0;
        }
    }
}
