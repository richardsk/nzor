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

namespace NZOR.Integration.Other
{
    public class IntegrationProcessor3
    {
        public List<MatchResult> Results = new List<MatchResult>();

        public string StatusText = "";
        public int Progress = 0;

        private Guid _thisBatchID = Guid.Empty;
        private Dictionary<Guid, List<IntegratorThread>> _threadsByRank = new Dictionary<Guid,List<IntegratorThread>>();
        private int _maxToProcess = -1;
        private int _namesToProcessCount = -1;
        private int _refsToProcessCount = -1;
        private int _processedNamesCount = 0;
        private int _processedRefsCount = 0;
        private String _dataFilePath = "";
        private bool _integrationCompleted = false;
        private Guid _currentRank = Guid.Empty;
        private DataForIntegration _data = null;
        Semaphore _rankSemaphore = null;
        Semaphore _finishSemaphore = null;
        
        public event EventHandler ProcessingComplete;

        /// <summary>
        /// Use a thread for "group" of names defined by the list of data files.  The first data file should only contain the references to integrate.
        /// An optional maxRecords parameter can be passed in to limit the number of records processed (-1 or 0 for unlimited).
        /// </summary>
        public void RunIntegration(string configFilePath, string dataFilePath, int maxRecords)
        {
            _thisBatchID = Guid.NewGuid();

            IntegratorThread.Log = new List<string>();
            IntegratorThread.Log.Add(DateTime.Now.ToString() + " : Loading dataset file.");


            XmlDocument config = new XmlDocument();
            config.Load(configFilePath);
            MatchProcessor.LoadConfig(config);
            
            Progress = 1; //started

            _namesToProcessCount = 0;
            _maxToProcess = 0;
            _refsToProcessCount = 0;
            _dataFilePath = dataFilePath;

            _data = NZOR.Data.Sql.Integration.LoadDataFile(dataFilePath);

            _refsToProcessCount += ReferencesForIntegrationCount(_data.References);
            
            if (maxRecords == -1 || _refsToProcessCount < maxRecords)
            {
                foreach (Guid rankId in _data.NamesByRank.Keys)
                {
                    List<DsIntegrationName> dsList = _data.NamesByRank[rankId];
                    List<IntegratorThread> rankThreads = new List<IntegratorThread>();
                            
                    int max = -1;

                    foreach (DsIntegrationName dsn in dsList)
                    {
                        int cnt = NamesForIntegrationCount(dsn);
                        if (cnt > 0)
                        {
                            //have we go to max records?
                            if (maxRecords > 0 && _namesToProcessCount + _refsToProcessCount + dsn.ProviderName.Count > maxRecords)
                            {
                                max = maxRecords - (_namesToProcessCount + _refsToProcessCount);
                            }

                            MatchData matchData = new MatchData(false, true, _data, dsn, null);

                            IntegratorThread it = new IntegratorThread(matchData, false, _thisBatchID, max);
                            rankThreads.Add(it);

                            _namesToProcessCount += (max == -1 ? cnt : max);

                            if (max != -1) break; 
                        }
                    }
                    if (rankThreads.Count > 0) _threadsByRank.Add(rankId, rankThreads);

                    if (max != -1) break; 
                }
            }

            _maxToProcess = _namesToProcessCount + _refsToProcessCount;

            //max records?
            if (maxRecords > 0 && maxRecords < _maxToProcess) _maxToProcess = maxRecords;

            IntegratorThread.Log.Add(DateTime.Now.ToString() + " : Number of references to integrate = " + _refsToProcessCount.ToString());
            IntegratorThread.Log.Add(DateTime.Now.ToString() + " : Number of names to integrate = " + _namesToProcessCount.ToString());

            _finishSemaphore = new Semaphore(1, 1);

            //do refs first
            MatchData md = new MatchData(false, false, _data, null, _data.References);
            IntegratorThread rit = new IntegratorThread(md, false, _thisBatchID, (_refsToProcessCount > maxRecords ? maxRecords : -1));
            rit.ReferenceProcessedCallback = new IntegratorThread.ReferenceProcessed(ReferenceProcessed);
            rit.ProcessReferences();
            
            //fire up the threads per rank, waiting til each rank set has finished
            foreach (Guid rankId in _threadsByRank.Keys)
            {
                _currentRank = rankId;
                List<IntegratorThread> itList = _threadsByRank[_currentRank];

                _rankSemaphore = new Semaphore(0, 1);
                
                foreach (IntegratorThread it in itList)
                {
                    it.NameProcessedCallback = new IntegratorThread.NameProcessed(NameProcessed);
                    it.ReferenceProcessedCallback = new IntegratorThread.ReferenceProcessed(ReferenceProcessed);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(it.ProcessAllData));
                }

                _rankSemaphore.WaitOne();

                if (_integrationCompleted)
                {
                    _rankSemaphore.Release();
                    break;
                }
            }

            if (!_integrationCompleted) CheckForCompletion(null);
        }


        private bool CheckForCompletion(IntegratorThread fromThread)
        {
            //only one thread needs to check
            if (!_finishSemaphore.WaitOne(1000)) return false;
            
            bool rankDone = false;
            //check for current rank completion
            if (_currentRank != Guid.Empty)
            {
                rankDone = true;
                foreach (IntegratorThread it in _threadsByRank[_currentRank])
                {
                    if (it.State != IntegratorThread.ProcessingState.Finished)
                    {
                        rankDone = false;
                        break;
                    }
                }
            }
                        
            //check for complete completion
            if (_integrationCompleted)
            {
                return true;
            }

            bool done = false;
            if (_currentRank != Guid.Empty)
            {
                done = true;
                foreach (Guid rankId in _threadsByRank.Keys)
                {
                    foreach (IntegratorThread it in _threadsByRank[rankId])
                    {
                        if (it.State != IntegratorThread.ProcessingState.Finished)
                        {
                            done = false;
                            break;
                        }
                    }
                }
            }

            int prog = 1;
            prog = (Results.Count * 100 / (_namesToProcessCount + _refsToProcessCount));

            if (prog == 0) prog = 1; //at least to indicate we have started
            if (done)
            {
                _integrationCompleted = true;
                
                Progress = 99;
                PostIntegrationCleanup(_data);                 

                Data.Sql.Integration.SaveDataFile(_data, _dataFilePath);

                prog = 100;
            }

            _finishSemaphore.Release();

            Progress = prog;

            if (rankDone || _integrationCompleted && _rankSemaphore != null) _rankSemaphore.Release();

            if (_integrationCompleted && ProcessingComplete != null) ProcessingComplete(null, EventArgs.Empty);

            return _integrationCompleted;
        }

        private void NameProcessed(IntegratorThread it, IntegrationData intData, MatchResult result)
        {
            Results.Add(result);
            
            _processedNamesCount++;

            IntegratorThread.Log.Add(DateTime.Now.ToString() + " : " + intData.FullName + " : " + intData.NameID.ToString() + " : RESULT = " + result.Status.ToString() + ", " + result.MatchedName + ", " + result.MatchedId);

            StatusText = "Processed " + _processedNamesCount.ToString() + " of " + _namesToProcessCount.ToString() + " names.  Number of running threads = " + _threadsByRank[_currentRank].Count.ToString();

            CheckForCompletion(it);
        }

        private void ReferenceProcessed(IntegratorThread it, MatchResult result)
        {
            Results.Add(result);

            _processedRefsCount++;

            IntegratorThread.Log.Add(DateTime.Now.ToString() + " : Reference (" + result.ProviderRecordId + ") : RESULT = " + result.Status.ToString() + ", " + result.MatchedReference + ", " + result.MatchedId);

            StatusText = "Processed " + _processedRefsCount.ToString() + " of " + _refsToProcessCount.ToString() + " references.";

            CheckForCompletion(it);
        }

        private void PostIntegrationCleanup(DataForIntegration data)
        {
            //remove any records that havent changed
            //foreach (DataRow cdr in data.ConsensusData.ConsensusName)
            //{
            //    if (cdr.RowState == DataRowState.Unchanged) cdr.Delete();
            //}
            data.ConsensusData.ConsensusName.AcceptChanges();
            foreach (List<DsIntegrationName> dsList in data.NamesByRank.Values)
            {
                foreach (DsIntegrationName dsn in dsList)
                {
                    foreach (DataRow pdr in dsn.ProviderName)
                    {
                        if (pdr.RowState == DataRowState.Unchanged) pdr.Delete();
                    }
                    dsn.AcceptChanges();
                }
            }
            //foreach (DataRow crr in data.ConsensusData.ConsensusReference)
            //{
            //    if (crr.RowState == DataRowState.Unchanged) crr.Delete();
            //}
            data.ConsensusData.ConsensusReference.AcceptChanges();
            foreach (DataRow prr in data.References.ProviderReference)
            {
                if (prr.RowState == DataRowState.Unchanged) prr.Delete();
            }
            data.References.AcceptChanges();
        }

        private void GetNamesForIntegration(DsIntegrationName names)
        {
            foreach (DsIntegrationName.ProviderNameRow nm in names.ProviderName)
            {
                if (!nm.IsConsensusNameIDNull())
                {
                    if (!nm.IsLinkStatusNull() && (
                        nm.LinkStatus == Data.LinkStatus.Integrating.ToString() ||
                        nm.LinkStatus == Data.LinkStatus.Discarded.ToString() ||
                        nm.LinkStatus == Data.LinkStatus.Matched.ToString() ||
                        nm.LinkStatus == Data.LinkStatus.Inserted.ToString()))
                    {
                        nm.Delete();
                    }
                }
            }

            names.AcceptChanges();
        }

        private void GetReferencesForIntegration(DsIntegrationReference refs)
        {
            foreach (DsIntegrationReference.ProviderReferenceRow pr in refs.ProviderReference)
            {
                if (!pr.IsConsensusReferenceIDNull())
                {
                    if (!pr.IsLinkStatusNull() && (
                        pr.LinkStatus == Data.LinkStatus.Integrating.ToString() ||
                        pr.LinkStatus == Data.LinkStatus.Discarded.ToString() ||
                        pr.LinkStatus == Data.LinkStatus.Matched.ToString() ||
                        pr.LinkStatus == Data.LinkStatus.Inserted.ToString()))
                    {
                        pr.Delete();
                        break;
                    }
                }
            }
            refs.ProviderReference.AcceptChanges();
        }

        private int NamesForIntegrationCount(DsIntegrationName names)
        {
            int nameCount = 0;

            foreach (DsIntegrationName.ProviderNameRow nm in names.ProviderName)
            {
                if (nm.IsConsensusNameIDNull())
                {
                    if (nm.IsLinkStatusNull() ||
                        (nm.LinkStatus != Data.LinkStatus.Discarded.ToString() &&
                        nm.LinkStatus != Data.LinkStatus.Matched.ToString() &&
                        nm.LinkStatus != Data.LinkStatus.Inserted.ToString()))
                    {
                        nameCount++;
                    }
                }
            }

            return nameCount;
        }

        private int ReferencesForIntegrationCount(DsIntegrationReference refs)
        {
            int refCount = 0;
            foreach (DsIntegrationReference.ProviderReferenceRow pr in refs.ProviderReference)
            {
                if (pr.IsConsensusReferenceIDNull())
                {
                    if (pr.IsLinkStatusNull() ||
                        (pr.LinkStatus != Data.LinkStatus.Discarded.ToString() &&
                        pr.LinkStatus != Data.LinkStatus.Matched.ToString() &&
                        pr.LinkStatus != Data.LinkStatus.Inserted.ToString()))
                    {
                        refCount++;
                    }
                }
            }

            return refCount;
        }

    }
}
