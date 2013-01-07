using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using NZOR.Matching.Batch;
using NZOR.Admin.Data.Entities;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Admin.Data.Sql.Repositories;
using NZOR.Admin.Data.Entities.Matching;
using NZOR.ExternalLookups;
using NZOR.Data.Entities.Provider;
using NZOR.Data.Sql.Repositories.Provider;
using NZOR.Data;
using NZOR.Integration;

namespace NZOR.Server
{
    public class NameRequestProcessor
    {
        private TaxonRankLookUp _taxonRankLookup = null;
        private List<ExternalLookupService> _externalLookupServices = null;
        private NameRepository _providerNameRepository = null;
        private ConceptRepository _providerConceptRepository = null;
        private ProviderRepository _providerRepository = null;
        private AdminRepository _adminRepository = null;
        
        private System.Timers.Timer _timer = new System.Timers.Timer();

        private System.Threading.Semaphore _sem = new System.Threading.Semaphore(1, 1);

        public static string StateIdle = "Idle";
        public static string StateProcessing = "Processing";

        public string State = StateIdle;

        public void Run()
        {
            Log.LogEvent("Name Request Processor started");

            string cnnStr = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            LookUpRepository lr = new NZOR.Data.Sql.Repositories.Common.LookUpRepository(cnnStr);
            _taxonRankLookup = new TaxonRankLookUp(lr.GetTaxonRanks());

            var externalLookupRepository = new Admin.Data.Sql.Repositories.ExternalLookupRepository(cnnStr);
            _externalLookupServices = externalLookupRepository
               .ListLookupServices()
               .Where(o => !o.DataFormat.Equals("HTML", StringComparison.OrdinalIgnoreCase))
               .ToList();

            _providerNameRepository = new NameRepository(cnnStr);
            _providerConceptRepository = new ConceptRepository(cnnStr);
            _providerRepository = new ProviderRepository(cnnStr);
            _adminRepository = new AdminRepository(cnnStr);
            
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            _timer.AutoReset = false;
            _timer.Interval = 1;
            _timer.Start();
        }

        public void Stop()
        {
            _sem.WaitOne();

            _timer.Enabled = false;
            _timer.Close();
            _timer.Dispose();

            _sem.Release();
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _sem.WaitOne();

            State = StateProcessing;

            //check for new names to process
            try
            {
                string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

                var adminRepository = new AdminRepository(cnnStr);

                List<NameRequest> pendingRequests = adminRepository.GetPendingNameRequests();
                
                if (pendingRequests.Any())
                {
                    Log.LogEvent("Processing " + pendingRequests.Count().ToString() + " name request(s)");

                    //record time of process start as we need to update prov records that are added/modified
                    DateTime beforeProcess = DateTime.Now;

                    foreach (var request in pendingRequests)
                    {
                        ProcessNameRequest(request);

                        request.State = Entity.EntityState.Modified;
                        adminRepository.NameRequests.Add(request);
                    }

                    adminRepository.Save();

                    Log.LogEvent("Finished name requests.");

                    Log.LogEvent("Processing updated names.");
                           
                    string intConfig = System.Configuration.ConfigurationManager.AppSettings["Integration Config File"];
                    System.Xml.XmlDocument config = new System.Xml.XmlDocument();
                    config.Load(intConfig);

                    UpdateProcessor up = new UpdateProcessor();
                    up.ProcessUpdatedProviderData(beforeProcess, config, null);

                    Log.LogEvent("Name requests complete.");
                }
            }

            catch (Exception ex)
            {
                Log.LogEvent("ERROR : Processing name requests: " + ex.Message + " : " + ex.StackTrace);
            }

            State = StateIdle;

            _timer.Interval = 5000;
            _timer.Start();

            _sem.Release();
        }

        
        private void ProcessNameRequest(NameRequest request)
        {            
            //lookup external services to locate names             
            ExternalLookupManager extMgr = new ExternalLookupManager(_externalLookupServices, _providerRepository, _taxonRankLookup);

            List<ExternalNameResult> names = new List<ExternalNameResult>();
            
            //2 ways to lookup - by full name search, or by submitted id and external lookup service 
            if (request.ExternalLooksupServiceId.HasValue)
            {
                names.AddRange(extMgr.GetByExternalDataUrl(request.ExternalLookupDataUrl, false));
            }
            else
            {
                names.AddRange(extMgr.GetMatchingNames(request.FullName, false));
            }

            //add provider name
            _providerNameRepository.Names.Clear();
            _providerConceptRepository.Concepts.Clear();

            foreach (ExternalNameResult enr in names)
            {
                Name extName = enr.Name;
                extName.AddedDate = DateTime.Now;
                extName.State = Data.Entities.Entity.EntityState.Added;
                extName.LinkStatus = LinkStatus.Unmatched.ToString();

                Name pn = _providerNameRepository.GetNameByProviderId(enr.DataSourceCode, enr.Name.ProviderRecordId); //update prov name?
                if (pn != null)
                {
                    Guid oldId = extName.NameId;
                    extName.NameId = pn.NameId;
                    extName.AddedDate = pn.AddedDate;
                    extName.State = Data.Entities.Entity.EntityState.Modified;
                    extName.LinkStatus = pn.LinkStatus;
                    extName.MatchPath = pn.MatchPath;
                    extName.MatchScore = pn.MatchScore;
                    extName.ConsensusNameId = pn.ConsensusNameId;

                    foreach (ExternalNameResult cenr in names)
                    {
                        foreach (Concept pc in cenr.Concepts)
                        {
                            if (pc.NameId == oldId) pc.NameId = extName.NameId;
                        }
                    }
                }                
                
                _providerNameRepository.Names.Add(extName);

                foreach (Concept pc in enr.Concepts)
                {
                    pc.AddedDate = DateTime.Now;
                    pc.State = Data.Entities.Entity.EntityState.Added;
                    pc.LinkStatus = LinkStatus.Unmatched.ToString();
                    _providerConceptRepository.Concepts.Add(pc);
                }
            }
            _providerNameRepository.Save();
            _providerConceptRepository.Save();

            //add brokered name(s)
            _adminRepository.BrokeredNames.Clear();
            foreach (ExternalNameResult enr in names)
            {
                BrokeredName bn = new BrokeredName();
                bn.AddedBy = "System";
                bn.AddedDate = DateTime.Now;
                bn.BrokeredNameId = Guid.NewGuid();
                bn.ExternalLookupServiceId = enr.LookupService.ExternalLookupServiceId;
                bn.NameRequestId = request.NameRequestId;
                bn.NZORProviderNameId = enr.Name.NameId;
                bn.ProviderRecordId = enr.Name.ProviderRecordId;
                bn.DataUrl = enr.DataUrl;
                bn.WebUrl = enr.WebUrl;

                bn.State = Entity.EntityState.Added;

                _adminRepository.BrokeredNames.Add(bn);
            }
            _adminRepository.Save();

            request.Status = NameRequest.Statuses.Processed;
        }

    }
}
