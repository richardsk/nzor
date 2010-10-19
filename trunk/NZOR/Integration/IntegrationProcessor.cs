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
        public List<NZOR.Data.MatchResult> Results = new List<Data.MatchResult>();
        public string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

        public int Progress = 0;
        public string StatusText = "";

        private int m_namesToProcess = 0;
        private Thread[] m_IntegrationThreads = new Thread[25];
        
        /// <summary>
        /// Use multiple threads to process records that need integrating
        /// Individual threads will need to work with names that dont overlap with names in other threads - by working with names in different parts of the taxonomic hierarchy.
        ///   i.e. get the next provider name that has it's parent already integrated and there are no siblings of this name currenlty being integrated
        /// </summary>
        public void RunIntegration(XmlDocument config, int setNumber)
        {
            bool doAnother = true;
            
            SqlConnection cnn = new SqlConnection(ConnectionString);
            cnn.Open();

            m_namesToProcess = GetNamesForIntegrationCount(cnn);
            Progress = 0;

            ConfigSet cs = new ConfigSet();
            cs.Routines = Integrator.LoadConfig(config, setNumber);
            cs.SetNumber = setNumber;

            while (doAnother)
            {
                string fullName = "";
                Guid nextName = GetNextNameForIntegration(cnn, ref fullName);
                if (nextName != Guid.Empty)
                {                    
                    IntegratorThread it = new IntegratorThread(nextName, cs, ConnectionString);
                    it.ProcessCompleteCallback = new IntegratorThread.ProcessComplete(this.ProcessComplete);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(it.ProcessName));

                    int numTh = 0;
                    int numOtherTh = 0;
                    ThreadPool.GetAvailableThreads(out numTh, out numOtherTh);

                    if (numTh < 2) doAnother = false;
                    
                    //if (currentThreads.Count > maxThreadCount) doAnother = false;
                }
                else
                {
                    doAnother = false;
                }
            }

            Progress = 100;

            //todo - look for any names that have been attempted to be integrated but failed and still have a status of "Integrating"
        }

        private void ProcessComplete(IntegratorThread it)
        {
            Results.Add(it.Result);
            Progress = (Results.Count * 100 / m_namesToProcess);

            int numTh = 0;
            int numOtherTh = 0;
            int maxNumTh = 0;
            int maxOtherTh = 0;
            ThreadPool.GetAvailableThreads(out numTh, out numOtherTh);
            ThreadPool.GetMaxThreads(out maxNumTh, out maxOtherTh);
            int threads = maxNumTh - numTh;
            StatusText = "Processed " + Results.Count.ToString() + " of " + m_namesToProcess.ToString() + " names.  Number of running threads = " + threads.ToString();
            if (Progress == 0) Progress = 1; //at least to indicate we have started
            if (Progress == 100 && m_namesToProcess > Results.Count) Progress = 99; //not 100 % complete until ALL names are done
        }

        private Guid GetNextNameForIntegration(SqlConnection cnn, ref string fullName)
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
                                    "from prov.Name n " +
                                    "inner join prov.Flatname fn on fn.seednameid = n.NameID and fn.depth = 0 " +
                                    "where n.ConsensusNameID is null and (n.LinkStatus is null or n.LinkStatus <> 'Integrating') " +
                                    "and not exists(select sfn.nameid from prov.FlatName sfn inner join prov.Name sn on sn.NameID = sfn.SeedNameID and sfn.ParentNameID = fn.ParentNameID where sn.LinkStatus = 'Integrating')";
                
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
                    cmd.CommandText = "update prov.Name set LinkStatus = 'Integrating' where NameID = '" + id.ToString() + "'";
                    cmd.ExecuteNonQuery();
                }
            }

            return id;
        }

        private int GetNamesForIntegrationCount(SqlConnection cnn)
        {
            int cnt = 0;

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "select count(nameid) from prov.Name where ConsensusNameId is null";

                object val = cmd.ExecuteScalar();
                if (val != DBNull.Value) cnt = (int)val;
            }

            return cnt;
        }

        
    }
}
