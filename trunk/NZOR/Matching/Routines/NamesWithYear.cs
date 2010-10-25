using System.Data.SqlClient;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithYear : BaseMatcher
    {

        public NamesWithYear()
        {
        }

        public override DsNameMatch GetMatchingNames(DsIntegrationName pn, ref string matchComments)
        {
            //todo (doesnt get called at beginning really)
            return null;
        }

        public override void RemoveNonMatches(DsIntegrationName pn, ref DsNameMatch names, ref string matchComments)
        {
            object pnYear = pn.ProviderName[0]["YearOnPublication"];
            if (pnYear == System.DBNull.Value || pnYear.ToString().Length == 0) return;
            //succeed 

            for (int i = names.Name.Count - 1; i >= 0; i--)
            {
                DsNameMatch.NameRow row = names.Name[i];
                if (row["Year"].ToString().Trim() != pnYear.ToString().Trim())
                {
                    row.Delete();
                }
            }

            if (names.Name.Count == 0)
            {
                names.RejectChanges();

                //check prov names 

                for (int i = names.Name.Count - 1; i >= 0; i--)
                {
                    DsNameMatch.NameRow row = names.Name[i];
                    if (NZOR.Data.ConsensusName.HasProviderValue(DBConnection, row.NameID, NZOR.Data.NameProperties.Year, pnYear.ToString()) == false)
                    {
                        row.Delete();
                    }
                }
            }

            names.AcceptChanges();
        }

    }
}
