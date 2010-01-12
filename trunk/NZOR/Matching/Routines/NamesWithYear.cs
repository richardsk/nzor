using System.Data.SqlClient;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithYear : INameMatcher
    {

        private int m_Id = -1;
        private int m_FailId = -1;
        private int m_SuccessId = -1;
        private int m_Threshold = -1;

        public NamesWithYear()
        {
        }

        public NamesWithYear(int id, int failId, int successId, int threshold)
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
            //todo
            return null;
        }

        public void RemoveNonMatches(System.Data.DataSet pn, ref DsNameMatch names)
        {
            object pnYear = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Year);

            if (pnYear == System.DBNull.Value || pnYear.ToString().Length == 0) return;
            //succeed 

            foreach (DsNameMatch.NameRow row in names.Name)
            {
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

                    foreach (DsNameMatch.NameRow row in names.Name)
                    {
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
