using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using NZOR.Integration;
using NZOR.Data.Sql;
using NZOR.Data.DataSets;

namespace NZOR.Command
{
    public static class Program
    {
        static System.Timers.Timer _t = new System.Timers.Timer();
        
        public static void Main(string[] args)
        {
            if ((args.Length < 3 || args.Length > 4) && (args.Length == 0 || !args[0].ToUpper().StartsWith("HARVEST") || !args[0].ToUpper().StartsWith("MATCH")))
            {
                Console.WriteLine("Usage : NZORConsole.exe [action] [param 1] [param 2] [partam 3] : where [action] can be Match, Integrate, Harvest, HarvestInit [param 1] is the NZOR Integration Rules config file or the provider code for the HarvestInit call, [param 2] is the data file to process and [param 3] is the provider code if given for data to integrate, or the output for CSV matching.");
                Environment.Exit(1);
            }
            else
            {
                if (args[0].ToUpper() == "MATCH")
                {
                    try
                    {
                        _t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
                        _t.Interval = 8000;
                        _t.Start();

                        Admin.Data.Process.ProcessResult res = NZOR.Server.Process.RunMatching(args[1], args[2], args[3]);
                        if (res != null && res.Result == Admin.Data.Process.ProcessResultOutcome.Failed)
                        {
                            string msg = "";
                            res.ErrorMessages.ForEach(m => msg += m + ";");
                            Console.WriteLine("Failed : " + msg);
                            System.IO.File.WriteAllText("log.txt", msg);
                        }
                        else
                        {
                            Console.WriteLine("Completed Successfully");
                        }

                        //log
                        System.IO.File.WriteAllLines("log.txt", IntegratorThread.Log);

                    }
                    catch (Exception ex)
                    {
                        System.IO.File.WriteAllLines("log.txt", IntegratorThread.Log);
                        System.IO.File.AppendAllText("log.txt", DateTime.Now.ToString() + " : ERROR running matching : " + ex.Message + " : " + ex.StackTrace);
                        Environment.Exit(1);
                    }
                }
                else if (args[0].ToUpper() == "INTEGRATE")
                {
                    try
                    {
                        _t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
                        _t.Interval = 8000;
                        _t.Start();

                        string pc = null;
                        if (args.Length == 4) pc = args[3];

                        Admin.Data.Process.ProcessResult res = NZOR.Server.Process.RunIntegration(args[1], args[2], pc, true, false);
                        if (res != null && res.Result == Admin.Data.Process.ProcessResultOutcome.Failed)
                        {
                            string msg = "";
                            res.ErrorMessages.ForEach(m => msg += m + ";");
                            Console.WriteLine("Failed : " + msg);
                            System.IO.File.WriteAllText("log.txt", msg);
                        }
                        else
                        {
                            Console.WriteLine("Completed Successfully");
                        }

                        _t.Stop();

                    }
                    catch (Exception ex)
                    {
                        System.IO.File.WriteAllLines("log.txt", IntegratorThread.Log);
                        System.IO.File.AppendAllText("log.txt", DateTime.Now.ToString() + " : ERROR running integration : " + ex.Message + " : " + ex.StackTrace);
                        Environment.Exit(1);
                    }
                }
                else if (args[0].ToUpper() == "HARVEST")
                {
                    try
                    {
                        _t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
                        _t.Interval = 8000;
                        _t.Start();

                        Admin.Data.Process.ProcessResult res = NZOR.Server.Process.RunHarvest(true);
                        if (res != null && res.Result == Admin.Data.Process.ProcessResultOutcome.Failed)
                        {
                            string msg = "";
                            res.ErrorMessages.ForEach(m => msg += m + ";");
                            Console.WriteLine("Failed : " + msg);
                            System.IO.File.WriteAllText("log.txt", msg);
                        }
                        else
                        {
                            Console.WriteLine("Completed Successfully");
                        }

                        _t.Stop();

                    }
                    catch (Exception ex)
                    {
                        System.IO.File.WriteAllLines("log.txt", IntegratorThread.Log);
                        System.IO.File.AppendAllText("log.txt", DateTime.Now.ToString() + " : ERROR running harvest : " + ex.Message + " : " + ex.StackTrace);
                        Environment.Exit(1);
                    }
                }
                else if (args[0].ToUpper() == "HARVESTINIT")
                {
                    try
                    {
                        _t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
                        _t.Interval = 8000;
                        _t.Start();

                        Admin.Data.Process.ProcessResult res = NZOR.Server.Process.RunInitialHarvest(args[1]);
                        if (res != null && res.Result == Admin.Data.Process.ProcessResultOutcome.Failed)
                        {
                            string msg = "";
                            res.ErrorMessages.ForEach(m => msg += m + ";");
                            Console.WriteLine("Failed : " + msg);
                            System.IO.File.WriteAllText("log.txt", msg);
                        }
                        else 
                        {
                            Console.WriteLine("Completed Successfully");
                        }

                        _t.Stop();

                    }
                    catch (Exception ex)
                    {
                        System.IO.File.WriteAllLines("log.txt", IntegratorThread.Log);
                        System.IO.File.AppendAllText("log.txt", DateTime.Now.ToString() + " : ERROR running harvest : " + ex.Message + " : " + ex.StackTrace);
                        Environment.Exit(1);
                    }
                }
            }

        }
        
        static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (NZOR.Server.Process.Finished)
            {
                //finished
                Console.WriteLine("Completed");
                
                //write log
                if (IntegratorThread.Log.Count > 0)
                {
                    System.IO.File.WriteAllLines("log.txt", IntegratorThread.Log);
                    IntegratorThread.Log.Clear();
                }

                //Console.WriteLine("Press a key to exit");
                //Console.ReadKey();

                _t.Stop();
            }
            else
            {
                //still integrating
                Console.WriteLine(NZOR.Server.Process.Status);
                
                _t.Start();
            }
        }

    }
}
