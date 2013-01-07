using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Data;
using NZOR.Data.DataSets;
using NZOR.Data.LookUps.Common;

namespace NZOR.Matching
{
    public class NamesWithSameValidity : BaseMatcher
    {
        
        public override DsNameMatch GetMatchingNames(DsIntegrationName.ProviderNameRow pn, ref string matchComments)
        {
            //todo (doesnt get called at beginning really)
            return null;
        }

        public override void RemoveNonMatches(DsIntegrationName.ProviderNameRow pn, ref DsNameMatch names, ref string matchComments)
        {
            object pnStatus = pn["NomStatus"];
            if (pnStatus == System.DBNull.Value || pnStatus.ToString().Length == 0) return; //succeed 
                        
            for (int i = names.Name.Count - 1; i >= 0; i--)
            {
                DsNameMatch.NameRow row = names.Name[i];
                if (!row.IsNomStatusNull() &&
                    (row["NomStatus"].ToString().Contains("inval") && !pnStatus.ToString().Contains("inval") ||
                    !row["NomStatus"].ToString().Contains("inval") && pnStatus.ToString().Contains("inval") ||
                    row["NomStatus"].ToString().Contains("illeg") && !pnStatus.ToString().Contains("illeg") ||
                    !row["NomStatus"].ToString().Contains("illeg") && pnStatus.ToString().Contains("illeg")))
                {
                    row.Delete();
                }
            }
        }
    }
}
