using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.EntityClient;

namespace NZOR.Data
{
    public class ProviderName
    {
        private static String _cnnStr = "";

        public static String ConnectionString
        {
            get 
            {
                if (_cnnStr == "") _cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
                return _cnnStr; 
            }
            set { _cnnStr = value; }

        }

        public static System.Data.DataSet GetNameMatchData(Guid provNameId)
        {
            System.Data.DataSet ds = new System.Data.DataSet();

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();

                SqlCommand cmd = new SqlCommand("sprSelect_ProvNameMatchingData", cnn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@provNameId", System.Data.SqlDbType.UniqueIdentifier).Value = provNameId;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                ds.Tables[0].TableName = "Name";
                ds.Tables[1].TableName = "NameProperty";
                ds.Tables[2].TableName = "Concepts";
            }

            return ds;
        }

        public static Object GetNamePropertyValue(System.Data.DataTable namePropDt, String field)
        {
            Object val = DBNull.Value;

            foreach (System.Data.DataRow dr in namePropDt.Rows)
            {
                if (dr["PropertyName"].ToString() == field)
                {
                    val = dr["Value"];
                    break;
                }
            }

            return val;
        }

        public static System.Data.DataRow GetNameConcept(System.Data.DataTable conceptsDt, String conceptType)
        {
            System.Data.DataRow r = null;

            foreach (System.Data.DataRow dr in conceptsDt.Rows)
            {
                if (dr["Relationship"].ToString() == conceptType)
                {
                    r = dr;
                    break;
                }
            }

            return r;
        }

        public static void UpdateProviderNameLink(DataSet provName, Guid? consensusNameID, LinkStatus status, int matchScore)
        {
            NZOR.Data.Provider.NZORProvider provData = new NZOR.Data.Provider.NZORProvider();
            Guid id = new Guid(provName.Tables["Name"].Rows[0]["NameID"].ToString());

            var res = from pns in provData.Name where pns.NameID == id select pns;

            if (res != null && res.First() != null)
            {
                NZOR.Data.Provider.Name pn = (NZOR.Data.Provider.Name)res.First();
                pn.ConsensusNameID = consensusNameID;
                pn.LinkStatus = status.ToString();
                pn.MatchScore = matchScore;
                provData.SaveChanges();
            }
        }
    }
}
