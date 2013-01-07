using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace NZOR.Matching
{
    public class BaseMatcher : INameMatcher
    {
        private int _Id = -1;
        private int _FailId = -1;
        private int _SuccessId = -1;
        private int _Threshold = -1;
        private NZOR.Data.MatchTypeSelection _MatchType = NZOR.Data.MatchTypeSelection.Both;
        private SqlConnection _cnn = null;
        private bool _useDBCnn = true;
        
        public BaseMatcher()
        {
        }

        public BaseMatcher(int id, int failId, int successId, int threshold, NZOR.Data.MatchTypeSelection matchType, bool useDBCnn, SqlConnection cnn)
        {
            _Id = id;
            _FailId = failId;
            _SuccessId = successId;
            _Threshold = threshold;
            _MatchType = matchType;
            _cnn = cnn;
            _useDBCnn = useDBCnn;
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

        public SqlConnection DBConnection
        {
            get
            {
                return _cnn;
            }
            set
            {
                _cnn = value;
            }
        }

        public bool UseDBConnection
        {
            get { return _useDBCnn; }
            set { _useDBCnn = value; }
        }

        public virtual NZOR.Data.DsNameMatch GetMatchingNames(NZOR.Data.DsIntegrationName.ProviderNameRow pn, ref string matchComments)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveNonMatches(NZOR.Data.DsIntegrationName.ProviderNameRow pn, ref NZOR.Data.DsNameMatch names, ref string matchComments)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
