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
        public static int MaxThreads = 20;
        public static int Progress = 0;

        private static List<IntegratorThread> _threads = new List<IntegratorThread>();
        
        /// <summary>
        /// Use multiple threads to process records that need integrating
        /// Individual threads will need to work with names that dont overlap with names in other threads - by working with names in different parts of the taxonomic hierarchy.
        ///   i.e. get the next provider name that has it's parent already integrated and there are no siblings of this name currenlty being integrated
        /// </summary>
        public static void RunIntegration(XmlDocument config, Data.DsIntegrationName data)
        {
            bool doAnother = true;

            MatchData.DataForIntegration = data;

            Progress = 1; //started

            MatchProcessor.LoadConfig(config);
            
            if (MaxThreads != -1 && MaxThreads < 1000) ThreadPool.SetMaxThreads(MaxThreads, 1000);

            while (doAnother)
            {
                doAnother = ProcessNextName();
            }

            PostIntegrationCleanup();
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

                IntegrationData data = new IntegrationData(nextName.NameID, nextName.FullName, parentConsNameID, cs, false, null);

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

                    if (numTh < 2) more = false; //leave at least 1 thread ??    
                }
            }
            else
            {
                Progress = 100; //done
                more = false;
            }

            return more;
        }

        private static void ProcessComplete(IntegratorThread it, Data.MatchResult result, Guid provNameID)
        {
            lock (MatchData.DataForIntegration)
            {
                int prog = 1;

                Results.Add(result);
                prog = (Results.Count * 100 / MatchData.DataForIntegration.ProviderName.Count);

                int numTh = 0;
                int numOtherTh = 0;
                int maxNumTh = 0;
                int maxOtherTh = 0;
                ThreadPool.GetAvailableThreads(out numTh, out numOtherTh);
                ThreadPool.GetMaxThreads(out maxNumTh, out maxOtherTh);
                int threads = maxNumTh - numTh;
                StatusText = "Processed " + Results.Count.ToString() + " of " + MatchData.DataForIntegration.ProviderName.Count.ToString() + " names.  Number of running threads = " + threads.ToString();
                if (prog == 0) Progress = 1; //at least to indicate we have started
                if (prog == 100 && MatchData.DataForIntegration.ProviderName.Count > Results.Count) prog = 99; //not 100 % complete until ALL names are done

                Progress = prog;

                if (IntegratorThread.LogFile != null)
                {
                    IntegrationData pn = it.GetProcessedNameData(provNameID);
                    IntegratorThread.LogFile.WriteLine(pn.FullName + " : " + pn.NameID.ToString() + " : RESULT = " + result.Status.ToString() + ", " + result.MatchedName + ", " + result.MatchedId);
                }

                //more names on this thread?
                if (it.NameData.Count == 0)
                {
                    _threads.Remove(it);
                }

                //next name?
                ProcessNextName();                
            }
        }

        private static void PostIntegrationCleanup()
        {
        }

        private static DsIntegrationName.ProviderNameRow GetNextNameForIntegration()
        {
            DsIntegrationName.ProviderNameRow pnRow = null;

            lock (MatchData.DataForIntegration)
            {
                foreach (Data.DsIntegrationName.ProviderNameRow nm in MatchData.DataForIntegration.ProviderName)
                {
                    if (nm.IsConsensusNameIDNull() && nm.LinkStatus != "Integrating")
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

                
    }
}
