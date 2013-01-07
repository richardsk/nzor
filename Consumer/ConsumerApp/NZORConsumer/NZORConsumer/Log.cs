using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZORConsumer
{
    public class Log
    {
        public static void LogError(Exception ex)
        {
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NZOR");
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

            string msg = DateTime.Now.ToString("s") + " : ERROR : " + ex.Message + " : " + ex.StackTrace;
            if (ex.InnerException != null)
            {
                msg += " : AT : " + ex.InnerException.Message + " : " + ex.InnerException.StackTrace;
            }

            System.IO.File.WriteAllText(System.IO.Path.Combine(path, "log.txt"), msg + Environment.NewLine);
        }
    }
}
