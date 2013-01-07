using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Xml;

using NZOR.Data;
using NZOR.Data.DataSets;
using NZOR.Data.Entities.Common;
using NZOR.Data.Entities.Integration;
using NZOR.Matching;
using NZOR.Validation;

namespace NZOR.Integration
{
    public class MemoryIntegrationProcessor
    {
        public List<MatchResult> Results = new List<MatchResult>();

        public string StatusText = "";
        public int MaxThreads = 25;
        public int Progress = 0;

        private Guid _thisBatchID = Guid.Empty;
        private List<IntegratorThread> _threads = new List<IntegratorThread>();
        private int _maxToProcess = -1;
        private int _namesToProcessCount = -1;
        private int _refsToProcessCount = -1;
        private Dictionary<Guid, ValidationResultLookup> _nameValidations = new Dictionary<Guid,ValidationResultLookup>();
        private int _processedNamesCount = 0;
        private int _processedRefsCount = 0;
        private string _dataFilePath = "";
        List<Admin.Data.Entities.AttachmentPoint> _attPoints = null;

        private bool _integrationCompleted = false;

        private MatchData _matchData = null;
                
        public event EventHandler ProcessingComplete;

        /// <summary>
        /// Use multiple threads to process records that need integrating
        /// Individual threads will need to work with names that dont overlap with names in other threads - by working with names in different parts of the taxonomic hierarchy.
        ///   i.e. get the next provider name that has it's parent already integrated and there are no siblings of this name currenlty being integrated
        /// An optional maxRecords parameter can be passed in to limit the number of records processed (-1 or 0 for unlimited).
        /// </summary>
        public void RunIntegration(string configFilePath, string dataFilePath, int maxRecords, List<Admin.Data.Entities.AttachmentPoint> attPoints)
        {
            bool doAnother = true;

            _dataFilePath = dataFilePath;
            _thisBatchID = Guid.NewGuid();
            _attPoints = attPoints;

            IntegratorThread.Log = new List<string>();
            IntegratorThread.Log.Add(DateTime.Now.ToString() + " : Loading dataset file.");

            DataForIntegration intData = NZOR.Data.Sql.Integration.LoadDataFile(dataFilePath);
            _matchData = new MatchData(true, true, intData, intData.SingleNamesSet, intData.References);
                        
            //get count at start
            NamesForIntegrationCount();
            ReferencesForIntegrationCount();

            _maxToProcess = _namesToProcessCount + _refsToProcessCount;
            
            //max records?
            if (maxRecords > 0 && maxRecords < _maxToProcess) _maxToProcess = maxRecords;

            IntegratorThread.Log.Add(DateTime.Now.ToString() + " : Number of references to integrate = " + _refsToProcessCount.ToString());
            IntegratorThread.Log.Add(DateTime.Now.ToString() + " : Number of names to integrate = " + _namesToProcessCount.ToString());

            Progress = 1; //started

            XmlDocument config = new XmlDocument();
            config.Load(configFilePath);
            MatchProcessor.LoadConfig(config);

            if (MaxThreads != -1 && MaxThreads < 1000) ThreadPool.SetMaxThreads(MaxThreads, 1000);
            
            while (doAnother)
            {
                //do references first

                while (ProcessNextReference())
                {
                    _processedRefsCount++;
                    StatusText = "Processed " + _processedRefsCount.ToString() + " of " + ReferencesForIntegrationCount().ToString() + " references.";
                    Progress = (Results.Count * 100 / (_namesToProcessCount + _refsToProcessCount));

                    doAnother = !CheckForCompletion();
                }

                if (doAnother) doAnother = ProcessNextName();                
            }
        }

        private bool ProcessNextReference()
        {
            DsIntegrationReference.ProviderReferenceRow nextRef = GetNextReferenceForIntegration();
            if (nextRef != null)
            {
                MatchResult res = MatchProcessor.DoMatchReference(_matchData, nextRef, false, string.Empty);
                if (res.Matches.Count == 0)
                {
                    //insert
                    res.Status = LinkStatus.Inserted;

                    Guid newId = Guid.NewGuid();

                    _matchData.GetProviderDataLock();

                    nextRef.ConsensusReferenceID = newId;
                    nextRef.MatchPath = res.MatchPath;
                    nextRef.LinkStatus = LinkStatus.Inserted.ToString();

                    _matchData.ReleaseProviderDataLock();

                    _matchData.GetConsensusDataLock();

                    _matchData.AllData.ConsensusData.ConsensusReference.AddConsensusReferenceRow(newId, nextRef.ReferenceTypeID, nextRef["Citation"].ToString(), nextRef["Properties"].ToString());
                    
                    NZOR.Data.Sql.Integration.RefreshConsensusReferenceData(newId, _matchData.ReferencesIntegrationSet, _matchData.AllData.ConsensusData); 

                    res.MatchedReference = nextRef.Citation;
                    res.MatchedId = newId.ToString();

                    _matchData.ReleaseConsensusDataLock();
                }
                else if (res.Matches.Count == 1)
                {
                    res.Status = LinkStatus.Matched;
                    res.MatchedId = res.Matches[0].Id.Value.ToString();
                    res.MatchedReference = res.Matches[0].DisplayText;

                    _matchData.GetProviderDataLock();

                    nextRef.ConsensusReferenceID = res.Matches[0].Id.Value;
                    nextRef.MatchPath = res.MatchPath;
                    nextRef.LinkStatus = LinkStatus.Matched.ToString();
                    nextRef.MatchScore = res.Matches[0].MatchScore;

                    _matchData.ReleaseProviderDataLock();

                    _matchData.GetConsensusDataLock();

                    NZOR.Data.Sql.Integration.RefreshConsensusReferenceData(nextRef.ConsensusReferenceID, _matchData.ReferencesIntegrationSet, _matchData.AllData.ConsensusData);

                    _matchData.ReleaseConsensusDataLock();
                }
                else
                {
                    res.Status = LinkStatus.Multiple;

                    _matchData.GetProviderDataLock();

                    nextRef.LinkStatus = LinkStatus.Multiple.ToString();
                    nextRef["ConsensusReferenceID"] = DBNull.Value;
                    nextRef.MatchPath = res.MatchPath;
                    nextRef["MatchScore"] = DBNull.Value;

                    _matchData.ReleaseProviderDataLock();
                }

                Results.Add(res);

                return true;
            }

            return false;
        }

        private bool ProcessNextName()
        {
            bool more = true;

            DsIntegrationName.ProviderNameRow nextName = GetNextNameForIntegration();
            if (nextName != null)
            {
                StatusText = "Processed " + _processedNamesCount.ToString() + " of " + NamesForIntegrationCount().ToString() + " names.  Number of running threads = " + (_threads.Count + 1).ToString();
                IntegratorThread.Log.Add(DateTime.Now.ToString() + " : Processing name ID=" + nextName.NameID.ToString());

                _processedNamesCount += 1;

                //validate
                //get the parent name information
                bool fail = false;
                string msg = "";

                _matchData.GetAllDataLock();
                try
                {
                    if (nextName.IsMatchRuleSetIDNull())
                    {
                        fail = true;
                        msg = "SYSTEM ERROR : No match rule set defined for taxon rank '" + nextName.TaxonRank + "', " + nextName.TaxonRankID.ToString();
                    }
                    else
                    {
                        
                        //validate provider data
                        ValidationResultLookup valRes = null;
                        if (_nameValidations.ContainsKey(nextName.NameID)) valRes = _nameValidations[nextName.NameID];
                        if (valRes == null)
                        {
                            Integration.IntegrationValidation iv = new IntegrationValidation();
                            valRes = iv.ValidateProviderData(nextName);
                        }
                        if (!valRes.AllClear())
                        {
                            fail = true;
                            msg = "ERROR : Validation of provider name (id = " + nextName["NameID"].ToString() + ") failed." + valRes.ErrorMessages();
                        }

                    }
                }
                catch (Exception ex)
                {
                    fail = true;
                    msg = ex.Message;
                }
                _matchData.ReleaseAllDataLock();
                
                if (fail)
                {
                    _matchData.GetProviderDataLock();

                    nextName.LinkStatus = LinkStatus.DataFail.ToString();
                    nextName.IntegrationBatchID = _thisBatchID;
                    nextName.MatchPath = msg;
                    IntegratorThread.Log.Add(DateTime.Now.ToString() + " : " + msg);
                    IntegratorThread.Log.Add(DateTime.Now.ToString() + ": ERROR : Integration failed for name '" + nextName.NameID.ToString() + "', " + nextName.FullName + ".");
                    MatchResult mr = new MatchResult();
                    mr.ProviderRecordId = nextName.ProviderRecordID;
                    mr.MatchPath = msg;
                    mr.Status = LinkStatus.DataFail;
                    Results.Add(mr);

                    _matchData.ReleaseProviderDataLock();
                    
                    CheckForCompletion();
                }
                else
                {
                    ConfigSet cs = MatchProcessor.GetMatchSet(nextName.MatchRuleSetID);
                    List<Guid> parentConsNameIDs = new List<Guid>();
                    if (nextName.HasClassification) parentConsNameIDs = Data.Sql.Integration.GetParentConsensusNameIDs(nextName);
                    
                    IntegrationData data = new IntegrationData(nextName.NameID, nextName.FullName, parentConsNameIDs, cs, false, null, _thisBatchID, _attPoints);

                    bool process = true;

                    _matchData.GetProviderDataLock();
                    //if this name has the same parent as another name being processed, then use that thread
                    foreach (IntegratorThread th in _threads)
                    {
                        th.ThreadDataMutex.WaitOne();
                        foreach (IntegrationData id in th.NameData)
                        {
                            if (id.ParentConsNameIDs.Intersect(parentConsNameIDs).Count() > 0 || (id.ParentConsNameIDs.Count == 0 && parentConsNameIDs.Count == 0))
                            {
                                th.ThreadDataMutex.ReleaseMutex();
                                th.AddNameData(data);
                                process = false;
                                break;
                            }
                        }
                        if (process) th.ThreadDataMutex.ReleaseMutex(); //not released yet
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

                    _matchData.ReleaseProviderDataLock();
                }
            }
            else
            {
                more = false;

                _matchData.GetAllDataLock();
                int thCount = _threads.Count;
                _matchData.ReleaseAllDataLock();

                //need to check for integration stall - ie when there are still names to integrate but all the parents have failed to integrate, so we cannot progress
                if (Progress < 100 && !_integrationCompleted && thCount == 0)  //ie there are no names to process and there are no threads processing names - so all done!
                {
                    _integrationCompleted = true;

                    PostIntegrationCleanup();
                    Data.Sql.Integration.SaveDataFile(_matchData.AllData, _dataFilePath);
                    Progress = 100;
                }
            }

            return more;
        }

        private bool CheckForCompletion()
        {
            _matchData.GetAllDataLock();

            if (_integrationCompleted)
            {
                _matchData.ReleaseAllDataLock();
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

                        //realy done
                        PostIntegrationCleanup();

                        //save data file
                        Data.Sql.Integration.SaveDataFile(_matchData.AllData, _dataFilePath);

                        prog = 100;
                    }
                    else prog = 99; //not quite done
                }
            }

            Progress = prog;

            _matchData.ReleaseAllDataLock();

            if (_integrationCompleted && ProcessingComplete != null) ProcessingComplete(null, EventArgs.Empty);

            return _integrationCompleted;
        }

        private void ProcessComplete(IntegratorThread it, IntegrationData intData, MatchResult result)
        {
            if (result.Error != null) throw new Exception(result.Error);

            Results.Add(result);

            IntegratorThread.Log.Add(DateTime.Now.ToString() + " : " + intData.FullName + " : " + intData.NameID.ToString() + " : RESULT = " + result.Status.ToString() + ", " + result.MatchedName + ", " + result.MatchedId);

            StatusText = "Processed " + _processedNamesCount.ToString() + " of " + NamesForIntegrationCount().ToString() + " names.  Number of running threads = " + _threads.Count.ToString();

            //more names on this thread?
            if (it.State == IntegratorThread.ProcessingState.Finished)
            {
                _matchData.GetAllDataLock();
                _threads.Remove(it);
                _matchData.ReleaseAllDataLock();
            }

            if (!CheckForCompletion() && Results.Count < _maxToProcess)
            {
                //spawn more threads?
                int numTh = 0;
                int numOtherTh = 0;
                ThreadPool.GetAvailableThreads(out numTh, out numOtherTh);

                if (numTh > 0 && _threads.Count < MaxThreads)
                {
                    bool doAnother = ProcessNextName();
                    while (doAnother && _threads.Count < 3 && _threads.Count < MaxThreads)
                    {
                        doAnother = ProcessNextName();                        
                    }
                }
            }
        }

        private void PostIntegrationCleanup()
        {
            //remove any records that havent changed
            foreach (DataRow cdr in _matchData.AllData.ConsensusData.ConsensusName)
            {
                if (cdr.RowState == DataRowState.Unchanged) cdr.Delete();
            }
            _matchData.AllData.ConsensusData.ConsensusName.AcceptChanges();
            foreach (DataRow pdr in _matchData.NameIntegrationSet.ProviderName)
            {
                if (pdr.RowState == DataRowState.Unchanged) pdr.Delete();
            }
            _matchData.NameIntegrationSet.AcceptChanges();
            foreach (DataRow crr in _matchData.AllData.ConsensusData.ConsensusReference)
            {
                if (crr.RowState == DataRowState.Unchanged) crr.Delete();
            }
            _matchData.AllData.ConsensusData.ConsensusReference.AcceptChanges();
            foreach (DataRow prr in _matchData.AllData.References.ProviderReference)
            {
                if (prr.RowState == DataRowState.Unchanged) prr.Delete();
            }
            _matchData.AllData.References.AcceptChanges();
        }

        private DsIntegrationName.ProviderNameRow GetNextNameForIntegration()
        {
            DsIntegrationName.ProviderNameRow pnRow = null;

            if (_integrationCompleted || _processedNamesCount >= NamesForIntegrationCount() || (_processedNamesCount + _processedRefsCount) > _maxToProcess) return null;

            _matchData.GetAllDataLock();
                        
            foreach (DsIntegrationName.ProviderNameRow nm in _matchData.NameIntegrationSet.ProviderName)
            {
                if (nm.IsConsensusNameIDNull() && nm["IntegrationBatchID"].ToString() != _thisBatchID.ToString())
                {
                    if (nm.IsLinkStatusNull() || (
                        nm.LinkStatus != Data.LinkStatus.Integrating.ToString() &&
                        nm.LinkStatus != Data.LinkStatus.Discarded.ToString() &&
                        nm.LinkStatus != Data.LinkStatus.Matched.ToString() &&
                        nm.LinkStatus != Data.LinkStatus.Inserted.ToString()))
                    {
                        //a name can be integrated if it's parent has been integrated (or if this name has multiple parents define, in which case it will fail anyway)
                        // OR it is a name type that does not have a classification - ie does not rely on the parent having been integrated
                        // OR this name can attach to an attachment point, or has no parents


                        if (nm.HasClassification) Data.Sql.Integration.GetParentData(nm, _matchData.AllData);

                        List<Guid> consParents = Data.Sql.Integration.GetParentConsensusNameIDs(nm); //ignore multiple parent issue at this point
                        List<Guid> parents = Data.Sql.Integration.GetParentNameIDs(nm); 

                        if (!nm.HasClassification || parents.Count == consParents.Count || parents.Count == 0 && consParents.Count > 0) //all possible parents have been integrated
                        {
                            pnRow = nm;
                            pnRow.LinkStatus = "Integrating";
                            break;
                        }
                    }
                }
            }

            _matchData.ReleaseAllDataLock();
            
            return pnRow;
        }

        private DsIntegrationReference.ProviderReferenceRow GetNextReferenceForIntegration()
        {
            DsIntegrationReference.ProviderReferenceRow prRow = null; 

            if (_integrationCompleted || _processedRefsCount >= ReferencesForIntegrationCount() || (_processedRefsCount > _maxToProcess)) return null;

            _matchData.GetProviderDataLock();
            _matchData.GetConsensusDataLock();
            
            foreach (DsIntegrationReference.ProviderReferenceRow pr in _matchData.ReferencesIntegrationSet.ProviderReference)
            {
                if (pr.IsConsensusReferenceIDNull() && pr["IntegrationBatchID"].ToString() != _thisBatchID.ToString())
                {
                    if (pr.IsLinkStatusNull() || (
                        pr.LinkStatus != Data.LinkStatus.Integrating.ToString() &&
                        pr.LinkStatus != Data.LinkStatus.Discarded.ToString() &&
                        pr.LinkStatus != Data.LinkStatus.Matched.ToString() &&
                        pr.LinkStatus != Data.LinkStatus.Inserted.ToString()))
                    {
                        prRow = pr;
                        prRow.LinkStatus = "Integrating";
                        break;
                    }
                }
            }

            _matchData.ReleaseConsensusDataLock();
            _matchData.ReleaseProviderDataLock();

            return prRow;
        }

        private int NamesForIntegrationCount()
        {
            if (_namesToProcessCount == -1)
            {
                _namesToProcessCount = 0;
                foreach (DsIntegrationName.ProviderNameRow nm in _matchData.NameIntegrationSet.ProviderName)
                {
                    if (nm.IsConsensusNameIDNull())
                    {
                        if (nm.IsLinkStatusNull() ||
                            (nm.LinkStatus != Data.LinkStatus.Discarded.ToString() &&
                            nm.LinkStatus != Data.LinkStatus.Matched.ToString() &&
                            nm.LinkStatus != Data.LinkStatus.Inserted.ToString()))
                        {
                            //validate provider data
                            //ValidationResultLookup valRes = IntegrationValidation.ValidateProviderData(nm, false);
                            //_nameValidations.Add(nm.NameID, valRes);
                            //if (valRes.AllClear())
                            //{
                                _namesToProcessCount++;
                            //}
                        }
                    }
                }
            }
            return _namesToProcessCount;
        }

        private int ReferencesForIntegrationCount()
        {
            if (_refsToProcessCount == -1)
            {
                _refsToProcessCount = 0;
                foreach (DsIntegrationReference.ProviderReferenceRow pr in _matchData.ReferencesIntegrationSet.ProviderReference)
                {
                    if (pr.IsConsensusReferenceIDNull())
                    {
                        if (pr.IsLinkStatusNull() ||
                            (pr.LinkStatus != Data.LinkStatus.Discarded.ToString() &&
                            pr.LinkStatus != Data.LinkStatus.Matched.ToString() &&
                            pr.LinkStatus != Data.LinkStatus.Inserted.ToString()))
                        {
                            _refsToProcessCount++;
                        }
                    }
                }
            }
            return _refsToProcessCount;
        }

    }
}
