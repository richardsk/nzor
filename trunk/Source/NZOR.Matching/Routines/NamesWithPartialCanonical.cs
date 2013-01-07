using System.Data.SqlClient;
using System.Data;

using NZOR.Data;
using NZOR.Data.DataSets;

namespace NZOR.Matching
{
    public class NamesWithPartialCanonical : BaseMatcher
    {

        public NamesWithPartialCanonical()
        {
        }


        public override DsNameMatch GetMatchingNames(DsIntegrationName.ProviderNameRow pn, ref string matchComments)
        {
            //todo
            return null;
        }

        public override void RemoveNonMatches(DsIntegrationName.ProviderNameRow pn, ref DsNameMatch names, ref string matchComments)
        {
            for (int i = names.Name.Count - 1; i >= 0; i--)
            {
                DsNameMatch.NameRow row = names.Name[i];
                double perc = Utility.LevenshteinPercent(row["Canonical"].ToString().Trim(), pn["Canonical"].ToString().Trim());
                if (perc < Threshold)
                {
                    row.Delete();
                }
                else
                {
                    //update match score
                    row.PercentMatch = (int)perc;
                }
            }

            names.AcceptChanges();
        }

    }
}
