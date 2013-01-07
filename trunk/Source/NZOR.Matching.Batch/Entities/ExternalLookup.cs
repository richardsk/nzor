using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Matching.Batch
{
    /// <summary>
    /// Represents a lookup to an external service for further name details.
    /// </summary>
    public class ExternalLookup
    {
        public string OrganisationName { get; set; }
        public string SearchUrl { get; set; }
        public string Type { get; set; }

        public ExternalLookup()
        {
            OrganisationName = String.Empty;
            SearchUrl = String.Empty;
            Type = String.Empty;
        }
    }
}
