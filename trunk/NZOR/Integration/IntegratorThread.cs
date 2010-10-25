using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Data.SqlClient;

using NZOR.Data;

namespace NZOR.Integration
{
    public enum IntegrationOutcome
    {
        Added,
        Mathced,
        Failed
    }

    public class IntegratorThread
    {
        private SqlConnection _cnn = null;

        public Guid NameID = Guid.Empty;
        public ConfigSet Config;

        public Data.MatchResult Result = new NZOR.Data.MatchResult();

        public delegate void ProcessComplete(IntegratorThread it);
        public ProcessComplete ProcessCompleteCallback;

        public IntegratorThread()
        {
        }
                
        public IntegratorThread(Guid nameID, ConfigSet cs, String dbCnnStr)
        {
            NameID = nameID;
            Config = cs;
            _cnn = new SqlConnection(dbCnnStr);
        }

        public void ProcessName(Object stateInfo)
        {            
            if (Config != null)
            {
                _cnn.Open();
                DsIntegrationName provName = Data.ProviderName.GetNameMatchData(_cnn, NameID);
                Data.MatchResult res = Integrator.DoMatch(_cnn, provName, Config.Routines);

                if (res.Matches.Count == 0)
                {
                    //insert
                    DataSet newName = NZOR.Data.ConsensusName.AddConsensusName(_cnn, provName);
                    DataRow nameRow = newName.Tables[0].Rows[0];
                    NZOR.Data.ProviderName.UpdateProviderNameLink(_cnn, provName, NZOR.Data.LinkStatus.Inserted, (Guid?)nameRow["NameID"], 0, res.MatchPath);

                    res.MatchedId = nameRow["NameID"].ToString();
                    res.MatchedName = nameRow["FullName"].ToString();                    
                    res.Status = NZOR.Data.LinkStatus.Inserted;
                }
                else if (res.Matches.Count == 1)
                {
                    //link 
                    NZOR.Data.ProviderName.UpdateProviderNameLink(_cnn, provName, NZOR.Data.LinkStatus.Matched, res.Matches[0].NameId, res.Matches[0].MatchScore, res.MatchPath);
                    res.Status = NZOR.Data.LinkStatus.Matched;
                }
                else
                {
                    //multiple matches
                    NZOR.Data.ProviderName.UpdateProviderNameLink(_cnn, provName, NZOR.Data.LinkStatus.Multiple, null, 0, res.MatchPath);                    
                    res.Status = NZOR.Data.LinkStatus.Multiple;
                }

                Result = res;

                _cnn.Close();
            }

            if (ProcessCompleteCallback != null) ProcessCompleteCallback(this);
        }
    }
}
