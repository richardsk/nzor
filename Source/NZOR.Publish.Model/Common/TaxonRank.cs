using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Common
{
    public class TaxonRank
    {
        public Guid TaxonRankId { get; set; }

        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public int SortOrder { get; set; }
    }
}
