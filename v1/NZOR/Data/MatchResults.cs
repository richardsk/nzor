using System;
using System.Data;
using System.Collections.Generic;

using NZOR.Data;

namespace NZOR.Data
{
    public class MatchResult
    {
        public string MatchedId = null;
        public string MatchedName = null;
        public string MatchedReference = null;
        public string MatchedConceptRelationship = null;
        public DataRow MatchedOtherData = null;
        public LinkStatus Status = LinkStatus.Unmatched;
        public string MatchPath = null;

        public List<NameMatch> Matches = null;
    }

    public class NameMatch
    {
        public Guid? NameId = null;
        public string NameFull = null;
        public int MatchScore = 0;
    }
}
