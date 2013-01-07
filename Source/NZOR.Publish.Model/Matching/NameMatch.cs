using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Names;

namespace NZOR.Publish.Model.Matching
{
    /// <summary>
    /// A potential match for a submitted name.
    /// </summary>
    public class NameMatch
    {
        public decimal Score { get; set; }
        public Name Name { get; set; }

        public List<ExternalLookup> ExternalLookups { get; set; }

        public NameMatch()
        {
            Score = 0M;
            Name = null;

            ExternalLookups = new List<ExternalLookup>();
        }
    }
}
