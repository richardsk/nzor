using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Admin.Data.Process;

namespace NZOR.Server
{
    public class NZORProcessor
    {
        static System.Timers.Timer _t = new System.Timers.Timer();
        static Process.ProcessType _processState = Process.ProcessType.Unknown;
        //static NZOR.Admin.Data.Entities.ScheduledTask _systemRunTask = null;
        static NZOR.Server.BatchMatchProcessor _batchProcessor;
        static NZOR.Server.NameRequestProcessor _nameRequestProcessor;
        static bool _saving = false;
        static string _lastStatus = "";

        public static bool Complete = false;

        public static void Start()
        {
            try
            {
                //string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
                //string nzorSystemTaskName = System.Configuration.ConfigurationManager.AppSettings["NZOR System Task"];

                //NZOR.Admin.Data.Repositories.IScheduledTaskRepository str = new Admin.Data.Sql.Repositories.ScheduledTaskRepository(cnnStr);
                //_systemRunTask = str.GetScheduledTask(nzorSystemTaskName);

                Complete = false;

                Process.StatusEvent += new Process.StatusEventHandler(ProcessStatusChanged);

                _t.Elapsed += new System.Timers.ElapsedEventHandler(_t_Elapsed);
                _t.AutoReset = false;
                _t.Interval = 1000;
                _t.Start();

                //kick off batch match processor
                _batchProcessor = new NZOR.Server.BatchMatchProcessor();
                _batchProcessor.Run();

                _nameRequestProcessor = new NameRequestProcessor();
                _nameRequestProcessor.Run();

                Log.LogEvent("NZOR Service Started");

                ProcessStatusChanged("Running", null);

            }
            catch (Exception ex)
            {
                Log.LogEvent("ERROR : " + ex.Message + " : " + ex.StackTrace);
            }
        }

        private static void ProcessStatusChanged(string status, string runOutcome)
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            string nzorSystemTaskName = System.Configuration.ConfigurationManager.AppSettings["NZOR System Task"];

            NZOR.Admin.Data.Repositories.IScheduledTaskRepository str = new Admin.Data.Sql.Repositories.ScheduledTaskRepository(cnnStr);
            Admin.Data.Entities.ScheduledTask systemRunTask = str.GetScheduledTask(nzorSystemTaskName);

            if (systemRunTask != null && !_saving)
            {
                _saving = true;

                try
                {
                    if (runOutcome != null)
                    {
                        systemRunTask.LastRunOutcome = runOutcome;
                        systemRunTask.LastRun = DateTime.Now;
                    }

                    systemRunTask.Status = status;
                    systemRunTask.State = Admin.Data.Entities.Entity.EntityState.Modified;                    
                    str.ScheduledTasks.Add(systemRunTask);
                    str.Save();

                    if (status != _lastStatus)
                    {
                        Log.LogEvent(status);
                        _lastStatus = status;
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    _saving = false;
                }
            }
        }

        private static void _t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            string nzorSystemTaskName = System.Configuration.ConfigurationManager.AppSettings["NZOR System Task"];

            NZOR.Admin.Data.Repositories.IScheduledTaskRepository str = new Admin.Data.Sql.Repositories.ScheduledTaskRepository(cnnStr);
            Admin.Data.Entities.ScheduledTask systemRunTask = str.GetScheduledTask(nzorSystemTaskName);

            //process?
            if (systemRunTask != null)
            {
                if (_processState == Process.ProcessType.Unknown)
                {
                    //start new run?
                    if (!systemRunTask.LastRun.HasValue || systemRunTask.LastRun.Value.AddDays(systemRunTask.FrequencyDays) < DateTime.Now)
                    {
                        try
                        {
                            //do harvest
                            _processState = Process.ProcessType.Harvest;

                            NZOR.Admin.Data.Process.ProcessResult res = null;

                            //for initial DB, need to clear all data first
                            string harvestMode = System.Configuration.ConfigurationManager.AppSettings["HarvestingMode"];

                            if (harvestMode == "None")
                            {
                                Log.LogEvent("Harvest mode=None.  Skipping harvest.");
                                res = new ProcessResult(); //empty result     
                                //always redo integration in no-harvest mode
                                res.UpdateRequired = true;
                                _processState = Process.ProcessType.Integrate;
                            }
                            else
                            {
                                Log.LogEvent("Starting harvest");
                                if (harvestMode == "Full")
                                {
                                    res = Process.RunInitialHarvest(null);
                                }
                                else 
                                {
                                    res = Process.RunHarvest(true);
                                }

                                if (res.Result == Admin.Data.Process.ProcessResultOutcome.Failed)
                                {
                                    Log.LogEvent("Harvesting Failed");

                                    foreach (string sl in res.ErrorMessages)
                                    {
                                        Log.LogEvent(sl);
                                    }
                                    foreach (String sl in res.Log)
                                    {
                                        Log.LogEvent(sl);
                                    }
                                }
                                else
                                {
                                    if (res.ErrorMessages.Count > 0)
                                    {
                                        Log.LogEvent("Errors encountered during harvest.");
                                        foreach (String sl in res.ErrorMessages)
                                        {
                                            Log.LogEvent(sl);
                                        }
                                    }
                                    if (res.Log.Count > 0)
                                    {
                                        foreach (String sl in res.Log)
                                        {
                                            Log.LogEvent(sl);
                                        }
                                    }
                                    Log.LogEvent("Harvesting complete");

                                }

                                _processState = Process.ProcessType.Integrate;
                            }

                            if (_processState == Process.ProcessType.Integrate)
                            {
                                //integrate?
                                NZOR.Admin.Data.Sql.Repositories.AdminRepository ar = new Admin.Data.Sql.Repositories.AdminRepository(cnnStr);
                                List<NZOR.Admin.Data.Entities.BrokeredName> bnList = ar.GetNewBrokeredNames();
                                if (res.UpdateRequired || bnList.Count > 0)
                                {
                                    Log.LogEvent("Starting integration");
                                    //do integration
                                    string intConfig = System.Configuration.ConfigurationManager.AppSettings["Integration Config File"];
                                    string intDataFile = System.Configuration.ConfigurationManager.AppSettings["Integration Data File"];
                                    bool updateSN = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["UpdateProviderStackedNameData"]);

                                    res = Process.RunIntegration(intConfig, intDataFile, null, updateSN, (harvestMode == "Full"));

                                    if (res.Result == Admin.Data.Process.ProcessResultOutcome.Failed)
                                    {
                                        _processState = Process.ProcessType.Unknown; //start agin next time

                                        foreach (string sl in res.ErrorMessages)
                                        {
                                            Log.LogEvent(sl);
                                        }

                                        ProcessStatusChanged("Completed System Run", "Failed");
                                    }
                                    else
                                    {
                                        Log.LogEvent("Integration complete");

                                        _processState = Process.ProcessType.WebTransfer;
                                    }
                                }
                                else
                                {
                                    //try again next time
                                    _processState = Process.ProcessType.Unknown;

                                    ProcessStatusChanged("Completed System Run", "Succeeded");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.LogEvent("ERROR : " + ex.Message + " : " + ex.StackTrace);
                            _processState = Process.ProcessType.Unknown;

                            ProcessStatusChanged("Completed System Run", "Failed");

                            Complete = true;
                        }
                    }
                }
                else
                {
                    // a run is in progress
                    if (_processState == Process.ProcessType.WebTransfer)
                    {
                        //do the transfer

                        try
                        {
                            Log.LogEvent("Starting web transfer");

                            ProcessResult res = Process.RunWebTransfer();

                            if (res.Result == ProcessResultOutcome.Failed)
                            {
                                _processState = Process.ProcessType.Unknown; //start agin next time

                                foreach (string sl in res.ErrorMessages)
                                {
                                    Log.LogEvent(sl);
                                }

                                ProcessStatusChanged("Completed System Run", "Failed");
                            }
                            else
                            {
                                Log.LogEvent("Web transfer complete");

                                _processState = Process.ProcessType.Unknown; //start again next time

                                Complete = true;

                                ProcessStatusChanged("Completed System Run", "Succeeded");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.LogEvent("ERROR : " + ex.Message + " : " + ex.StackTrace);
                            _processState = Process.ProcessType.Unknown;

                            ProcessStatusChanged("Completed System Run", "Failed");

                            Complete = true;
                        }
                        

                        Log.LogEvent("NZOR Service run complete");
                    }
                }
            }

            _t.Interval = new TimeSpan(0, 10, 0).TotalMilliseconds;
            _t.Start();
        }

        public static void Stop()
        {
            _t.Stop();
            _t.Dispose();

            if (_batchProcessor != null) _batchProcessor.Stop();
            if (_nameRequestProcessor != null) _nameRequestProcessor.Stop();
        }
    }
}
