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

        DsNameMatch GetMatchingNames(System.Data.DataSet pn);
        void RemoveNonMatches(System.Data.DataSet pn, ref DsNameMatch names);
    }
}
