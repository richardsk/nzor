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
        
        public void ProcessName(Object stateInfo)
        {            
            if (Config != null)
            {
                DataSet provName = Data.ProviderName.GetNameMatchData(NameID);
                List<Matching.NameMatch> matches = Integrator.DoMatch(provName, Config.Routines);

                if (matches.Count == 0)
                {
                    //insert

                    Result.Status = NZOR.Data.LinkStatus.Inserted;
                }
                else if (matches.Count == 1)
                {
                    //link 

                    Result.Status = NZOR.Data.LinkStatus.Matched;
                }
                else
                {
                    //multiple matches

                    Result.Status = NZOR.Data.LinkStatus.Multiple;
                }
            }
        }
    }
}
