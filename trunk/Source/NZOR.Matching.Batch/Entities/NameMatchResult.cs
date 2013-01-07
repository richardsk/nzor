using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Matching.Batch
{
    /// <summary>
    /// Represents the result of name matching for a submitted name.
    /// </summary>
    public class NameMatchResult
    {
        public string SubmittedId { get; set; }
        public string SubmittedScientificName { get; set; }

        public string Message { get; set; }

        public List<NameMatch> NameMatches { get; private set; }

        public NameMatchResult()
        {
            SubmittedId = String.Empty;
            SubmittedScientificName = String.Empty;

            Message = String.Empty;

            NameMatches = new List<NameMatch>();
        }
    }
}
