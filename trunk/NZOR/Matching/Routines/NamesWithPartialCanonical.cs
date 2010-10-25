using System.Data.SqlClient;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithPartialCanonical : BaseMatcher
    {

        public NamesWithPartialCanonical()
        {
        }


        public override DsNameMatch GetMatchingNames(DsIntegrationName pn, ref string matchComments)
        {
            //todo
            return null;
        }

        public override void RemoveNonMatches(DsIntegrationName pn, ref DsNameMatch names, ref string matchComments)
        {
            for (int i = names.Name.Count - 1; i >= 0; i--)
            {
                DsNameMatch.NameRow row = names.Name[i];
                if (Utility.LevenshteinPercent(row.Canonical.Trim(), pn.ProviderName[0].Canonical.Trim()) < Threshold)
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }

    }
}
