using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Dynamic;
using System.Text;
using System.Linq;

namespace Database.Test.Helper.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Turns the target into an Expando object.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static dynamic ToExpando(this Object target)
        {
            if (target.GetType() == typeof(ExpandoObject))
            {
                return target;
            }

            var result = new ExpandoObject();
            var dictionary = result as IDictionary<String, Object>;

            if (target.GetType() == typeof(NameValueCollection) || target.GetType().IsSubclassOf(typeof(NameValueCollection)))
            {
                var nameValues = (NameValueCollection)target;

                nameValues.Cast<String>().Select(key => new KeyValuePair<string, object>(key, nameValues[key])).ToList().ForEach(i => dictionary.Add(i));
            }
            else
            {
                var properties = target.GetType().GetProperties();

                foreach (var property in properties)
                {
                    dictionary.Add(property.Name, property.GetValue(target, null));
                }
            }

            return result;
        }

        public static void AddParameter(this DbCommand target, KeyValuePair<String, Object> property)
        {
            var parameter = target.CreateParameter();

            parameter.ParameterName = property.Key;

            if (property.Value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = property.Value;
            }

            target.Parameters.Add(parameter);
        }
    }
}

namespace Database.Test.Helper
{
    using Database.Test.Helper.Extensions;
    using System.Data.SqlClient;

    public class DatabaseTestHelper
    {
        DbProviderFactory _factory;
        String _connectionString;

        public DatabaseTestHelper(String connectionString)
        {
            _factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            _connectionString = connectionString;
        }

        /// <summary>
        /// Returns a single object that matches the target criteria.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public dynamic Single(String table, Object target)
        {
            var result = new ExpandoObject();
            var properties = target.ToExpando() as IDictionary<String, Object>;

            using (var cnn = OpenConnection())
            {
                using (var cmd = CreateCommand(cnn, "SELECT TOP 1 * FROM " + table, properties))
                {
                    var rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        var dictionary = result as IDictionary<String, Object>;

                        for (var index = 0; index < rdr.FieldCount; index++)
                        {
                            dictionary.Add(rdr.GetName(index), rdr[index]);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a count of the number of records in the table matching the target criteria.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="target">Set to null to return all records.</param>
        /// <returns></returns>
        public Int32 Count(String table, Object target)
        {
            Int32 result = 0;
            IDictionary<String, Object> properties = null;

            if (target != null)
            {
                properties = target.ToExpando() as IDictionary<String, Object>;
            }

            using (var cnn = OpenConnection())
            {
                using (var cmd = CreateCommand(cnn, "SELECT COUNT(*) FROM " + table, properties))
                {
                    result = (Int32)cmd.ExecuteScalar();
                }
            }

            return result;
        }

        public void ExecuteSql(String sql)
        {
            ExecuteSql(sql, 180);
        }

        public void ExecuteSql(String sql, int timeoutSeconds)
        {
            using (var cnn = OpenConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = timeoutSeconds;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public String GetXmlAsString(String sql)
        {
            String result = String.Empty;

            if (!String.IsNullOrEmpty(sql))
            {
                using (var cnn = OpenConnection())
                {
                    using (var cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;
                        cmd.CommandTimeout = 180;

                        System.Xml.XmlReader rdr = cmd.ExecuteXmlReader();

                        rdr.Read();

                        while (rdr.ReadState != System.Xml.ReadState.EndOfFile)
                        {
                            result += rdr.ReadOuterXml();
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns an open connection.
        /// </summary>
        private SqlConnection OpenConnection()
        {
            var cnn = _factory.CreateConnection();

            cnn.ConnectionString = _connectionString;
            cnn.Open();

            return cnn as SqlConnection;
        }

        private SqlCommand CreateCommand(SqlConnection cnn, String sql, IDictionary<String, Object> properties)
        {
            SqlCommand result = cnn.CreateCommand();

            result.CommandType = System.Data.CommandType.Text;
            result.CommandText = sql;

            if (properties != null)
            {
                String whereClause = BuildWhereClause(properties);

                result.CommandText += " " + whereClause;

                foreach (var property in properties)
                {
                    result.AddParameter(property);
                }
            }

            return result;
        }

        private String BuildWhereClause(IDictionary<String, Object> properties)
        {
            var whereClause = new StringBuilder();

            foreach (var property in properties)
            {
                whereClause.AppendFormat("{0} = @{0} AND ", property.Key);
            }
            if (whereClause.Length > 0)
            {
                whereClause.Insert(0, "WHERE ");
                whereClause.Length = whereClause.Length - " AND ".Length;
            }

            return whereClause.ToString();
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data.SqlClient;

//namespace NZOR.Data.Test
//{
//    public class DatabaseTestHelper
//    {
//        private String _connectionString;

//        public DatabaseTestHelper(String connectionString)
//        {
//            _connectionString = connectionString;
//        }





//        public void ExecuteSqlScript(SqlScript script)
//        {
//            String sql = String.Empty;

//            switch (script)
//            {
//                case SqlScript.ResetDatabase:
//                    sql = GetSQL(@"NZOR.Data.Test.Resources.Sql.Reset Database.sql");
//                    break;
//                case SqlScript.ResetProviderConcept:
//                    sql = GetSQL(@"NZOR.Data.Test.Resources.Sql.Reset Provider Concept.sql");
//                    break;
//                case SqlScript.InsertProviderData:
//                    sql = GetSQL(@"NZOR.Data.Test.Resources.Sql.Insert Provider Data.sql");
//                    break;
//                case SqlScript.InsertBaseProviderNameData:
//                    sql = GetSQL(@"NZOR.Data.Test.Resources.Sql.Insert Base Provider Name Data.sql");
//                    break;
//                case SqlScript.InsertBaseProviderReferenceData:
//                    sql = GetSQL(@"NZOR.Data.Test.Resources.Sql.Insert Base Provider Reference Data.sql");
//                    break;
//                case SqlScript.InsertBaseProviderConceptData:
//                    sql = GetSQL(@"NZOR.Data.Test.Resources.Sql.Insert Base Provider Concept Data.sql");
//                    break;
//                case SqlScript.InsertBaseConsensusNameData:
//                    sql = GetSQL(@"NZOR.Data.Test.Resources.Sql.Insert Base Consensus Name Data.sql");
//                    break;
//                default:
//                    break;
//            }

//            if (!String.IsNullOrEmpty(sql))
//            {
//                using (SqlConnection cnn = new SqlConnection(_connectionString))
//                {
//                    cnn.Open();

//                    using (SqlCommand cmd = cnn.CreateCommand())
//                    {
//                        cmd.CommandType = System.Data.CommandType.Text;
//                        cmd.CommandText = sql;
//                        cmd.CommandTimeout = 120;

//                        cmd.ExecuteNonQuery();
//                    }
//                }
//            }
//        }

//        public Int32 GetRecordCount(String tableName, String columnName, Object value)
//        {
//            return GetRecordCountInternal(String.Format(@"SELECT COUNT(*) FROM {0} WHERE {1} = '{2}'", tableName, columnName, value.ToString()));
//        }

//        public Int32 GetRecordCount(String tableName)
//        {
//            return GetRecordCountInternal(String.Format(@"SELECT COUNT(*) FROM {0}", tableName));
//        }

//        private Int32 GetRecordCountInternal(String sql)
//        {
//            Int32 count = 0;

//            using (SqlConnection cnn = new SqlConnection(_connectionString))
//            {
//                cnn.Open();

//                using (SqlCommand cmd = cnn.CreateCommand())
//                {
//                    cmd.CommandType = System.Data.CommandType.Text;
//                    cmd.CommandText = sql;

//                    count = (Int32)cmd.ExecuteScalar();
//                }
//            }

//            return count;
//        }

//        public Dictionary<String, Object> GetRecordValues(String sql, List<SqlParameter> sqlParameters)
//        {
//            Dictionary<String, Object> values = null;

//            using (SqlConnection cnn = new SqlConnection(_connectionString))
//            {
//                cnn.Open();

//                using (SqlCommand cmd = cnn.CreateCommand())
//                {
//                    cmd.CommandType = System.Data.CommandType.Text;
//                    cmd.CommandText = sql;
//                    if (sqlParameters != null && sqlParameters.Count > 0)
//                    {
//                        cmd.Parameters.AddRange(sqlParameters.ToArray());
//                    }

//                    using (SqlDataReader drd = cmd.ExecuteReader())
//                    {
//                        if (drd.Read())
//                        {
//                            values = new Dictionary<String, Object>();

//                            for (Int32 columnIndex = 0; columnIndex <= drd.FieldCount - 1; columnIndex++)
//                            {
//                                if (drd.IsDBNull(columnIndex))
//                                {
//                                    values.Add(drd.GetName(columnIndex), null);
//                                }
//                                else
//                                {
//                                    values.Add(drd.GetName(columnIndex), drd.GetValue(columnIndex));
//                                }
//                            }
//                        }
//                    }
//                }
//            }

//            return values;
//        }

//        /// <summary>
//        /// Returns a dictionary (column name/value) of the database vales for a record.
//        /// </summary>
//        /// <param name="tableName"></param>
//        /// <param name="columnName"></param>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public Dictionary<String, Object> GetRecordValues(String tableName, String columnName, String value)
//        {
//            return this.GetRecordValues(String.Format("SELECT * FROM {0} WHERE {1} = '{2}';", tableName, columnName, value), null);
//        }


//    }
//}
