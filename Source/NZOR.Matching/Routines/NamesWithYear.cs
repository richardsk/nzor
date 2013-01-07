using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

using NZOR.Data;
using NZOR.Data.DataSets;
using NZOR.Data.LookUps.Common;

namespace NZOR.Matching
{
    public class NamesWithYear : BaseMatcher
    {

        public NamesWithYear()
        {
        }

        public override DsNameMatch GetMatchingNames(DsIntegrationName.ProviderNameRow pn, ref string matchComments)
        {
            //todo (doesnt get called at beginning really)
            return null;
        }

        public override void RemoveNonMatches(DsIntegrationName.ProviderNameRow pn, ref DsNameMatch names, ref string matchComments)
        {
            object pnYear = pn["YearOfPublication"];
            if (pnYear == System.DBNull.Value || pnYear.ToString().Length == 0) return;
            //succeed 

            for (int i = names.Name.Count - 1; i >= 0; i--)
            {
                DsNameMatch.NameRow row = names.Name[i];
                if (row["Year"] != System.DBNull.Value && row["Year"].ToString().Trim() != pnYear.ToString().Trim())
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
                    bool hasValue = false;
                    if (UseDBConnection)
                    {
                        using (SqlConnection cnn = new SqlConnection(DBConnectionString))
                        {
                            cnn.Open();
                            hasValue = NZOR.Data.Sql.Integration.HasProviderValue(cnn, row.NameID, NamePropertyTypeLookUp.Year, pnYear.ToString());
                        }
                    }
                    else
                    {
                        MatchData.GetAllDataLock();

                        List<DsIntegrationName.ProviderNameRow> pnList = MatchData.AllData.GetProviderNames(row.NameID);

                        foreach (DsIntegrationName.ProviderNameRow pnr in pnList)
                        {
                            if (pnr.YearOfPublication.ToLower() == pnYear.ToString().ToLower()) hasValue = true;
                        }

                        MatchData.ReleaseAllDataLock();
                    }

                    if (!hasValue)
                    {
                        row.Delete();
                    }
                }
            }

            names.AcceptChanges();
        }

    }
}
