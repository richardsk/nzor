using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;

namespace NZOR.Publish.Publisher.Helpers
{
    static class DataAccess
    {
        public static SqlDataReader ExecuteReader(string connectionString, string sql, List<SqlParameter> parameters = null)
        {
            var cnn = new SqlConnection(connectionString);

            cnn.Open();

            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = sql;
                cmd.CommandTimeout = 600;

                if (parameters != null && parameters.Count > 0)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            }
        }
    }
}
