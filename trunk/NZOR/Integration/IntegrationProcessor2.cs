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

namespace NZOR.Integration
{
    public class IntegrationProcessor2
    {
        public static List<NZOR.Data.MatchResult> Results = new List<Data.MatchResult>();
        
        public static string StatusText = "";
        public static int MaxThreads = 100;
        public static int Progress = 0;

        private static Guid _thisBatchID = Guid.Empty;
        private static List<IntegratorThread> _threads = new List<IntegratorThread>();
        private static int _namesToProcess = -1;
        private static string _dataFilePath = "";
        
        /// <summary>
        /// Use multiple threads to process records that need integrating
        /// Individual threads will need to work with names that dont overlap with names in other threads - by working with names in different parts of the taxonomic hierarchy.
        ///   i.e. get the next provider name that has it's parent already integrated and there are no siblings of this name currenlty being integrated
        /// </summary>
        public static void RunIntegration(string configFilePath, string dataFilePath)
        {
            bool doAnother = true;

            _dataFilePath = dataFilePath;
            _thisBatchID = Guid.NewGuid();
                        
            IntegratorThread.Log = new List<string>();
            MatchData.DataForIntegration = NZOR.Data.Integration.LoadDataFile(dataFilePath);

            Progress = 1; //started

            XmlDocument config = new XmlDocument();
            config.Load(configFilePath);
            MatchProcessor.LoadConfig(config);
            
            if (MaxThreads != -1 && MaxThreads < 1000) ThreadPool.SetMaxThreads(MaxThreads, 1000);

            while (doAnother)
            {
                doAnother = ProcessNextName();
            }
        }

        private static bool ProcessNextName()
        {
            bool more = true;
            
            DsIntegrationName.ProviderNameRow nextName = GetNextNameForIntegration();
            if (nextName != null)
            {
                if (nextName.IsMatchRuleSetIDNull())
                {
                    lock (MatchData.DataForIntegration)
                    {
                        //system error - no match set defined for this rank
                        nextName.LinkStatus = LinkStatus.DataFail.ToString();
                        nextName.IntegrationBatchID = _thisBatchID;
                        IntegratorThread.Log.Add(DateTime.Now.ToString() + " : SYSTEM ERROR : No match rule set defined for taxon rank '" + nextName.TaxonRank + "', " + nextName.TaxonRankID.ToString());
                        IntegratorThread.Log.Add(DateTime.Now.ToString() + ": ERROR : Integration failed for name '" + nextName.NameID.ToString() + "', " + nextName.FullName + ".  No match rule set defined for the rank.");
                        MatchResult mr = new MatchResult();
                        mr.MatchedId = nextName.NameID.ToString();
                        mr.MatchedName = nextName.FullName;
                        mr.Status = LinkStatus.DataFail;
                        Results.Add(mr);
                    }
                    //returns true so it looks for the next name to do

                    CheckForCompletion();
                }
                else
                {
                    ConfigSet cs = MatchProcessor.GetMatchSet(nextName.MatchRuleSetID);
                    Guid parentConsNameID = Guid.Empty;
                    if (!nextName.IsParentConsensusNameIDNull()) parentConsNameID = nextName.ParentConsensusNameID;

                    IntegrationData data = new IntegrationData(nextName.NameID, nextName.FullName, parentConsNameID, cs, false, null, _thisBatchID);

                    bool process = true;
                    //if this name has the same parent as another name being processed, then use that thread
                    if (parentConsNameID != Guid.Empty)
                    {
                        lock (MatchData.DataForIntegration)
                        {
                            foreach (IntegratorThread th in _threads)
                            {
                                foreach (IntegrationData id in th.NameData)
                                {
                                    if (id.ParentConsNameID == parentConsNameID)
                                    {
                                        th.AddNameData(data);
                                        process = false;
                                        break;
                                    }
                                }
                                if (!process) break;
                            }
                        }
                    }

                    if (process)
                    {
                        IntegratorThread it = new IntegratorThread();
                        it.AddNameData(data);
                        it.ProcessCompleteCallback = new IntegratorThread.ProcessComplete(ProcessComplete);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(it.ProcessName));

                        _threads.Add(it);

                        //check threads                    
                        int numTh = 0;
                        int numOtherTh = 0;
                        ThreadPool.GetAvailableThreads(out numTh, out numOtherTh);

                        if (numTh < 2 || _threads.Count >= MaxThreads) more = false; //leave at least 1 thread ??    
                    }
                }
            }
            else
            {
                more = false;
            }

            return more;
        }

        private static bool CheckForCompletion()
        {
            bool done = false;

            int prog = 1;
            prog = (Results.Count * 100 / NamesForIntegrationCount());

            if (prog == 0) prog = 1; //at least to indicate we have started
            if (prog == 100)
            {
                if (NamesForIntegrationCount() > Results.Count)
                {
                    prog = 99; //not 100 % complete until ALL names are done
                }
                else
                {
                    Progress = 100;

                    //realy done
                    PostIntegrationCleanup();

                    //save data file
                    Data.Integration.SaveDataFile(MatchData.DataForIntegration, _dataFilePath);

                    done = true;
                }
            }

            Progress = prog;

            return done;
        }

        private static void ProcessComplete(IntegratorThread it, IntegrationData intData, Data.MatchResult result, bool threadFinished)
        {
            lock (MatchData.DataForIntegration)
            {
                Results.Add(result);

                IntegratorThread.Log.Add(DateTime.Now.ToString() + " : " + intData.FullName + " : " + intData.NameID.ToString() + " : RESULT = " + result.Status.ToString() + ", " + result.MatchedName + ", " + result.MatchedId);

                StatusText = "Processed " + Results.Count.ToString() + " of " + NamesForIntegrationCount().ToString() + " names.  Number of running threads = " + _threads.Count.ToString();

                //more names on this thread?
                if (threadFinished)
                {
                    _threads.Remove(it);
                }

                if (!CheckForCompletion()) ProcessNextName();                
            }
        }

        private static void PostIntegrationCleanup()
        {
            //TODO ??
        }

        private static DsIntegrationName.ProviderNameRow GetNextNameForIntegration()
        {
            DsIntegrationName.ProviderNameRow pnRow = null;

            lock (MatchData.DataForIntegration)
            {
                foreach (Data.DsIntegrationName.ProviderNameRow nm in MatchData.DataForIntegration.ProviderName)
                {
                    if (nm.IsConsensusNameIDNull() && nm["IntegrationBatchID"].ToString() != _thisBatchID.ToString())
                    {
                        if (nm.IsLinkStatusNull() || (
                            nm.LinkStatus != Data.LinkStatus.Integrating.ToString() &&
                            nm.LinkStatus != Data.LinkStatus.Discarded.ToString() &&
                            nm.LinkStatus != Data.LinkStatus.Matched.ToString() &&
                            nm.LinkStatus != Data.LinkStatus.Inserted.ToString()))
                        {
                            pnRow = nm;

                            //get the parent name information
                            Data.ProviderName.GetParentData(pnRow, MatchData.DataForIntegration);

                            pnRow.LinkStatus = "Integrating";
                            break;
                        }
                    }
                }
            }

            return pnRow;
        }

        private static int NamesForIntegrationCount()
        {
            if (_namesToProcess == -1)
            {
                _namesToProcess = 0;
                foreach (Data.DsIntegrationName.ProviderNameRow nm in MatchData.DataForIntegration.ProviderName)
                {
                    if (nm.IsConsensusNameIDNull())
                    {
                        if (nm.IsLinkStatusNull() ||
                            (nm.LinkStatus != Data.LinkStatus.Discarded.ToString() &&
                            nm.LinkStatus != Data.LinkStatus.Matched.ToString() &&
                            nm.LinkStatus != Data.LinkStatus.Inserted.ToString()))
                        {
                            _namesToProcess++;
                        }
                    }
                }
            }
            return _namesToProcess;
        }

                
    }
}
