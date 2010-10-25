using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Xml;

namespace NZOR.Integration
{
    public class IntegrationProcessor
    {
        public static List<NZOR.Data.MatchResult> Results = new List<Data.MatchResult>();
        public static string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

        public static string StatusText = "";
        public static int MaxThreads = 20;
        public static int Progress = 0;

        private static int _namesToProcess = 0;
        private static ConfigSet _configSet = null;

        private static object lockKey = new object();

        /// <summary>
        /// Use multiple threads to process records that need integrating
        /// Individual threads will need to work with names that dont overlap with names in other threads - by working with names in different parts of the taxonomic hierarchy.
        ///   i.e. get the next provider name that has it's parent already integrated and there are no siblings of this name currenlty being integrated
        /// </summary>
        public static void RunIntegration(XmlDocument config, int setNumber)
        {
            bool doAnother = true;
            
            SqlConnection cnn = new SqlConnection(ConnectionString);
            cnn.Open();
            
            _namesToProcess = GetNamesForIntegrationCount(cnn);
            Progress = 1; //started

            _configSet = new ConfigSet();
            _configSet.Routines = Integrator.LoadConfig(config, setNumber);
            _configSet.SetNumber = setNumber;

            if (MaxThreads != -1 && MaxThreads < 1000) ThreadPool.SetMaxThreads(MaxThreads, 1000);

            while (doAnother)
            {
                doAnother = ProcessNextName(cnn);
            }

            NZOR.Data.ProviderName.PostIntegrationCleanup(cnn);

            cnn.Close();            
        }

        private static bool ProcessNextName(SqlConnection cnn)
        {
            bool more = true;

            string fullName = "";
            Guid nextName = GetNextNameForIntegration(cnn, ref fullName);
            if (nextName != Guid.Empty)
            {
                IntegratorThread it = new IntegratorThread(nextName, _configSet, ConnectionString);
                it.ProcessCompleteCallback = new IntegratorThread.ProcessComplete(ProcessComplete);
                ThreadPool.QueueUserWorkItem(new WaitCallback(it.ProcessName));

                //check threads                    
                int numTh = 0;
                int numOtherTh = 0;
                ThreadPool.GetAvailableThreads(out numTh, out numOtherTh);

                if (numTh < 2) more = false; //leave at least 1 thread ??    
            }
            else
            {
                Progress = 100; //done
                more = false;
            }

            return more;
        }

        private static void ProcessComplete(IntegratorThread it)
        {
            lock (lockKey)
            {
                int prog = 1;

                Results.Add(it.Result);
                prog = (Results.Count * 100 / _namesToProcess);

                int numTh = 0;
                int numOtherTh = 0;
                int maxNumTh = 0;
                int maxOtherTh = 0;
                ThreadPool.GetAvailableThreads(out numTh, out numOtherTh);
                ThreadPool.GetMaxThreads(out maxNumTh, out maxOtherTh);
                int threads = maxNumTh - numTh;
                StatusText = "Processed " + Results.Count.ToString() + " of " + _namesToProcess.ToString() + " names.  Number of running threads = " + threads.ToString();
                if (prog == 0) Progress = 1; //at least to indicate we have started
                if (prog == 100 && _namesToProcess > Results.Count) prog = 99; //not 100 % complete until ALL names are done

                Progress = prog;

                //next name?
                SqlConnection cnn = new SqlConnection(ConnectionString);
                cnn.Open();
                ProcessNextName(cnn);
                cnn.Close();
            }
        }

        private static Guid GetNextNameForIntegration(SqlConnection cnn, ref string fullName)
        {
            Guid id = Guid.Empty;


            using (SqlCommand cmd = cnn.CreateCommand())
            {
                // ???
                /*cmd.CommandText = "select top 1 pn.nameid " + 
                    "from vwproviderconcepts pn " +
                    "left join vwproviderconcepts sn on sn.relationshiptypeid = '" + NZOR.Data.ConceptRelationshipType.ParentRelationshipTypeID().ToString() + "' and sn.toconceptid = pn.toconceptid and sn.nameid <> pn.nameid " +
                        "and (sn.linkstatus is null or sn.linkstatus <> 'Integrating') " +                        
                    "where sn.nameid is null and pn.consensusnameid is null " +
                    "order by pn.sortorder";*/

                cmd.CommandText = "select top 1 n.NameId, n.FullName " + 
                                    "from provider.Name n " +
                                    "inner join prov.Flatname fn on fn.seednameid = n.NameID and fn.depth = 0 " +
                                    "left join (select sfn.NameID, sfn.ParentNameID from prov.FlatName sfn inner join provider.Name sn on sn.NameID = sfn.SeedNameID and sn.LinkStatus = 'Integrating') sn on sn.ParentNameID = fn.ParentNameID and sn.NameID <> n.NameID " +
                                    "where n.ConsensusNameID is null and (n.LinkStatus is null or n.LinkStatus <> 'Integrating') and sn.ParentNameID is null " +
                                    "order by fn.SortOrder";
                
                DataSet res = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(res);
                if (res != null && res.Tables.Count > 0 && res.Tables[0].Rows.Count > 0)
                {
                    id = (Guid)res.Tables[0].Rows[0]["NameId"];
                    fullName = res.Tables[0].Rows[0]["FullName"].ToString();
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

            return id;
        }

        private static int GetNamesForIntegrationCount(SqlConnection cnn)
        {
            int cnt = 0;

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "select count(nameid) from provider.Name where ConsensusNameId is null";

                object val = cmd.ExecuteScalar();
                if (val != DBNull.Value) cnt = (int)val;
            }

            return cnt;
        }

        
    }
}
