using System.Data.SqlClient;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithRank : BaseMatcher    
    {

        public NamesWithRank()
        {
        }

        public override DsNameMatch GetMatchingNames(System.Data.DataSet pn)
        {
            object val = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Rank);

            DsNameMatch ds = new DsNameMatch();

            if (val != System.DBNull.Value)
            {
                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    cnn.Open();

                    ds = NZOR.Data.ConsensusName.GetNamesWithProperty(cnn, NZOR.Data.NameProperties.Rank, val);

                    if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
                }
            }

            return ds;
        }

        public override void RemoveNonMatches(System.Data.DataSet pn, ref DsNameMatch names)
        {
            object pnRank = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Rank);

            for (int i = names.Name.Count - 1; i >= 0; i--) 
            {
                DsNameMatch.NameRow row = names.Name[i];
                if (row["Rank"].ToString().Trim() != pnRank.ToString().Trim())
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }

    }
}
