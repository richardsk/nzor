using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace NZOR.Admin.Data.Sql.Helpers
{
    public static class Utility
    {
        public static String GetSQL(String sqlName)
        {
            String sql = String.Empty;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using (System.IO.Stream stream = assembly.GetManifestResourceStream(sqlName))
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(stream))
                {
                    sql = streamReader.ReadToEnd();
                }
            }

            return sql;
        }

        public static DataTable GetSourceData(String sourceConnectionString, String commandText, int? timeoutSeconds)
        {
            return GetSourceData(sourceConnectionString, commandText, null, timeoutSeconds);
        }
        
        public static DataTable GetSourceData(String sourceConnectionString, String commandText)
        {
            return GetSourceData(sourceConnectionString, commandText, null, null);
        }

        public static DataTable GetSourceData(String sourceConnectionString, String commandText, List<SqlParameter> parameters, int? timeoutSeconds)
        {
            DataTable tbl = new DataTable();

            using (SqlConnection cnn = new SqlConnection(sourceConnectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = commandText;
                    if (timeoutSeconds.HasValue) cmd.CommandTimeout = timeoutSeconds.Value;
                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    using (SqlDataAdapter dad = new SqlDataAdapter(cmd))
                    {
                        dad.Fill(tbl);
                    }
                }
                cnn.Close();
            }

            return tbl;
        }

        public static DataTable GetSourceData(String sourceConnectionString, String commandText, List<SqlParameter> parameters)
        {
            return GetSourceData(sourceConnectionString, commandText, parameters, null);
        }
    }

}
