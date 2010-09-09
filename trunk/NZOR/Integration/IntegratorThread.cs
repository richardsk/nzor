using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;

namespace NZOR.Integration
{
    public enum IntegrationOutcome
    {
        Added,
        Mathced,
        Failed
    }

    public class IntegratorThread
    {
        public Guid NameID = Guid.Empty;
        public ConfigSet Config;

        public Matching.MatchResult Result = new NZOR.Matching.MatchResult();

        public IntegratorThread()
        {
        }

        public IntegratorThread(Guid nameID, ConfigSet cs)
        {
            NameID = nameID;
            Config = cs;
        }

        public void ProcessName(Object stateInfo)
        {            
            if (Config != null)
            {
                DataSet provName = Data.ProviderName.GetNameMatchData(NameID);
                List<Matching.NameMatch> matches = Integrator.DoMatch(provName, Config.Routines);

                if (matches.Count == 0)
                {
                    //insert
                    NZOR.Data.Consensus.Name newName = NZOR.Data.ConsensusName.AddConsensusName(provName);
                    NZOR.Data.ProviderName.UpdateProviderNameLink(provName, newName.NameID, NZOR.Data.LinkStatus.Inserted, 0);

                    Result.MatchedId = newName.NameID.ToString();
                    Result.MatchedName = newName.FullName;                    
                    Result.Status = NZOR.Data.LinkStatus.Inserted;
                }
                else if (matches.Count == 1)
                {
                    //link 
                    NZOR.Data.ProviderName.UpdateProviderNameLink(provName, matches[0].NameId, NZOR.Data.LinkStatus.Matched, matches[0].MatchScore);
                    Result.Status = NZOR.Data.LinkStatus.Matched;
                }
                else
                {
                    //multiple matches
                    NZOR.Data.ProviderName.UpdateProviderNameLink(provName, null, NZOR.Data.LinkStatus.Multiple, 0);                    
                    Result.Status = NZOR.Data.LinkStatus.Multiple;
                }
            }
        }
    }
}
