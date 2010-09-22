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

        public Data.MatchResult Result = new NZOR.Data.MatchResult();

        public delegate void ProcessComplete(IntegratorThread it);
        public ProcessComplete ProcessCompleteCallback;

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
                Data.MatchResult res = Integrator.DoMatch(provName, Config.Routines);

                if (res.Matches.Count == 0)
                {
                    //insert
                    NZOR.Data.Consensus.Name newName = NZOR.Data.ConsensusName.AddConsensusName(provName);
                    NZOR.Data.ProviderName.UpdateProviderNameLink(provName, NZOR.Data.LinkStatus.Inserted, newName.NameID, 0, res.MatchPath);

                    res.MatchedId = newName.NameID.ToString();
                    res.MatchedName = newName.FullName;                    
                    res.Status = NZOR.Data.LinkStatus.Inserted;
                }
                else if (res.Matches.Count == 1)
                {
                    //link 
                    NZOR.Data.ProviderName.UpdateProviderNameLink(provName, NZOR.Data.LinkStatus.Matched, res.Matches[0].NameId, res.Matches[0].MatchScore, res.MatchPath);
                    res.Status = NZOR.Data.LinkStatus.Matched;
                }
                else
                {
                    //multiple matches
                    NZOR.Data.ProviderName.UpdateProviderNameLink(provName, NZOR.Data.LinkStatus.Multiple, null, 0, res.MatchPath);                    
                    res.Status = NZOR.Data.LinkStatus.Multiple;
                }

                Result = res;
            }

            if (ProcessCompleteCallback != null) ProcessCompleteCallback(this);
        }
    }
}
