using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Common;
using NZOR.Data.Repositories.Common;
using System.Data;
using System.Data.SqlClient;

namespace NZOR.Data.Sql.Repositories.Common
{
    public class DeprecatedRepository : IDeprecatedRepository
    {
        private String _connectionString;

        public DeprecatedRepository(String connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertDeprecatedRecord(string table, Guid oldId, Guid? newId, DateTime deprecationDate)
        {
            string sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Common.Deprecated-INSERT.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@table", table);
                    cmd.Parameters.AddWithValue("@oldId", oldId);
                    cmd.Parameters.AddWithValue("@newId", (object)newId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@deprecationDate", deprecationDate);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
