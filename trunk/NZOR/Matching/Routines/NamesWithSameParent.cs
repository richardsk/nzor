using System;
using System.Data;
using System.Data.SqlClient;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithSameParent : INameMatcher
    {

        private int m_Id = -1;
        private int m_FailId = -1;
        private int m_SuccessId = -1;
        private int m_Threshold = -1;

        public NamesWithSameParent()
        {
        }

        public NamesWithSameParent(int id, int failId, int successId, int threshold)
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
            System.Data.DataRow parRow = NZOR.Data.ProviderName.GetNameConcept(pn.Tables["Concepts"], NZOR.Data.ConceptProperties.ParentRelationshipType);

            DsNameMatch ds = new DsNameMatch();

            if (!parRow.IsNull("ConsensusNameToID"))
            {
                System.Guid parentId = (System.Guid)parRow["ConsensusNameToID"];

                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    cnn.Open();

                    ds = NZOR.Data.ConsensusName.GetNamesWithConcept(cnn, NZOR.Data.ConceptProperties.ParentRelationshipType, parentId);

                    if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
                }
            }

            return ds;
        }

        public void RemoveNonMatches(DataSet pn, ref DsNameMatch names)
        {
            System.Data.DataRow parRow = NZOR.Data.ProviderName.GetNameConcept(pn.Tables["Concepts"], NZOR.Data.ConceptProperties.ParentRelationshipType);

            DsNameMatch ds = new DsNameMatch();

            if (!parRow.IsNull("ConsensusNameToID"))
            {
                System.Guid parentId = (System.Guid)parRow["ConsensusNameToID"];

                foreach (DsNameMatch.NameRow nmRow in names.Name)
                {
                    if (nmRow.ParentID != parentId) nmRow.Delete();
                }
            }

            ds.AcceptChanges();
        }
    }
}
