using System.Data.SqlClient;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithAuthors : INameMatcher
    {

        private int m_Id = -1;
        private int m_FailId = -1;
        private int m_SuccessId = -1;
        private int m_Threshold = -1;

        public NamesWithAuthors()
        {
        }

        public NamesWithAuthors(int id, int failId, int successId, int threshold)
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


        public DsNameMatch GetMatchingNames(DataSet pn)
        {
            return null;
        }

        public void RemoveNonMatches(DataSet pn, ref DsNameMatch names)
        {
            //TODO :
            // - full, basionym, combination author matches 
            // - corrected authors / lookup
            // may need another table on pn dataset for Authors??

            object authors = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NameProperties.Authors);

            if (authors != System.DBNull.Value)
            {
                for (int i = names.Name.Count - 1; i >= 0; i--)
                {
                    DsNameMatch.NameRow row = names.Name[i];
                    if (!row.IsAuthorsNull() && row.Authors.Length > 0 && row.Authors.ToLower().Trim() != authors.ToString().ToLower().Trim())
                    {
                        row.Delete();
                    }
                }

                if (names.Name.Rows.Count == 0)
                {
                    //try prov names 
                    names.RejectChanges();

                    using (SqlConnection cnn = new SqlConnection())
                    {
                        for (int i = names.Name.Count - 1; i >= 0; i--)
                        {
                            DsNameMatch.NameRow row = names.Name[i];
                            if (ConsensusName.HasProviderValue(cnn, row.NameID, NameProperties.Authors, authors) == false)
                            {
                                row.Delete();
                            }
                        }

                        if (cnn.State != ConnectionState.Closed) cnn.Close();
                    }
                }

            }

            names.AcceptChanges();
        }

    }
}