using System.Data;
using NZOR.Data;

namespace NZOR.Matching
{
    public class MatchResult
    {
        public string MatchedId;
        public string MatchedName;
        public string MatchedReference;
        public string MatchedConceptRelationship;
        public DataRow MatchedOtherData;
        public LinkStatus Status = LinkStatus.Unmatched;
    }

    public class NameMatch
    {
        public string NameId = "";
        public string NameFull = "";
        public int MatchScore = 0;
    }
}
