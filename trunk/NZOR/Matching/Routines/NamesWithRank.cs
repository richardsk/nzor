using System.Data.SqlClient;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithRank : INameMatcher
    {

        private int m_Id = -1;
        private int m_FailId = -1;
        private int m_SuccessId = -1;
        private int m_Threshold = -1;

        public NamesWithRank()
        {
        }

        public NamesWithRank(int id, int failId, int successId, int threshold)
        {
            m_Id = id;
            m_FailId = failId;
            m_SuccessId = successId;
            m_Threshold = threshold;
        }

        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public int FailId
        {
            get { return m_FailId; }
            set { m_FailId = value; }
        }

        public int SuccessId
        {
            get { return m_SuccessId; }
            set { m_SuccessId = value; }
        }

        public int Threshold
        {
            get { return m_Threshold; }
            set { m_Threshold = value; }
        }


        public DsNameMatch GetMatchingNames(System.Data.DataSet pn)
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

        public void RemoveNonMatches(System.Data.DataSet pn, ref DsNameMatch names)
        {
            object pnRank = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Rank);

            foreach (DsNameMatch.NameRow row in names.Name)
            {
                if (row["Rank"].ToString().Trim() != pnRank.ToString().Trim())
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }

    }
}
