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
        
        /// <summary>
        /// Use multiple threads to process records that need integrating
        /// Individual threads will need to work with names that dont overlap with names in other threads - by working with names in different parts of the taxonomic hierarchy.
        ///   i.e. get the next provider name that has it's parent already integrated and there are no siblings of this name currenlty being integrated
        /// </summary>
        public static void RunIntegration(XmlDocument config, Data.DsIntegrationName data)
        {
            bool doAnother = true;

            _thisBatchID = Guid.NewGuid();
            
            MatchData.DataForIntegration = data;

            Progress = 1; //started

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
            else
            {
                more = false;
            }

            return more;
        }

        private static void ProcessComplete(IntegratorThread it, IntegrationData intData, Data.MatchResult result, bool threadFinished)
        {
            lock (MatchData.DataForIntegration)
            {
                int prog = 1;

                Results.Add(result);

                if (IntegratorThread.LogFile != null)
                {
                    IntegratorThread.LogFile.WriteLine(intData.FullName + " : " + intData.NameID.ToString() + " : RESULT = " + result.Status.ToString() + ", " + result.MatchedName + ", " + result.MatchedId);
                }
                
                prog = (Results.Count * 100 / NamesForIntegrationCount());

                StatusText = "Processed " + Results.Count.ToString() + " of " + NamesForIntegrationCount().ToString() + " names.  Number of running threads = " + _threads.Count.ToString();
                if (prog == 0) Progress = 1; //at least to indicate we have started
                if (prog == 100)
                {
                    if (NamesForIntegrationCount() > Results.Count)
                    {
                        prog = 99; //not 100 % complete until ALL names are done
                    }
                    else
                    {
                        //realy done
                        PostIntegrationCleanup();
                    }
                }

                Progress = prog;

                //more names on this thread?
                if (threadFinished)
                {
                    _threads.Remove(it);
                }

                //next name?
                ProcessNextName();                
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
                    if (nm.IsConsensusNameIDNull() &&  nm["IntegrationBatchID"].ToString() != _thisBatchID.ToString() &&
                        nm.LinkStatus != Data.LinkStatus.Integrating.ToString() && 
                        nm.LinkStatus != Data.LinkStatus.Discarded.ToString() && 
                        nm.LinkStatus != Data.LinkStatus.Matched.ToString() &&
                        nm.LinkStatus != Data.LinkStatus.Inserted.ToString())
                    {
                        pnRow = nm;

                        //get the parent name information
                        Data.ProviderName.GetParentData(pnRow, MatchData.DataForIntegration);

                        pnRow.LinkStatus = "Integrating";
                        break;
                    }
                }
            }

            return pnRow;
        }

        private static int NamesForIntegrationCount()
        {
            if (_namesToProcess == -1)
            {
                foreach (Data.DsIntegrationName.ProviderNameRow nm in MatchData.DataForIntegration.ProviderName)
                {
                    if (nm.IsConsensusNameIDNull() &&
                        nm.LinkStatus != Data.LinkStatus.Discarded.ToString() &&
                        nm.LinkStatus != Data.LinkStatus.Matched.ToString() &&
                        nm.LinkStatus != Data.LinkStatus.Inserted.ToString())
                    {
                        _namesToProcess++;
                    }
                }
            }
            return _namesToProcess;
        }

                
    }
}
