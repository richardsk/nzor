using System.Data.SqlClient;
using System.Data;
using System;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithPartialYear : BaseMatcher
    {

        public NamesWithPartialYear()
        {
        }

        public override DsNameMatch GetMatchingNames(DsIntegrationName pn, ref string matchComments)
        {
            //todo
            return null;
        }

        public override void RemoveNonMatches(DsIntegrationName pn, ref DsNameMatch names, ref string matchComments)
        {
            object pYr = pn.ProviderName[0]["YearOnPublication"];

            if (pYr == System.DBNull.Value || pYr.ToString().Length == 0) return;
            //succeed 

            String pnYear = pYr.ToString().Trim();
            pnYear = pnYear.Replace("  ", " ");
            pnYear = pnYear.Replace("[", "");
            pnYear = pnYear.Replace("]", "");
            pnYear = pnYear.Replace("\"", "");
            pnYear = pnYear.Replace("?", "");

            for (int i = names.Name.Count - 1; i >= 0; i--)
            {
                DsNameMatch.NameRow row = names.Name[i];
                String yr = row["Year"].ToString().Trim();
                yr = yr.Replace("  ", " ");
                yr = yr.Replace("[", "");
                yr = yr.Replace("]", "");
                yr = yr.Replace("\"", "");
                yr = yr.Replace("?", "");

                if (yr != pnYear)
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }

    }
}
