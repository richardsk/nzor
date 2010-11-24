using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data.SqlClient;

using NZOR.Data;
using NZOR.Integration;

namespace NZORConsole
{
    public class Program
    {
        static System.Timers.Timer _t = new System.Timers.Timer();
                
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage : NZORConsole.exe [action] [config file] [data file] : where [action] can be Integrate, [config file] is the NZOR Integration Rules config file and [data file] is the data file to process.");                
            }
            else
            {
                if (args[0].ToUpper() == "INTEGRATE")
                {
                    try
                    {
                        _t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
                        _t.Interval = 8000;
                        _t.Start();

                        RunIntegration(args[1], args[2]);

                        //log
                        System.IO.File.WriteAllLines("log.txt", IntegratorThread.Log);

                    }
                    catch (Exception ex)
                    {
                        System.IO.File.WriteAllLines("log.txt", IntegratorThread.Log);
                        System.IO.File.AppendAllText("log.txt", DateTime.Now.ToString() + " : ERROR running integration : " + ex.Message + " : " + ex.StackTrace);
                    }
                }
            }

        }

        static int Progress()
        {
            //return IntegrationProcessor.Progress;
            return IntegrationProcessor2.Progress; //non-db
        }

        static int SaveProgress()
        {
            //return 100;
            return Integration.Progress; //non-db
        }

        static String Status()
        {
            //return IntegrationProcessor.StatusText;
            return IntegrationProcessor2.StatusText; //non-db
        }

        static void RunIntegration(string configFilePath, string dataFilePath)
        {
            // ----- DB version -----
            //IntegrationProcessor.MaxThreads = 100; //try one name at a time
            //NZOR.Integration.IntegrationProcessor.RunIntegration(doc);

            //while (Progress() != 100)
            //{
            //    System.Threading.Thread.Sleep(2000);
            //}

            // ----- end of DB version -----


            // ----- non db version -----
            //string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            //SqlConnection cnn = new SqlConnection(cnnStr);

            //cnn.Open();
            //DsIntegrationName data = ProviderName.GetAllDataForIntegration(cnn);
            //cnn.Close();
            
            IntegrationProcessor2.RunIntegration(configFilePath, dataFilePath);

            while (Progress() != 100)
            {
                System.Threading.Thread.Sleep(2000);
            }

            //save results to DB
            //cnn.Open();
            //NZOR.Data.Integration.SaveIntegrationData(cnn, data);
            //cnn.Close();
            
            // ----- end of non DB version  -----
        }

        static void  Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (SaveProgress() == 100 && Progress() == 100)
            {
                //finished
                Console.WriteLine("Completed");
                Console.WriteLine("Press a key to exit");
                Console.ReadKey();
            }
            else if (Progress() == 100)
            {
                //up to saving
                Console.WriteLine("Saving data ... " + SaveProgress().ToString() + "%");
                _t.Start();
            }
            else
            {
                //still integrating
                Console.WriteLine(Status());
                _t.Start();
            }
        }

    }
}
