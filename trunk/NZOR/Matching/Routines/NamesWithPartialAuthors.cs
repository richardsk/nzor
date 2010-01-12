using System.Data.SqlClient;
using System.Data;
using System;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithPartialAuthors : INameMatcher
    {

        private int m_Id = -1;
        private int m_FailId = -1;
        private int m_SuccessId = -1;
        private int m_Threshold = 100;

        public NamesWithPartialAuthors()
        {
        }

        public NamesWithPartialAuthors(int id, int failId, int successId, int threshold)
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
            object auth = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Authors);

            if (auth == System.DBNull.Value || auth.ToString().Length == 0) return;
            //succeed 

            String authors = auth.ToString().Trim();
            authors = authors.Replace("  ", " ");
            authors = authors.Replace(" et ", " & ");
            if (authors.IndexOf(" ex ") != -1) authors = authors.Substring(authors.IndexOf(" ex ") + 4);

            foreach (DsNameMatch.NameRow row in names.Name)
            {
                String nameAuth = row["Authors"].ToString().Trim();
                nameAuth = nameAuth.Replace("  ", " ");
                nameAuth = nameAuth.Replace(" et ", " & ");
                if (nameAuth.IndexOf(" ex ") != -1) nameAuth = nameAuth.Substring(nameAuth.IndexOf(" ex ") + 4);

                if (Utility.LevenshteinPercent(authors, nameAuth) < m_Threshold)
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }

    }
}
