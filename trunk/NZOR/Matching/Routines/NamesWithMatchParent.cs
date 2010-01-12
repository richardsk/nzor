using System.Data.SqlClient;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithMatchParent : INameMatcher
    {

        private int m_Id = -1;
        private int m_FailId = -1;
        private int m_SuccessId = -1;
        private int m_Threshold = -1;

        public NamesWithMatchParent()
        {
        }

        public NamesWithMatchParent(int id, int failId, int successId, int threshold)
        {
            m_Id = id;
            m_FailId = failId;
            m_SuccessId = successId;
            m_Threshold = threshold;
        }

        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public int FailId
        {
            get { return m_FailId; }
            set { m_FailId = value; }
        }

        public int SuccessId
        {
            get { return m_SuccessId; }
            set { m_SuccessId = value; }
        }

        public int Threshold
        {
            get { return m_Threshold; }
            set { m_Threshold = value; }
        }


        public DsNameMatch GetMatchingNames(System.Data.DataSet pn)
        {
            //not done - this routine only adds names
            return null;
        }

        public void RemoveNonMatches(System.Data.DataSet pn, ref DsNameMatch names)
        {
            //this routine only adds names, so add names under the "match" parent
            //TODO
        }

    }
}
