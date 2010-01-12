using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

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

    }
}
