using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Integration
{
    class Log
    {
        public static void LogError(Exception ex)
        {
            System.Diagnostics.EventLog.WriteEntry("Application", ex.Message + " : " + ex.StackTrace);
        }
    }
}
