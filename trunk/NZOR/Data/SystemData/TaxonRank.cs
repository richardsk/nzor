using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace NZOR.Data.SystemData
{
    public class TaxonRank
    {
        public Guid TaxonRankId = Guid.Empty;
        public String Name = string.Empty;
        public int SortOrder = 0;

        public void Load(DataRow row)
        {
            TaxonRankId = (Guid)row["TaxonRankId"];
            Name = row["Name"].ToString();
            SortOrder = (int)row["SortOrder"];
        }
    }

    public class TaxonRankData
    {
        private static TaxonRank _genusRank = null;

        public static TaxonRank GenusRank(SqlConnection cnn)
        {
            if (_genusRank == null)
            {

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select * from TaxonRank where Name = 'genus'";

                    System.Data.DataSet ds = new System.Data.DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        _genusRank = new TaxonRank();
                        _genusRank.Load(ds.Tables[0].Rows[0]);
                    }
                }
            }
            return _genusRank;
        }

        public static SystemData.TaxonRank GetTaxonRank(SqlConnection cnn, Guid taxonRankID)
        {
            TaxonRank tr = null;

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "select * from TaxonRank where TaxonRankId = '" + taxonRankID.ToString() + "'";

                System.Data.DataSet ds = new System.Data.DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    tr = new TaxonRank();
                    tr.Load(ds.Tables[0].Rows[0]);
                }
            }

            return tr;
        }
    }
}
