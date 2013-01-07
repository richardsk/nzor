using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Common
{
    public class TaxonRank
    {
        public Guid TaxonRankId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }
        public string KnownAbbreviations { get; set; }
        public Int32? SortOrder { get; set; }
        public Int32? MatchRuleSetId { get; set; }

        public bool? IncludeInFullName { get; set; }
        public bool? ShowRank { get; set; }
        public string GoverningCode { get; set; }

        public TaxonRank()
        {
            TaxonRankId = Guid.Empty;

            Name = string.Empty;
            DisplayName = string.Empty;
            KnownAbbreviations = string.Empty;
            SortOrder = null;
            MatchRuleSetId = null;

            IncludeInFullName = null;
            ShowRank = null;
            GoverningCode = null;
        }
    }
}
