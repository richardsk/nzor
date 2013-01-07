using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Test
{
    public static class DatabaseTestSql
    {
        public enum SqlScript
        {
            /// <summary>
            /// Resets all provider tables
            /// </summary>
            ResetDatabase,

            ResetProviderConcept,

            /// <summary>
            /// Inserts a base set of provider and dataset data for testing
            /// </summary>
            InsertProviderData,

            InsertBaseProviderNameData,
            InsertBaseProviderReferenceData,
            InsertBaseProviderConceptData,

            InsertBaseConsensusNameData,
            InsertTestConsensusNameData,
            InsertTestAdminData,

            GetNZORTestLargeDataset
        }

        public static String GetSql(SqlScript script)
        {
            String sql = String.Empty;

            switch (script)
            {
                case SqlScript.ResetDatabase:
                    sql = GetSql(@"NZOR.Data.Test.Resources.Sql.Reset Database.sql");
                    break;
                case SqlScript.ResetProviderConcept:
                    sql = GetSql(@"NZOR.Data.Test.Resources.Sql.Reset Provider Concept.sql");
                    break;
                case SqlScript.InsertProviderData:
                    sql = GetSql(@"NZOR.Data.Test.Resources.Sql.Insert Provider Data.sql");
                    break;
                case SqlScript.InsertBaseProviderNameData:
                    sql = GetSql(@"NZOR.Data.Test.Resources.Sql.Insert Base Provider Name Data.sql");
                    break;
                case SqlScript.InsertBaseProviderReferenceData:
                    sql = GetSql(@"NZOR.Data.Test.Resources.Sql.Insert Base Provider Reference Data.sql");
                    break;
                case SqlScript.InsertBaseProviderConceptData:
                    sql = GetSql(@"NZOR.Data.Test.Resources.Sql.Insert Base Provider Concept Data.sql");
                    break;
                case SqlScript.InsertBaseConsensusNameData:
                    sql = GetSql(@"NZOR.Data.Test.Resources.Sql.Insert Base Consensus Name Data.sql");
                    break;
                case SqlScript.GetNZORTestLargeDataset:
                    sql = GetSql(@"NZOR.Data.Test.Resources.Sql.NZOR_Test XML Large Dataset.sql");
                    break;
                case SqlScript.InsertTestConsensusNameData:
                    sql = GetSql(@"NZOR.Data.Test.Resources.Sql.Insert Test Concensus Name Data.sql");
                    break;
                case SqlScript.InsertTestAdminData:
                    sql = GetSql(@"NZOR.Data.Test.Resources.Sql.Insert Test Admin Data.sql");
                    break;
                default:
                    break;
            }

            return sql;
        }

        private static String GetSql(String sqlName)
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
    }
}
