using System.Data.SqlClient;
using System.Data;
using System;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithPartialYear : INameMatcher
    {

        private int m_Id = -1;
        private int m_FailId = -1;
        private int m_SuccessId = -1;
        private int m_Threshold = 100;

        public NamesWithPartialYear()
        {
        }

        public NamesWithPartialYear(int id, int failId, int successId, int threshold)
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
            object pYr = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Year);

            if (pYr == System.DBNull.Value || pYr.ToString().Length == 0) return;
            //succeed 

            String pnYear = pYr.ToString().Trim();
            pnYear = pnYear.Replace("  ", " ");
            pnYear = pnYear.Replace("[", "");
            pnYear = pnYear.Replace("]", "");
            pnYear = pnYear.Replace("\"", "");
            pnYear = pnYear.Replace("?", "");

            for (int i = names.Name.Count - 1; i >= 0; i--)
            {
                DsNameMatch.NameRow row = names.Name[i];
                String yr = row["Year"].ToString().Trim();
                yr = yr.Replace("  ", " ");
                yr = yr.Replace("[", "");
                yr = yr.Replace("]", "");
                yr = yr.Replace("\"", "");
                yr = yr.Replace("?", "");

                if (yr != pnYear)
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }

    }
}
