using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesAtRankWithAncestor : INameMatcher
    {

        private int m_Id = -1;
        private int m_FailId = -1;
        private int m_SuccessId = -1;
        private int m_Threshold = -1;

        public NamesAtRankWithAncestor()
        {
        }

        public NamesAtRankWithAncestor(int id, int failId, int successId, int threshold)
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
            //TODO


            //adds matches, not removes them 
            //gets the descendent names with correct rank 

            //DsNameMatch ds = new DsNameMatch();

            //string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            //using (SqlConnection cnn = new SqlConnection(ConnectionString))
            //{
            //    cnn.Open();

            //    string ancestorRank = "";
            //    Guid ancestorId = Guid.Empty;

            //    using (SqlCommand cmd = cnn.CreateCommand())
            //    {
            //        cmd.CommandText = "select ar.rankname from tblrank r inner join tblrank ar on ar.rankpk = r.ancestorrankfk where r.rankpk = " + pn.PNNameRankFk.ToString;

            //        ancestorRank = cmd.ExecuteScalar().ToString();
            //    }

            //    if (!string.IsNullOrEmpty(ancestorRank))
            //    {

            //        using (SqlCommand cmd = cnn.CreateCommand)
            //        {
            //            cmd.CommandText = "sprSelect_ParentAtRank";
            //            cmd.CommandType = CommandType.StoredProcedure;
            //            cmd.Parameters.Add("@nameGuid", SqlDbType.UniqueIdentifier).Value = names.Name(0).NameGuid;
            //            cmd.Parameters.Add("@rank", SqlDbType.NVarChar).Value = ancestorRank;

            //            ancestorId = cmd.ExecuteScalar();
            //        }

            //        if (ancestorId != Guid.Empty)
            //        {

            //            foreach (DsNameMatch.NameRow row in names.Name)
            //            {

            //                using (SqlCommand cmd = cnn.CreateCommand)
            //                {
            //                    cmd.CommandText = "sprSelect_ChildrenAtRank";
            //                    cmd.CommandType = CommandType.StoredProcedure;
            //                    cmd.Parameters.Add("@nameGuid", SqlDbType.UniqueIdentifier).Value = row.NameGuid;
            //                    cmd.Parameters.Add("@rank", SqlDbType.NVarChar).Value = pn.PNNameRank;

            //                    SqlDataAdapter da = new SqlDataAdapter(cmd);
            //                    da.TableMappings.Add("Table", "Name");
            //                    da.Fill(ds);

            //                }
            //            }

            //            names = ds;
            //        }
            //    }

            //    if (cnn.State != ConnectionState.Closed) cnn.Close();

            //}
        }


    }
}
