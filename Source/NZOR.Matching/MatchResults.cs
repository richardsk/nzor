using System;
using System.Data;
using System.Collections.Generic;

using NZOR.Data;

namespace NZOR.Matching
{
    public class MatchResult
    {
        /// <summary>
        /// The Id of the name that is given to the matching routine
        /// </summary>
        public string ProviderRecordId = null;
        public string ProviderNameFull = null;
        public string ProviderRefCitation = null;
        /// <summary>
        /// The NZOR Id of the name that matches, if any
        /// </summary>
        public string MatchedId = null;
        public string MatchedName = null;
        public string MatchedReference = null;
        public string MatchedConceptRelationship = null;
        public DataRow MatchedOtherData = null;
        public LinkStatus Status = LinkStatus.Unmatched;
        public string MatchPath = null;

        public string Error = null;

        /// <summary>
        /// List of name/ref matches (could be 0, 1, or many)
        /// </summary>
        public List<MatchInstance> Matches = new List<MatchInstance>();
    }

    public class MatchInstance
    {
        public Guid? Id = null;
        public string DisplayText = null; 
        public int MatchScore = 0;
    }
}
