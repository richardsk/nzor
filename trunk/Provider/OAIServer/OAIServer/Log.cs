using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OAIServer
{
    public class Log
    {
        public static void LogError(Exception ex)
        {
            try
            {
                string file = System.Configuration.ConfigurationManager.AppSettings["LOG_FILE"];
                string msg = DateTime.Now.ToString() + " : ERROR : " + ex.Message + " : " + ex.StackTrace + Environment.NewLine;
                System.IO.File.AppendAllText(file, msg);
            }
            catch (Exception)
            { }
        }

        public static void LogMessage(String msg)
        {
            try
            {
                string file = System.Configuration.ConfigurationManager.AppSettings["LOG_FILE"];
                msg = DateTime.Now.ToString() + " : " + msg + Environment.NewLine;
                System.IO.File.AppendAllText(file, msg);
            }
            catch (Exception)
            { }
        }
    }
}
