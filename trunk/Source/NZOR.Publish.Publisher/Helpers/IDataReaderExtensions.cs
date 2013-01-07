using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NZOR.Publish.Publisher.Helpers
{
    public static class IDataReaderExtensions
    {
        public static Boolean IsDBNull(this IDataReader target, string columnName)
        {
            int columnIndex = target.GetOrdinal(columnName);

            return target.IsDBNull(columnIndex);
        }

        public static Guid GetGuid(this IDataReader target, string columnName)
        {
            int columnIndex = target.GetOrdinal(columnName);

            if (target.IsDBNull(columnIndex))
            {
                return Guid.Empty;
            }
            else
            {
                return target.GetGuid(columnIndex);
            }
        }

        public static Guid? GetNullableGuid(this IDataReader target, string columnName)
        {
            int columnIndex = target.GetOrdinal(columnName);

            if (target.IsDBNull(columnIndex))
            {
                return null;
            }
            else
            {
                return target.GetGuid(columnIndex);
            }
        }

        public static string GetString(this IDataReader target, string columnName)
        {
            int columnIndex = target.GetOrdinal(columnName);

            if (target.IsDBNull(columnIndex))
            {
                return String.Empty;
            }
            else
            {
                return target.GetString(columnIndex);
            }
        }

        public static Decimal? GetNullableDecimal(this IDataReader target, string columnName)
        {
            int columnIndex = target.GetOrdinal(columnName);

            if (target.IsDBNull(columnIndex))
            {
                return null;
            }
            else
            {
                return target.GetDecimal(columnIndex);
            }
        }

        public static DateTime? GetNullableDateTime(this IDataReader target, string columnName)
        {
            int columnIndex = target.GetOrdinal(columnName);

            if (target.IsDBNull(columnIndex))
            {
                return null;
            }
            else
            {
                return target.GetDateTime(columnIndex);
            }
        }

        public static DateTime GetDateTime(this IDataReader target, string columnName)
        {
            int columnIndex = target.GetOrdinal(columnName);

            return target.GetDateTime(columnIndex);
        }

        public static Int32 GetInt32(this IDataReader target, string columnName)
        {
            int columnIndex = target.GetOrdinal(columnName);

            return target.GetInt32(columnIndex);
        }

        public static Int64 GetInt64(this IDataReader target, string columnName)
        {
            int columnIndex = target.GetOrdinal(columnName);

            return target.GetInt64(columnIndex);
        }

        public static Double GetDouble(this IDataReader target, string columnName)
        {
            int columnIndex = target.GetOrdinal(columnName);

            return target.GetDouble(columnIndex);
        }

        public static bool GetBoolean(this IDataReader target, string columnName)
        {
            int columnIndex = target.GetOrdinal(columnName);

            return target.GetBoolean(columnIndex);
        }
    }
}

