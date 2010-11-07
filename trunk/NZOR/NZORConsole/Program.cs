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


                    _logFile = System.IO.File.CreateText(@"C:\Development\NZOR\Dev\NZOR\NZORConsole\log.txt");
                    IntegratorThread.LogFile = _logFile;

                    XmlDocument doc = new XmlDocument();
                    doc.Load(args[1]);
                    
                    // ----- DB version -----
                    IntegrationProcessor.MaxThreads = 1; //try one name at a time
                    NZOR.Integration.IntegrationProcessor.RunIntegration(doc);

                    while (NZOR.Integration.IntegrationProcessor.Progress != 100)
                    {
                        System.Threading.Thread.Sleep(2000);
                    }

                    // ----- end of DB version -----


                    // ----- non db version -----
                    //string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
                    //SqlConnection cnn = new SqlConnection(cnnStr);
                    
                    //cnn.Open();
                    //DsIntegrationName data = ProviderName.GetAllDataForIntegration(cnn);
                    //cnn.Close();
                    
                    //data.AcceptChanges();
                    //IntegrationProcessor2.RunIntegration(doc, data);

                    //while (NZOR.Integration.IntegrationProcessor2.Progress != 100)
                    //{
                    //    System.Threading.Thread.Sleep(2000);
                    //}

                    ////save results to DB
                    //cnn.Open();
                    //NZOR.Data.Integration.SaveIntegrationData(cnn, data);
                    //cnn.Close();

                    // ----- end of non DB version  -----

                    _logFile.Close();
                }
            }

        }

        static void  Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (NZOR.Data.Integration.Progress == 100)
            {
                //finished
                Console.WriteLine("Completed");
                Console.WriteLine("Press a key to exit");
                Console.ReadKey();
            }
            else if (NZOR.Integration.IntegrationProcessor2.Progress == 100)
            {
                //up to saving
                Console.WriteLine("Saving data ... " + NZOR.Data.Integration.Progress.ToString() + "%");
                _t.Start();
            }
            else
            {
                //still integrating
                Console.WriteLine(NZOR.Integration.IntegrationProcessor2.StatusText);
                _t.Start();
            }
        }

    }
}
