using NZOR.Data;

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

        System.Data.SqlClient.SqlConnection DBConnection
        {
            get;
            set;
        }

        bool UseDBConnection
        {
            get;
            set;
        }

        DsNameMatch GetMatchingNames(NZOR.Data.DsIntegrationName.ProviderNameRow pn, ref string matchComments);
        void RemoveNonMatches(NZOR.Data.DsIntegrationName.ProviderNameRow pn, ref DsNameMatch names, ref string matchComments);
    }
}
