using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Matching
{
    public class BaseMatcher : INameMatcher
    {
        private int m_Id = -1;
        private int m_FailId = -1;
        private int m_SuccessId = -1;
        private int m_Threshold = -1;
        private NZOR.Data.MatchTypeSelection m_MatchType = NZOR.Data.MatchTypeSelection.Both;

        public BaseMatcher()
        {
        }

        public BaseMatcher(int id, int failId, int successId, int threshold, NZOR.Data.MatchTypeSelection matchType)
        {
            m_Id = id;
            m_FailId = failId;
            m_SuccessId = successId;
            m_Threshold = threshold;
            m_MatchType = matchType;
        }

        #region INameMatcher Members

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

        public NZOR.Data.MatchTypeSelection MatchType
        {
            get
            {
                return m_MatchType;
            }
            set
            {
                m_MatchType = value;
            }
        }

        public virtual NZOR.Data.DsNameMatch GetMatchingNames(System.Data.DataSet pn, ref string matchComments)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveNonMatches(System.Data.DataSet pn, ref NZOR.Data.DsNameMatch names, ref string matchComments)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
