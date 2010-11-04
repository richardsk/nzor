using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Data.SqlClient;

using NZOR.Data;
using NZOR.Matching;

namespace NZOR.Integration
{
    public class IntegrationData
    {
        public bool UseDB = true;
        public string DBCnnStr = null;
        public Guid NameID = Guid.Empty;
        public String FullName = "";
        public Guid ParentConsNameID = Guid.Empty;
        public ConfigSet Config = null;
                
        /// If useDB then the dbCnnStr DB is used to get the data and check for matches, otherwise all data for matching is contained in the nameData dataset, in memory.
        public IntegrationData(Guid nameID, String fullName, Guid parentConsNameID, ConfigSet config, bool useDB, string dbCnnStr)
        {
            this.UseDB = useDB;
            this.DBCnnStr = dbCnnStr;
            this.NameID = nameID;
            this.FullName = fullName;
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
        private Dictionary<Guid, IntegrationData> _processedNames = new Dictionary<Guid, IntegrationData>();

        public delegate void ProcessComplete(IntegratorThread it, MatchResult result, Guid provNameID);
        public ProcessComplete ProcessCompleteCallback;

        public static System.IO.StreamWriter LogFile = null;

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
            lock (_data)
            {
                if (!_processedNames.ContainsKey(data.NameID)) //already processed ???
                    _data.Add(data);
            }
        }

        public MatchResult Result(Guid provNameId)
        {
            MatchResult res = _results[provNameId];             
            return res;
        }

        public IntegrationData GetProcessedNameData(Guid provNameID)
        {
            lock (_data)
            {
                IntegrationData id = _processedNames[provNameID];
                return id;
            }
        }

        public void ProcessName(Object stateInfo)
        {
            MatchResult result = new MatchResult();
            //get next data to use
            while (_data.Count > 0)
            {
                IntegrationData data = null;
                lock (_data)
                {
                    data = _data[0];
                    _processedNames.Add(data.NameID, data);
                    _data.Remove(data);
                }

                if (data.Config != null)
                {
                    if (data.UseDB)
                    {
                        _cnn = new SqlConnection(data.DBCnnStr);
                        _cnn.Open();
                        DsIntegrationName.ProviderNameRow provName = Data.ProviderName.GetNameMatchData(_cnn, data.NameID);
                        Data.MatchResult res = MatchProcessor.DoMatch(provName, data.Config.Routines, data.UseDB, _cnn);

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

                        result = res;
                        _results.Add(data.NameID, res);

                        _cnn.Close();
                    }
                    else
                    {
                        //non DB version 
                        DsIntegrationName.ProviderNameRow provName = null;
                        lock (MatchData.DataForIntegration)
                        {
                            DataRow[] names = MatchData.DataForIntegration.ProviderName.Select("NameID = '" + data.NameID.ToString() + "'");
                            if (names.Count() > 0)
                            {
                                provName = (DsIntegrationName.ProviderNameRow)names[0];
                            }
                        }

                        if (provName != null)
                        {
                            Data.MatchResult res = MatchProcessor.DoMatch(provName, data.Config.Routines, false, null);
                            
                            if (res.Matches.Count == 0)
                            {
                                //insert
                                if (provName.IsParentConsensusNameIDNull() || provName.ParentConsensusNameID == Guid.Empty)
                                {
                                    //dont know where to put it
                                    res.Status = LinkStatus.DataFail;
                                    lock (MatchData.DataForIntegration)
                                    {
                                        provName.LinkStatus = LinkStatus.DataFail.ToString();
                                        provName["ConsensusNameID"] = DBNull.Value;
                                        provName.MatchPath = res.MatchPath;
                                        provName["MatchScore"] = DBNull.Value;
                                        provName["ParentConsensusNameID"] = DBNull.Value;
                                    }

                                    if (LogFile != null) LogFile.WriteLine("ERROR : Integration failed for name '" + provName.NameID.ToString() + "', " + provName.FullName + ".  Not enough parent taxon information.");
                                }
                                else
                                {
                                    res.Status = LinkStatus.Inserted;

                                    Guid newId = Guid.NewGuid();
                                    //basionym
                                    object basID = DBNull.Value;
                                    DataRow[] bas = MatchData.DataForIntegration.ProviderName.Select("NameID = '" + provName.BasionymID.ToString() + "'");
                                    if (bas.Length > 0) basID = bas[0]["ConsensusNameID"];

                                    lock (MatchData.DataForIntegration)
                                    {
                                        MatchData.DataForIntegration.ConsensusName.Rows.Add(newId, provName.FullName, provName.NameClassID, provName.NameClass, provName.TaxonRankID, provName.TaxonRank,
                                            provName.TaxonRankSort, provName["Authors"], provName.GoverningCode, provName.Canonical, provName["YearOnPublication"], basID, provName["Basionym"], provName["BasionymAuthors"],
                                            provName["CombinationAuthors"], provName["MicroReference"], provName["PublishedIn"], provName.ParentConsensusNameID, provName["ParentConsensusNameID"].ToString(), provName.Parent,
                                            provName["PreferredConsensusNameID"], provName["PreferredName"]);

                                        provName.ConsensusNameID = newId;
                                        provName.MatchPath = res.MatchPath;
                                        provName.LinkStatus = LinkStatus.Inserted.ToString();

                                        NZOR.Data.ConsensusName.RefreshConsensusData(newId, MatchData.DataForIntegration);
                                    }

                                    res.MatchedName = provName.FullName;
                                    res.MatchedId = newId.ToString();

                                }
                            }
                            else if (res.Matches.Count == 1)
                            {
                                res.Status = LinkStatus.Matched;
                                res.MatchedId = res.Matches[0].NameId.Value.ToString();
                                res.MatchedName = res.Matches[0].NameFull;

                                lock (MatchData.DataForIntegration)
                                {
                                    provName.ConsensusNameID = res.Matches[0].NameId.Value;
                                    provName.MatchPath = res.MatchPath;
                                    provName.LinkStatus = LinkStatus.Matched.ToString();

                                    NZOR.Data.ConsensusName.RefreshConsensusData(provName.ConsensusNameID, MatchData.DataForIntegration);
                                }
                            }
                            else
                            {
                                res.Status = LinkStatus.Multiple;
                                lock (MatchData.DataForIntegration)
                                {
                                    provName.LinkStatus = LinkStatus.Multiple.ToString();
                                    provName["ConsensusNameID"] = DBNull.Value;
                                    provName.MatchPath = res.MatchPath;
                                    provName["MatchScore"] = DBNull.Value;
                                    provName["ParentConsensusNameID"] = DBNull.Value;
                                }
                            }

                            result = res;
                            _results.Add(data.NameID, res);
                        }
                    }
                }
                
                if (ProcessCompleteCallback != null) ProcessCompleteCallback(this, result, data.NameID);

                result = new MatchResult();
            }
         
        }
    }
}
