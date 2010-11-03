using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data.SqlClient;

using NZOR.Data;
using NZOR.Integration;

namespace NZORConsole
{
    class Program
    {
        static System.Timers.Timer _t = new System.Timers.Timer();
        static System.IO.StreamWriter _logFile = null;
        
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage : NZORConsole.exe [action] [config file] : where [action] can be Integrate, [config file] is the NZOR Integration Rule Set config file.");                
            }
            else
            {
                if (args[0].ToUpper() == "INTEGRATE")
                {
                    _t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
                    _t.Interval = 8000;
                    _t.Start();

                    XmlDocument doc = new XmlDocument();
                    doc.Load(args[1]);
                    
                    //NZOR.Integration.IntegrationProcessor.RunIntegration(doc);

                    //non db version
                    string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
                    SqlConnection cnn = new SqlConnection(cnnStr);
                    
                    cnn.Open();
                    DsIntegrationName data = ProviderName.GetAllDataForIntegration(cnn);
                    cnn.Close();

                    _logFile = System.IO.File.CreateText(@"C:\Development\NZOR\Dev\NZOR\NZORConsole\log.txt");
                    IntegratorThread.LogFile = _logFile;

                    IntegrationProcessor2.RunIntegration(doc, data);

                    while (NZOR.Integration.IntegrationProcessor2.Progress != 100)
                    {
                        System.Threading.Thread.Sleep(2000);
                    }

                    _logFile.Close();
                }
            }

        }

        static void  Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine(NZOR.Integration.IntegrationProcessor2.StatusText);
         
            if (NZOR.Integration.IntegrationProcessor2.Progress == 100)
            {
                Console.WriteLine("Completed");
                Console.WriteLine("Press a key to exit");
                Console.ReadKey();
            }
            else
            {
                _t.Start();
            }
        }

    }
}
