using System.Data.SqlClient;
using System.Data;
using System;

using NZOR.Data;
using NZOR.Data.DataSets;

namespace NZOR.Matching
{
    public class NamesWithPartialYear : BaseMatcher
    {

        public NamesWithPartialYear()
        {
        }

        public override DsNameMatch GetMatchingNames(DsIntegrationName.ProviderNameRow pn, ref string matchComments)
        {
            //todo
            return null;
        }

        public override void RemoveNonMatches(DsIntegrationName.ProviderNameRow pn, ref DsNameMatch names, ref string matchComments)
        {
            object pYr = pn["YearOfPublication"];

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
                else
                {
                    //year only partially matches
                    row.PercentMatch = (Math.Abs(row["Year"].ToString().Length - pYr.ToString().Length) * 100 / Math.Max(row["Year"].ToString().Length, pYr.ToString().Length));
                }
            }

            names.AcceptChanges();
        }

    }
}
