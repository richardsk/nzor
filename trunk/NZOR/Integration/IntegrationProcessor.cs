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
        public string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

        //private static int maxThreadCount = 200; //max process threads

        /// <summary>
        /// Use multiple threads to process records that need integrating
        /// Individual threads will need to work with names that dont overlap with names in other threads - perhaps by working with names 
        /// in different parts of the taxonomic hierarchy.
        /// </summary>
        public void RunIntegration(XmlDocument config, int setNumber)
        {
            bool doAnother = true;

            ConfigSet cs = new ConfigSet();
            cs.Routines = Integrator.LoadConfig(config, setNumber);
            cs.SetNumber = setNumber;

            while (doAnother)
            {
                Guid nextName = GetNextNameForIntegration();
                if (nextName != Guid.Empty)
                {
                    IntegratorThread it = new IntegratorThread(nextName, cs);
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
        }

        private Guid GetNextNameForIntegration()
        {
            //TODO create threads per letter, ie, one for A, B, C, D ... - to avoid possible conflicts
            Guid id = Guid.Empty;

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = cnn.CreateCommand())
                {                    
                    cmd.CommandText = "select top 1 pn.nameid " + 
                        "from vwproviderconcepts pn " +
                        "left join vwproviderconcepts sn on sn.relationshiptypeid = '" + NZOR.Data.ConceptRelationshipType.ParentRelationshipTypeID().ToString() + "' and sn.toconceptid = pn.toconceptid and sn.nameid <> pn.nameid " +
	                        "and (sn.linkstatus is null or sn.linkstatus <> 'Integrating') " +
                        "left join prov.name par on pn.relationshiptypeid = '" + NZOR.Data.ConceptRelationshipType.ParentRelationshipTypeID().ToString() + "' and par.nameid = pn.nametoid " +
                        "where pn.consensusnameid is not null and sn.nameid is null and pn.consensusnameid is null " +
                        "order by pn.sortorder";

                    object val = cmd.ExecuteScalar();
                    if (val != DBNull.Value) id = (Guid)val;
                }
            }

            return id;
        }

        
    }
}
