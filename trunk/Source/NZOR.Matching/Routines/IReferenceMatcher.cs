using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Data;
using NZOR.Data.DataSets;

namespace NZOR.Matching.Routines
{
    public interface IReferenceMatcher
    {
        int Id
        {
            get;
            set;
        }
        int FailId
        {
            get;
            set;
        }
        int SuccessId
        {
            get;
            set;
        }
        int Threshold
        {
            get;
            set;
        }

        MatchTypeSelection MatchType
        {
            get;
            set;
        }

        MatchData MatchData
        {
            get;
            set;
        }
        
        DsReferenceMatch GetMatchingReferences(DsIntegrationReference.ProviderReferenceRow pr, ref string matchComments);
        void RemoveNonMatches(DsIntegrationReference.ProviderReferenceRow pr, ref DsReferenceMatch refs, ref string matchComments);
    }
}
