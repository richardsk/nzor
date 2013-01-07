using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace NZOR.Web.Service.Helpers
{
    public static class Utility
    {
        /// <summary>
        /// Attempts to parse a given date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Null if the date cannot be parsed according to the predefined formats.</returns>
        public static DateTime? ParseDate(string date)
        {
            DateTime parsedDate;

            string[] formats = { "yyyy-MM-dd", "yyyy-M-d", "yyyyMMdd" };

            if (String.IsNullOrWhiteSpace(date) || !DateTime.TryParseExact(date.Trim(), formats, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                return null;
            }

            return parsedDate;
        }
    }
}