using System.Data.SqlClient;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithExactCanonical : INameMatcher
    {

        private int m_Id = -1;
        private int m_FailId = -1;
        private int m_SuccessId = -1;
        private int m_Threshold = -1;

        public NamesWithExactCanonical()
        {
        }

        public NamesWithExactCanonical(int id, int failId, int successId, int threshold)
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
            object pnCanonical = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Canonical);

            foreach (DsNameMatch.NameRow row in names.Name)
            {
                if (row.Canonical.Trim() != pnCanonical.ToString().Trim())
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }


    }
}
