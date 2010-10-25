using System.Data.SqlClient;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithExactCanonical : BaseMatcher
    {

        public NamesWithExactCanonical()
        {
        }

        public override DsNameMatch GetMatchingNames(DsIntegrationName pn, ref string matchComments)
        {
            //todo (doesnt get called at beginning really)
            return null;
        }

        public override void RemoveNonMatches(DsIntegrationName pn, ref DsNameMatch names, ref string matchComments)
        {
            for (int i = names.Name.Count - 1; i >= 0; i--)
            {
                DsNameMatch.NameRow row = names.Name[i];
                if (row.Canonical.Trim() != pn.ProviderName[0].Canonical.Trim())
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }


    }
}
