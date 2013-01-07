using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Server
{
    public class Log
    {
        public static void LogEvent(string message)
        {
            string msg = DateTime.Now.ToString("s") + " : " + message;
            System.IO.File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "log.txt"), msg + Environment.NewLine);            
        }
    }
}
