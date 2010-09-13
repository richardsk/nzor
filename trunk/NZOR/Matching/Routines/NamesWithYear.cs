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

        public override DsNameMatch GetMatchingNames(System.Data.DataSet pn, ref string matchComments)
        {
            //todo
            return null;
        }

        public override void RemoveNonMatches(System.Data.DataSet pn, ref DsNameMatch names, ref string matchComments)
        {
            object pnYear = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Year);

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
                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    cnn.Open();

                    for (int i = names.Name.Count - 1; i >= 0; i--)
                    {
                        DsNameMatch.NameRow row = names.Name[i];
                        if (NZOR.Data.ConsensusName.HasProviderValue(cnn, row.NameID, NZOR.Data.NameProperties.Year, pnYear.ToString()) == false)
                        {
                            row.Delete();
                        }
                    }

                    if (cnn.State != ConnectionState.Closed) cnn.Close();

                }
            }


            names.AcceptChanges();
        }

    }
}
