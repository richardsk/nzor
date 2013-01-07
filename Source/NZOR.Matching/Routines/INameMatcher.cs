using NZOR.Data;
using NZOR.Data.DataSets;

namespace NZOR.Matching
{
    public interface INameMatcher
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

        string DBConnectionString
        {
            get;
            set;
        }

        bool UseDBConnection
        {
            get;
            set;
        }

        DsNameMatch GetMatchingNames(DsIntegrationName.ProviderNameRow pn, ref string matchComments);
        void RemoveNonMatches(DsIntegrationName.ProviderNameRow pn, ref DsNameMatch names, ref string matchComments);
    }
}
