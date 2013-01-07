using System.Data.SqlClient;
using System.Data;

using NZOR.Data;
using NZOR.Data.DataSets;

namespace NZOR.Matching
{
    public class NamesWithExactCanonical : BaseMatcher
    {

        public NamesWithExactCanonical()
        {
        }

        public override DsNameMatch GetMatchingNames(DsIntegrationName.ProviderNameRow pn, ref string matchComments)
        {
            //todo (doesnt get called at beginning really)
            return null;
        }

        public override void RemoveNonMatches(DsIntegrationName.ProviderNameRow pn, ref DsNameMatch names, ref string matchComments)
        {
            for (int i = names.Name.Count - 1; i >= 0; i--)
            {
                DsNameMatch.NameRow row = names.Name[i];
                if (row["Canonical"].ToString().Trim() != pn["Canonical"].ToString().Trim())
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }


    }
}
