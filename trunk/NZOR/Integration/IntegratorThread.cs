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
    public class IntegrationData
    {
        public bool UseDB = true;
        public string DBCnnStr = null;
        public DsIntegrationName NameData = null;
        public Guid NameID = Guid.Empty;
        public Guid ParentConsNameID = Guid.Empty;
        public ConfigSet Config = null;

        /// If useDB then the dbCnnStr DB is used to get the data and check for matches, otherwise all data for matching is contained in the nameData dataset, in memory.
        public IntegrationData(Guid nameID, Guid parentConsNameID, ConfigSet config, bool useDB, string dbCnnStr, DsIntegrationName data)
        {
            this.UseDB = useDB;
            this.DBCnnStr = dbCnnStr;
            this.NameData = data;
            this.NameID = nameID;
            this.ParentConsNameID = parentConsNameID;
            this.Config = config;
        }
    }

    public enum IntegrationOutcome
    {
        Added,
        Mathced,
        Failed
    }

    public class IntegratorThread
    {
        private SqlConnection _cnn = null;
        private List<IntegrationData> _data = new List<IntegrationData>();
        private Dictionary<Guid, MatchResult> _results = new Dictionary<Guid, MatchResult>();

        public delegate void ProcessComplete(IntegratorThread it, MatchResult result);
        public ProcessComplete ProcessCompleteCallback;

        public IntegratorThread()
        {
        }

        public List<IntegrationData> NameData
        {
            get
            {
                return _data;
            }
        }

        public void AddNameData(IntegrationData data)
        {
            _data.Add(data);            
        }

        public MatchResult Result(Guid provNameId)
        {
            MatchResult res = _results[provNameId];             
            return res;
        }

        public void ProcessName(Object stateInfo)
        {
            MatchResult result = new MatchResult();
            //get next data to use
            while (_data.Count > 0)
            {
                IntegrationData data = _data[0];
                _data.RemoveAt(0);

                if (data.Config != null)
                {
                    if (data.UseDB)
                    {
                        _cnn = new SqlConnection(data.DBCnnStr);
                        _cnn.Open();
                        DsIntegrationName provName = Data.ProviderName.GetNameMatchData(_cnn, data.NameID);
                        Data.MatchResult res = Integrator.DoMatch(_cnn, provName, data.Config.Routines);

                        if (res.Matches.Count == 0)
                        {
                            //insert
                            DataSet newName = NZOR.Data.ConsensusName.AddConsensusName(_cnn, provName.ProviderName[0]);
                            DataRow nameRow = newName.Tables[0].Rows[0];
                            NZOR.Data.ProviderName.UpdateProviderNameLink(_cnn, provName.ProviderName[0], NZOR.Data.LinkStatus.Inserted, (Guid?)nameRow["NameID"], 0, res.MatchPath);

                            res.MatchedId = nameRow["NameID"].ToString();
                            res.MatchedName = nameRow["FullName"].ToString();
                            res.Status = NZOR.Data.LinkStatus.Inserted;
                        }
                        else if (res.Matches.Count == 1)
                        {
                            //link 
                            NZOR.Data.ProviderName.UpdateProviderNameLink(_cnn, provName.ProviderName[0], NZOR.Data.LinkStatus.Matched, res.Matches[0].NameId, res.Matches[0].MatchScore, res.MatchPath);
                            res.Status = NZOR.Data.LinkStatus.Matched;
                        }
                        else
                        {
                            //multiple matches
                            NZOR.Data.ProviderName.UpdateProviderNameLink(_cnn, provName.ProviderName[0], NZOR.Data.LinkStatus.Multiple, null, 0, res.MatchPath);
                            res.Status = NZOR.Data.LinkStatus.Multiple;
                        }

                        result = res;
                        _results.Add(data.NameID, res);

                        _cnn.Close();
                    }
                    else
                    {
                        //non DB version 

                    }
                }
                
                if (ProcessCompleteCallback != null) ProcessCompleteCallback(this, result);

                result = new MatchResult();
            }
         
        }
    }
}
