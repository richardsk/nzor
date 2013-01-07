using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Xml;

using NZOR.Data;
using NZOR.Matching;
using NZOR.Data.Entities.Common;
using NZOR.Validation;
using NZOR.Data.DataSets;

namespace NZOR.Integration.Other
{
    public class IntegrationProcessor 
    {
        public List<MatchResult> Results = new List<MatchResult>();
        public string ConnectionString = null;

        public string StatusText = "";
        public int MaxThreads = 20;
        public int Progress = 0;

        private Guid _thisBatchID = Guid.Empty;
        private int _maxToProcess = -1;
        private int _namesToProcessCount = 0;
        private int _refsToProcessCount = 0;
        private List<IntegratorThread> _threads = new List<IntegratorThread>();
        private MatchData _matchData = null;
        private int _processedNamesCount = 0;
        private int _processedRefsCount = 0;
        private List<NZOR.Admin.Data.Entities.AttachmentPoint> _attachmentPoints = null;
        private bool _integrationCompleted = false;

        private static object lockKey = new object();

        public event EventHandler ProcessingComplete;

        /// <summary>
        /// Use multiple threads to process records that need integrating
        /// Individual threads will need to work with names that dont overlap with names in other threads - by working with names in different parts of the taxonomic hierarchy.
        ///   i.e. get the next provider name that has it's parent already integrated and there are no siblings of this name currenlty being integrated
        /// </summary>
        public void RunIntegration(XmlDocument config, int maxRecords)
        {
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            bool doAnother = true;
            
            _thisBatchID = Guid.NewGuid();
            _namesToProcessCount = GetNamesForIntegrationCount(ConnectionString);
            _refsToProcessCount = GetReferencesForIntegrationCount(ConnectionString);
            Progress = 1; //started

            MatchProcessor.LoadConfig(config);
            
            if (MaxThreads != -1 && MaxThreads < 100) ThreadPool.SetMaxThreads(MaxThreads, 100);

            _matchData = new MatchData(false, false, null, null, null);

            NZOR.Admin.Data.Repositories.IProviderRepository pr = new NZOR.Admin.Data.Sql.Repositories.ProviderRepository(ConnectionString);
            _attachmentPoints = pr.GetAllAttachmentPoints();
            
            _maxToProcess = _namesToProcessCount + _refsToProcessCount;

            //max records?
            if (maxRecords > 0 && maxRecords < _maxToProcess) _maxToProcess = maxRecords;

            while (doAnother && !_integrationCompleted) 
            {
                //do references first

                while (ProcessNextReference(ConnectionString) && doAnother)
                {
                    _processedRefsCount++;
                    StatusText = "Processed " + _processedRefsCount.ToString() + " of " + _refsToProcessCount.ToString() + " references.";
                    Progress = (Results.Count * 100 / (_namesToProcessCount + _refsToProcessCount));

                    doAnother = !CheckForCompletion();
                }

                if (doAnother)
                {
                    doAnother = ProcessNextName(ConnectionString);
                    if (doAnother) doAnother = !CheckForCompletion();
                }
            }
                               
        }

        private bool ProcessNextReference(String cnnStr)
        {
            DsIntegrationReference.ProviderReferenceRow nextRef = GetNextReferenceForIntegration(cnnStr);
            if (nextRef != null)
            {
                MatchResult res = MatchProcessor.DoMatchReference(_matchData, nextRef, true, ConnectionString);
                if (res.Matches.Count == 0)
                {
                    //insert
                    res.Status = LinkStatus.Inserted;
                    
                    DataSet newRef = Data.Sql.Integration.AddConsensusReference(cnnStr, nextRef);

                    nextRef.ConsensusReferenceID = (Guid)newRef.Tables[0].Rows[0]["ReferenceID"];
                    nextRef.MatchPath = res.MatchPath;
                    nextRef.LinkStatus = LinkStatus.Inserted.ToString();

                    res.MatchedReference = nextRef.Citation;
                    res.MatchedId = nextRef.ConsensusReferenceID.ToString();

                    NZOR.Data.Sql.Integration.UpdateProviderReferenceLink(cnnStr, nextRef, NZOR.Data.LinkStatus.Inserted, nextRef.ConsensusReferenceID, 0, res.MatchPath, _thisBatchID);
                }
                else if (res.Matches.Count == 1)
                {                    
                    NZOR.Data.Sql.Integration.UpdateProviderReferenceLink(cnnStr, nextRef, NZOR.Data.LinkStatus.Matched, res.Matches[0].Id, res.Matches[0].MatchScore, res.MatchPath, _thisBatchID);
                    res.Status = NZOR.Data.LinkStatus.Matched;
                    res.MatchedId = res.Matches[0].Id.ToString();
                    res.MatchedReference = nextRef.Citation;

                    NZOR.Data.Sql.Integration.RefreshConsensusReference(res.Matches[0].Id.Value, cnnStr);
                }
                else
                {
                    res.Status = LinkStatus.Multiple;
                    
                    NZOR.Data.Sql.Integration.UpdateProviderReferenceLink(cnnStr, nextRef, NZOR.Data.LinkStatus.Multiple, null, 0, res.MatchPath, _thisBatchID);
                }
                                
                lock (lockKey)
                {
                    Results.Add(res);
                }

                return true;
            }

            return false;
        }

        private bool ProcessNextName(String cnnStr)
        {
            bool more = true;

            string fullName = "";
            List<Guid> parConsNameIDs = new List<Guid>();
            int setId = -1;
            Guid nextName = GetNextNameForIntegration(cnnStr, ref fullName, ref parConsNameIDs, ref setId);
            if (nextName != Guid.Empty)
            {
                StatusText = "Processed " + _processedNamesCount.ToString() + " of " + _namesToProcessCount.ToString() + " names.  Number of running threads = " + (_threads.Count + 1).ToString();
                IntegratorThread.Log.Add(DateTime.Now.ToString() + " : Processing name ID=" + nextName.ToString());

                _processedNamesCount += 1;

                ConfigSet cs = MatchProcessor.GetMatchSet(setId);
                                
                IntegrationData data = new IntegrationData(nextName, fullName, parConsNameIDs, cs, true, ConnectionString, _thisBatchID, _attachmentPoints);

                //validate
                //get the parent name information
                bool fail = false;
                string msg = "";

                try
                {
                    if (setId == -1)
                    {
                        fail = true;
                        msg = "SYSTEM ERROR : No match rule set defined for taxon rank of name " + nextName.ToString();
                    }
                    else
                    {
                        DsIntegrationName.ProviderNameRow provName = Data.Sql.Integration.GetNameMatchData(cnnStr, nextName, _attachmentPoints);
                        data.ProviderName = provName;

                        //validate provider data
                        Integration.IntegrationValidation iv = new IntegrationValidation();
                        ValidationResultLookup valRes = iv.ValidateProviderData(provName);

                        if (!valRes.AllClear())
                        {
                            fail = true;
                            msg = "ERROR : Validation of provider name (id = " + nextName.ToString() + ") failed." + valRes.ErrorMessages();
                        }

                    }
                }
                catch (Exception ex)
                {
                    fail = true;
                    msg = ex.Message;
                }

                if (fail)
                {
                    using (SqlConnection cnn = new SqlConnection(cnnStr))
                    {
                        cnn.Open();
                        Data.Sql.Integration.UpdateProviderNameLink(cnn, nextName, NZOR.Data.LinkStatus.DataFail, null, 0, msg, _thisBatchID);
                    }
                    
                    IntegratorThread.Log.Add(DateTime.Now.ToString() + " : " + msg);
                    IntegratorThread.Log.Add(DateTime.Now.ToString() + ": ERROR : Integration failed for name '" + nextName.ToString() + "'.");
                    MatchResult mr = new MatchResult();
                    mr.ProviderRecordId = data.ProviderName.ProviderRecordID;
                    mr.MatchedId = nextName.ToString();
                    mr.MatchedName = data.ProviderName.FullName;
                    mr.MatchPath = msg;
                    mr.Status = LinkStatus.DataFail;

                    lock (lockKey)
                    {
                        Results.Add(mr);
                    }
                }
                else
                {
                    //if this name has the same parent as another name being processed, then use that thread
                    bool process = true;

                    lock (lockKey)
                    {
                        if (_integrationCompleted) //in case it has just ended
                        {
                            more = false;
                        }
                        else
                        {
                            foreach (IntegratorThread th in _threads)
                            {
                                if (th.State != IntegratorThread.ProcessingState.Finished)
                                {
                                    th.ThreadDataMutex.WaitOne();
                                    foreach (IntegrationData id in th.NameData)
                                    {
                                        if (id.ParentConsNameIDs.Intersect(parConsNameIDs).Count() > 0 || (id.ParentConsNameIDs.Count == 0 && parConsNameIDs.Count == 0))
                                        {
                                            th.ThreadDataMutex.ReleaseMutex();
                                            th.AddNameData(data);
                                            process = false;
                                            break;
                                        }
                                    }
                                    if (process) th.ThreadDataMutex.ReleaseMutex();
                                }
                                if (!process) break;
                            }


                            if (process)
                            {
                                IntegratorThread it = new IntegratorThread(_matchData, true, _thisBatchID, -1);
                                it.AddNameData(data);
                                it.NameProcessedCallback = new IntegratorThread.NameProcessed(ProcessComplete);
                                ThreadPool.QueueUserWorkItem(new WaitCallback(it.ProcessNameProc));
                                _threads.Add(it);

                                //check threads                    
                                int numTh = 0;
                                int numOtherTh = 0;
                                ThreadPool.GetAvailableThreads(out numTh, out numOtherTh);

                                if (numTh < 2 || _threads.Count >= MaxThreads) more = false; //leave at least 1 thread ??    
                            }
                        }
                    }

                }
            }
            else
            {
                lock (lockKey)
                {
                    int thCount = _threads.Count;

                    //need to check for integration stall - ie when there are still names to integrate but all the parents have failed to integrate, so we cannot progress
                    if (Progress < 100 && !_integrationCompleted && thCount == 0)  //ie there are no names to process and there are no threads processing names - so all done!
                    {
                        _integrationCompleted = true;
                        Progress = 100;
                    }
                }
                more = false;
            }

            return more;
        }

        private bool CheckForCompletion()
        {
            lock (lockKey)
            {
                if (_integrationCompleted)
                {
                    return true;
                }


                int prog = 1;
                prog = (Results.Count * 100 / (_namesToProcessCount + _refsToProcessCount));

                if (prog == 0) prog = 1; //at least to indicate we have started
                if (prog >= 100 || Results.Count >= _maxToProcess)
                {
                    if (_maxToProcess > Results.Count)
                    {
                        prog = 99; //not 100 % complete until ALL names are done
                    }
                    else
                    {
                        if (_threads.Count == 0) //all threads done
                        {
                            _integrationCompleted = true;
                            prog = 100;
                        }
                        else
                        {
                            prog = 99; //not quite done
                            //abort threads
                            foreach (IntegratorThread it in _threads)
                            {
                                it.StopThread = true;
                            }
                        }
                    }
                }

                Progress = prog;
            }

            if (_integrationCompleted && ProcessingComplete != null) ProcessingComplete(null, EventArgs.Empty);

            return _integrationCompleted;
        }

        private void ProcessComplete(IntegratorThread it, IntegrationData data, MatchResult result)
        {
            try
            {
                if (result.Error != null) throw new Exception(result.Error);
                if (_integrationCompleted) return;

                int prog = 1;

                Results.Add(result);
                prog = (Results.Count * 100 / _namesToProcessCount);

                StatusText = "Processed " + _processedNamesCount.ToString() + " of " + _namesToProcessCount.ToString() + " names.  Number of running threads = " + _threads.Count.ToString();

                IntegratorThread.Log.Add(DateTime.Now.ToString() + " : " + data.FullName + " : " + data.NameID.ToString() + " : RESULT = " + result.Status.ToString() + ", " + result.MatchedName + ", " + result.MatchedId);

                Progress = prog;

                //more names on this thread?
                if (it.State == IntegratorThread.ProcessingState.Finished)
                {
                    lock (lockKey)
                    {
                        _threads.Remove(it);
                    }
                }

                if (!CheckForCompletion() && Results.Count < _maxToProcess && !_integrationCompleted)
                {
                    //spawn more threads?
                    int numTh = 0;
                    int numOtherTh = 0;
                    ThreadPool.GetAvailableThreads(out numTh, out numOtherTh);

                    if (numTh > 0 && _threads.Count < MaxThreads)
                    {
                        bool doAnother = ProcessNextName(ConnectionString);
                        while (doAnother && _threads.Count < 3 && _threads.Count < MaxThreads && !_integrationCompleted)
                        {
                            doAnother = ProcessNextName(ConnectionString);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    IntegratorThread.Log.Add(DateTime.Now.ToString() + " : ERROR : " + ex.Message + " : " + ex.StackTrace);
                }
                catch (Exception) { }
            }
        }


        private Guid GetNextNameForIntegration(String cnnStr, ref string fullName, ref List<Guid> parentConsNameIDs, ref int matchRuleSetId)
        {
            Guid id = Guid.Empty;

            if (_integrationCompleted) return id;

            lock (lockKey)
            {

                using (SqlConnection cnn = new SqlConnection(cnnStr))
                {
                    cnn.Open();
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        //cmd.CommandText = "select top 1 n.NameID, n.FullName, pc.ConsensusNameToID, tr.MatchRuleSetID " +
                        //        "from provider.Name n " +
                        //        "inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID " +
                        //        "left join provider.vwConcepts pc on pc.NameID = n.NameID and pc.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and InUse = 1 " +
                        //        "where n.ConsensusNameID is null and (n.IntegrationBatchID is null or n.IntegrationBatchID <> '" + _thisBatchID.ToString() + "') and " +
                        //        " (n.LinkStatus is null or n.LinkStatus <> 'Integrating') " +
                        //        "order by tr.SortOrder ";

                        cmd.CommandText = @"set concat_null_yields_null off

                        select top 1 pn.NameID, pn.FullName, tr.MatchRuleSetID,
                            c.par as ParentNames 
                        from provider.Name pn
                        inner join dbo.TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
                        inner join dbo.NameClass nc on nc.NameClassID = pn.NameClassID
                        left join provider.vwConcepts par on par.NameID = pn.NameID and par.ConsensusNameToID is null and par.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155'
                        CROSS APPLY 
                        ( 
                            select (SELECT distinct CONVERT(VARCHAR(38), pc.ConsensusNameToID) + '|' AS [text()] 
                            FROM provider.vwConcepts pc 
                            where pc.NameID = pn.NameID 
		                        and pc.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155'
                            FOR XML PATH(''))
                        ) C (par)
                        where pn.ConsensusNameID is null and (pn.IntegrationBatchID is null or pn.IntegrationBatchID <> '" + _thisBatchID.ToString() + @"') and 
                            (pn.LinkStatus is null or pn.LinkStatus <> 'Integrating') and (par.NameID is null or par.ConsensusNameToID is not null)
                        order by nc.Name, tr.SortOrder;";

                        DataSet res = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(res);
                        if (res != null && res.Tables.Count > 0 && res.Tables[0].Rows.Count > 0)
                        {
                            id = (Guid)res.Tables[0].Rows[0]["NameId"];
                            fullName = res.Tables[0].Rows[0]["FullName"].ToString();
                            matchRuleSetId = (int)res.Tables[0].Rows[0]["MatchRuleSetID"];

                            if (!res.Tables[0].Rows[0].IsNull("ParentNames"))
                            {
                                string[] parents = res.Tables[0].Rows[0]["ParentNames"].ToString().Split('|');
                                foreach (string par in parents)
                                {
                                    if (par != string.Empty) parentConsNameIDs.Add(new Guid(par));
                                }
                            }

                            //if (!res.Tables[0].Rows[0].IsNull("ConsensusNameToID")) parentConsNameIDs.Add((Guid)res.Tables[0].Rows[0]["ConsensusNameToID"]);
                        }

                    }

                    if (id != Guid.Empty)
                    {
                        using (SqlCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = "update provider.Name set LinkStatus = 'Integrating' where NameID = '" + id.ToString() + "'";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            return id;
        }

        private DsIntegrationReference.ProviderReferenceRow GetNextReferenceForIntegration(String cnnStr)
        {
            DsIntegrationReference.ProviderReferenceRow pr = null;

            lock (lockKey)
            {

                using (SqlConnection cnn = new SqlConnection(cnnStr))
                {
                    cnn.Open();
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = @"select top 1 r.ReferenceID,
	                        r.ReferenceTypeID,
	                        r.DataSourceID,
	                        r.ConsensusReferenceID,
	                        r.LinkStatus,
	                        r.MatchScore,
	                        rp.Value as Citation,
	                        r.ProviderRecordID,
	                        props.val as Properties
                        from provider.Reference r
                        left join provider.ReferenceProperty rp on rp.ReferenceID = r.ReferenceID and rp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07' --citation
                        cross apply
                        (
                            select (SELECT distinct '[' + CONVERT(VARCHAR(38), rp.ReferencePropertyTypeID) + ':' + rp.Value + '],' AS [text()] 
                            FROM provider.ReferenceProperty rp 
                            where rp.ReferenceID = r.ReferenceID
                            FOR XML PATH(''))
                        ) props(val)
                        where r.ConsensusReferenceID is null and (r.LinkStatus is null or r.LinkStatus <> 'Integrating')";

                        DsIntegrationReference ds = new DsIntegrationReference();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.TableMappings.Add("Table", ds.ProviderReference.TableName);
                        da.Fill(ds);
                        if (ds != null && ds.ProviderReference.Count > 0)
                        {
                            pr = ds.ProviderReference[0];
                        }

                    }

                    if (pr != null)
                    {
                        using (SqlCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = "update provider.Reference set LinkStatus = 'Integrating' where ReferenceID = '" + pr.ReferenceID.ToString() + "'";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            return pr;
        }

        private int GetNamesForIntegrationCount(string cnnStr)
        {
            int cnt = 0;

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select count(nameid) from provider.Name where ConsensusNameId is null";

                    object val = cmd.ExecuteScalar();
                    if (val != DBNull.Value) cnt = (int)val;
                }
            }

            return cnt;
        }

        private int GetReferencesForIntegrationCount(string cnnStr)
        {
            int cnt = 0;

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select count(referenceid) from provider.Reference where ConsensusReferenceId is null";

                    object val = cmd.ExecuteScalar();
                    if (val != DBNull.Value) cnt = (int)val;
                }
            }

            return cnt;
        }

        
    }
}
