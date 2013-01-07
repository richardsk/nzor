using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using NZOR.Data.DataSets;
using NZOR.Data;

namespace NZOR.Matching
{
    public class BaseMatcher : INameMatcher
    {
        private int _Id = -1;
        private int _FailId = -1;
        private int _SuccessId = -1;
        private int _Threshold = -1;
        private MatchTypeSelection _MatchType = NZOR.Data.MatchTypeSelection.Both;
        private String _cnnStr = null;
        private bool _useDBCnn = true;
        private MatchData _matchData = null;
        
        public BaseMatcher()
        {
        }

        public BaseMatcher(int id, int failId, int successId, int threshold, MatchTypeSelection matchType, MatchData matchData, bool useDBCnn, String cnnStr)
        {
            _Id = id;
            _FailId = failId;
            _SuccessId = successId;
            _Threshold = threshold;
            _MatchType = matchType;
            _cnnStr = cnnStr;
            _useDBCnn = useDBCnn;
            _matchData = matchData;
        }

        #region INameMatcher Members

        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public int FailId
        {
            get { return _FailId; }
            set { _FailId = value; }
        }

        public int SuccessId
        {
            get { return _SuccessId; }
            set { _SuccessId = value; }
        }
        
        public int Threshold
        {
            get { return _Threshold; }
            set { _Threshold = value; }
        }
        
        public MatchData MatchData
        {
            get { return _matchData; }
            set { _matchData = value; }
        }

        public NZOR.Data.MatchTypeSelection MatchType
        {
            get
            {
                return _MatchType;
            }
            set
            {
                _MatchType = value;
            }
        }

        public String DBConnectionString
        {
            get
            {
                return _cnnStr;
            }
            set
            {
                _cnnStr = value;
            }
        }

        public bool UseDBConnection
        {
            get { return _useDBCnn; }
            set { _useDBCnn = value; }
        }

        public virtual DsNameMatch GetMatchingNames(DsIntegrationName.ProviderNameRow pn, ref string matchComments)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveNonMatches(DsIntegrationName.ProviderNameRow pn, ref DsNameMatch names, ref string matchComments)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
