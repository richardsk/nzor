using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Consensus
{
    public class StackedName
    {
        public Guid StatckedNameId { get; set; }
        public Guid SeedNameId { get; set; }
        public Guid? AccordingToId { get; set; }
        public String AccordingTo { get; set; }
        public Guid NameId { get; set; }
        public Guid? TaxonRankId { get; set; }
        public string CanonicalName { get; set; }
        public string RankName { get; set; }
        public int? SortOrder { get; set; }
        public int? Depth { get; set; }

        public StackedName()
        {
            StatckedNameId = Guid.Empty;
            SeedNameId = Guid.Empty;
            AccordingToId = null;
            AccordingTo = null;
            NameId = Guid.Empty;
            TaxonRankId = null;
            CanonicalName = null;
            RankName = null;
            SortOrder = null;
            Depth = null;
        }

    }
}
