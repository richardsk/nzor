using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using System.Data.Objects;

namespace NZORConsumer.Harvest
{
    public class HarvestProcessor
    {
        public enum HarvestStatus
        {
            NotRunning,
            Running
        }

        public event ProcessingMessage Message;

        public HarvestStatus Status = HarvestStatus.NotRunning;
        public int Progress = 0;
        public int TotalToHarvest = 0;
        public int TotalDone = 0;
        public string StatusMessage = "Not running";

        private System.Timers.Timer _timer = new System.Timers.Timer();
        private System.Threading.Thread _processThread = null;
        //private bool _resumeLastHarvest = false;

        public void Start() //bool resumeLastHarvest)
        {
            if (Status == HarvestStatus.NotRunning)
            {
                //_resumeLastHarvest = resumeLastHarvest;
                Status = HarvestStatus.Running;

                _processThread = new System.Threading.Thread(new System.Threading.ThreadStart(RunHarvesting));
                _processThread.Start();
            }
        }

        public void Stop()
        {
            if (Status == HarvestStatus.Running)
            {
                if (_processThread != null)
                {
                    _processThread.Abort();
                    _processThread = null;
                }
                Status = HarvestStatus.NotRunning;
            }
        }

        private void RunHarvesting()
        {
            _timer.Interval = 1;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            _timer.AutoReset = false;
            _timer.Start();
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Status == HarvestStatus.NotRunning) return;

            try
            {
                Model.ConsumerEntities ce = new Model.ConsumerEntities();
                Model.Harvest h = ce.Harvests.FirstOrDefault();
                
                if (h != null)
                {
                    DateTime lastHarvest = h.LastHarvest.HasValue ? h.LastHarvest.Value : DateTime.MinValue;
                    int interval = h.IntervalDays.HasValue ? h.IntervalDays.Value : 2;
                    
                    if (DateTime.Now.AddDays(-interval) > lastHarvest)
                    {
                        //do harvesting

                        Progress = 0;
                        TotalToHarvest = 0;
                        TotalDone = 0;
                        int pageNum = 1;
                        //int totalPages = 0;
                        StatusMessage = "Harvesting...";

                        //if (_resumeLastHarvest && h.LastHarvestStopPage != null)
                        //{
                        //    totalPages = h.LastHarvestStopPage.Value;
                        //    if (h.LastHarvestNameCount != null) TotalDone = h.LastHarvestNameCount.Value;
                        //}

                        NZORServiceMessage message = null;
                        
                        Model.HarvestScope scope = ce.HarvestScopes.FirstOrDefault();
                        
                        Model.BiostatusOption bo = ce.BiostatusOptions.FirstOrDefault();
            
                        int doneInStage = 0;

                        if (ce.HarvestScopeNames.Count() > 0)
                        {
                            List<Model.HarvestScopeName> hsNames = ce.HarvestScopeNames.ToList();
                            foreach (Model.HarvestScopeName hsn in hsNames)
                            {
                                pageNum = 1;
                                doneInStage = 0;

                                if (bo != null && ((bo.Present.HasValue && bo.Present.Value) || (bo.Uncertain.HasValue && bo.Uncertain.Value)))
                                {
                                    if (bo.Present.HasValue && bo.Present.Value)
                                    {
                                        Model.NameResultList names = ConsumerClient.GetUpdatedNames(h.ServiceUrl, lastHarvest, scope.AcceptedNamesOnly, "Present", hsn.NZORNameId, pageNum, ref TotalToHarvest, out message);

                                        while (names.Count > 0 && Status == HarvestStatus.Running)
                                        {
                                            doneInStage += names.Count;

                                            SaveNames(names, message);

                                            pageNum++;

                                            h.LastHarvestStopPage = pageNum;
                                            h.LastHarvestNameCount = TotalDone;
                                            ce.SaveChanges();

                                            StatusMessage = "Harvesting " + doneInStage.ToString() + " of " + TotalToHarvest.ToString() + " Names of descendent " + hsn.FullName + ", with biostatus Present";
                                            Message(message, false);

                                            TotalDone += names.Count;
                                            Progress = doneInStage * 100 / TotalToHarvest;

                                            names = ConsumerClient.GetUpdatedNames(h.ServiceUrl, lastHarvest, scope.AcceptedNamesOnly, "Present", hsn.NZORNameId, pageNum, ref TotalToHarvest, out message);
                                        }
                                    }
                                    if (bo.Uncertain.HasValue && bo.Uncertain.Value)
                                    {
                                        pageNum = 1;
                                        doneInStage = 0;
                                        Model.NameResultList names = ConsumerClient.GetUpdatedNames(h.ServiceUrl, lastHarvest, scope.AcceptedNamesOnly, "Uncertain", hsn.NZORNameId, pageNum, ref TotalToHarvest, out message);

                                        while (names.Count > 0 && Status == HarvestStatus.Running)
                                        {
                                            doneInStage += names.Count;

                                            SaveNames(names, message);

                                            pageNum++;

                                            h.LastHarvestStopPage = pageNum;
                                            h.LastHarvestNameCount = TotalDone;
                                            ce.SaveChanges();

                                            StatusMessage = "Harvesting " + doneInStage.ToString() + " of " + TotalToHarvest.ToString() + " Names of descendent " + hsn.FullName + ", with biostatus Uncertain";
                                            Message(message, false);

                                            TotalDone += names.Count;
                                            Progress = doneInStage * 100 / TotalToHarvest;

                                            names = ConsumerClient.GetUpdatedNames(h.ServiceUrl, lastHarvest, scope.AcceptedNamesOnly, "Uncertain", hsn.NZORNameId, pageNum, ref TotalToHarvest, out message);
                                        }
                                    }
                                }
                                else
                                {
                                    pageNum = 1;
                                    doneInStage = 0;
                                    Model.NameResultList names = ConsumerClient.GetUpdatedNames(h.ServiceUrl, lastHarvest, scope.AcceptedNamesOnly, null, hsn.NZORNameId, pageNum, ref TotalToHarvest, out message);

                                    while (names.Count > 0 && Status == HarvestStatus.Running)
                                    {
                                        doneInStage += names.Count;

                                        SaveNames(names, message);

                                        pageNum++;

                                        h.LastHarvestStopPage = pageNum;
                                        h.LastHarvestNameCount = TotalDone;
                                        ce.SaveChanges();

                                        StatusMessage = "Harvesting " + doneInStage.ToString() + " of " + TotalToHarvest.ToString() + " Names of descendent " + hsn.FullName;
                                        Message(message, false);

                                        TotalDone += names.Count;
                                        Progress = doneInStage * 100 / TotalToHarvest;

                                        names = ConsumerClient.GetUpdatedNames(h.ServiceUrl, lastHarvest, scope.AcceptedNamesOnly, null, hsn.NZORNameId, pageNum, ref TotalToHarvest, out message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            pageNum = 1;
                            doneInStage = 0;

                            Model.NameResultList names = ConsumerClient.GetUpdatedNames(h.ServiceUrl, lastHarvest, scope.AcceptedNamesOnly, null, null, pageNum, ref TotalToHarvest, out message);

                            while (names.Count > 0 && Status == HarvestStatus.Running)
                            {
                                doneInStage += names.Count;

                                SaveNames(names, message);

                                pageNum++;

                                h.LastHarvestStopPage = pageNum;
                                h.LastHarvestNameCount = TotalDone;
                                ce.SaveChanges();

                                StatusMessage = "Harvesting " + doneInStage.ToString() + " of " + TotalToHarvest.ToString() + " Names";
                                Message(message, false);

                                TotalDone += names.Count;
                                Progress = doneInStage * 100 / TotalToHarvest;

                                names = ConsumerClient.GetUpdatedNames(h.ServiceUrl, lastHarvest, scope.AcceptedNamesOnly, null, null, pageNum, ref TotalToHarvest, out message);
                            }
                        }

                        if (Status == HarvestStatus.Running)
                        {
                            Progress = 100;

                            h.LastHarvest = DateTime.Now;
                            h.LastHarvestStopPage = null;
                            h.LastHarvestNameCount = null;
                            ce.SaveChanges();

                            StatusMessage = "Completed";
                            Message(null, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Progress = 100;
                Status = HarvestStatus.NotRunning;
                NZORServiceMessage sm = new NZORServiceMessage();
                sm.Error = "Error harvesting names: " + ex.Message;
                if (ex.InnerException != null) sm.Error += " : " + ex.InnerException.Message;
                Message(sm, false);
                Log.LogError(ex);
            }

            if (Status == HarvestStatus.Running)
            {
                _timer.Interval = 5000;
                _timer.Start();
            }
        }

        private void SaveNames(Model.NameResultList names, NZORServiceMessage message)
        {
            Model.ConsumerEntities ce = new Model.ConsumerEntities();

            if (Message != null) Message(message, false);

            foreach (Model.NameResult hn in names)
            {                
                Model.Name existingName = ce.Names.Where(en => en.NZORId == hn.Name.NZORId).FirstOrDefault();
                if (existingName != null)
                {
                    existingName.AcceptedNameId = hn.Name.AcceptedNameId;
                    existingName.Authors = hn.Name.Authors;
                    existingName.FullName = hn.Name.FullName;
                    existingName.GoverningCode = hn.Name.GoverningCode;
                    existingName.AcceptedNameId = hn.Name.AcceptedNameId;
                    existingName.ParentNameId = hn.Name.ParentNameId;
                    existingName.TaxonRank = hn.Name.TaxonRank;
                    existingName.Year = hn.Name.Year;
                }
                else
                {
                    ce.Names.AddObject(hn.Name);
                }

                foreach (Model.Provider p in hn.Providers)
                {
                    Model.Provider existingProvider = ce.Providers.Where(ep => ep.ProviderId == p.ProviderId).FirstOrDefault();
                    if (existingProvider == null)
                    {
                        ce.Providers.AddObject(p);
                    }
                    Model.NameProvider np = ce.NameProviders.Where(np1 => np1.NZORId == hn.Name.NZORId && np1.ProviderId == p.ProviderId).FirstOrDefault();
                    if (np == null)
                    {
                        np = new Model.NameProvider();
                        np.ProviderId = p.ProviderId;
                        np.NZORId = hn.Name.NZORId;
                        ce.NameProviders.AddObject(np);
                    }
                }

                ce.SaveChanges();

            }

        }
                
    }
}
