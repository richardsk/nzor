using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

using NZOR.Data.Entities.Integration;

namespace NZOR.Integration
{
    public class SQLIntegrationProcessor
    {
        public string StatusText = "Not running.";
        
        /// <summary>
        /// Empties the consensus data from the DB (Does NOT maintain NZOR IDs).  Then regenerates NZOR data from provider data.
        /// </summary>
        public void RunInitialIntegration(XmlDocument config, bool clearConsensusData)
        {
            String cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            if (clearConsensusData)
            {
                StatusText = "Clearing NZOR data";
                Data.Sql.Integration.ClearConsensusData(cnnStr);
            }

            StatusText = "Generating NZOR backbone data";
            Data.Sql.Integration.GenerateConsensusBackbone(cnnStr);

            //update full names
            StatusText = "Refreshing Full Name Values";
            Data.Sql.Integration.UpdateConsensusFullNameValues(cnnStr);

            StatusText = "Intregrating records";

            //integrate remaining tricky names
            IntegrationProcessor proc = new IntegrationProcessor();
            proc.RunIntegration(config, -1);


            StatusText = "Fixing conflicting consensus data";
            //check for conflicts and use preferred provider ranking
            Data.Sql.Integration.ProcessProviderDataConflicts(cnnStr);
            
            //concept conflicts
            Data.Sql.Integration.ProcessProviderConceptConflicts(cnnStr);

            Data.Sql.Integration.UpdateConsensusStackedNameData(cnnStr);

            StatusText = "Integration run completed";
        }


        /// <summary>
        /// Runs an update integration to attempt to link up any unintegrated provider data.
        /// </summary>
        public void RunUpdateIntegration(XmlDocument config)
        {
            String cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            StatusText = "Running integration update";

            Data.Sql.Integration.GenerateConsensusBackbone(cnnStr);
            StatusText = "Intregrating bare records";

            //integrate remaining tricky names
            IntegrationProcessor proc = new IntegrationProcessor();
            proc.RunIntegration(config, -1);


            //update full names
            StatusText = "Refreshing Full Name Values";
            Data.Sql.Integration.UpdateConsensusFullNameValues(cnnStr);

            StatusText = "Fixing conflicting consensus data";
            //check for conflicts and use preferred provider ranking
            Data.Sql.Integration.ProcessProviderDataConflicts(cnnStr);

            //concept conflicts
            Data.Sql.Integration.ProcessProviderConceptConflicts(cnnStr);

            Data.Sql.Integration.UpdateConsensusStackedNameData(cnnStr);

            //post integration
            Data.Sql.Integration.PostIntegrationCleanup(cnnStr);

            StatusText = "Integration update completed";
        }

        public List<Matching.MatchResult> GetMatches(DataForIntegration dfi)
        {
            List<Matching.MatchResult> results = new List<Matching.MatchResult>();

            StatusText = "Matching names";

            //TODO 

            StatusText = "Matching names completed";

            return results;
        }

    }
}
