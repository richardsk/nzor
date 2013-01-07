using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Common
{

    public class DistinctName
    {
        public string NameId = "";
        public string Canonical = "";
        public string Authors = "";
        public string Year = "";
        public string Genus = "";
        public string Species = "";
        public string GoverningCode = "";
        public Guid TaxonRankId = Guid.Empty;
        public int SortOrder = -1;
        public int MatchRuleSetId = -1;
    }
}
