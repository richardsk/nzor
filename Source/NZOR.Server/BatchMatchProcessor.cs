using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Configuration;
using System.Globalization;
using NZOR.Matching.Batch.Helpers;
using NZOR.Admin.Data.Entities.Matching;
using NZOR.Matching.Batch;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Data.LookUps.Common;
using NZOR.Admin.Data.Sql.Repositories.Matching;
using NZOR.Admin.Data.Entities;
using NZOR.Admin.Data.Sql.Repositories;
using NZOR.ExternalLookups;

namespace NZOR.Server
{
    public class BatchMatchProcessor
    {
        private System.Timers.Timer _timer = new System.Timers.Timer();

        private System.Threading.Semaphore _sem = new System.Threading.Semaphore(1, 1);

        private TaxonRankLookUp _taxonRankLookup = null;
        private AdminRepository _adminRepository = null;
        private MatchRepository _matchRepository = null;
        private ProviderRepository _provRepository = null;
        private List<NZOR.Admin.Data.Entities.ExternalLookupService> _externalLookupServices = null;

        public void Run()
        {            
            Log.LogEvent("Batch Match Processor started");
            
            string cnnStr = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            LookUpRepository lr = new NZOR.Data.Sql.Repositories.Common.LookUpRepository(cnnStr);
            _taxonRankLookup = new TaxonRankLookUp(lr.GetTaxonRanks());

            var externalLookupRepository = new Admin.Data.Sql.Repositories.ExternalLookupRepository(cnnStr);
            _externalLookupServices = externalLookupRepository
               .ListLookupServices()
               .Where(o => o.DataFormat.Equals("HTML", StringComparison.OrdinalIgnoreCase))
               .ToList();

            _adminRepository = new AdminRepository(cnnStr);
            _matchRepository = new MatchRepository(cnnStr);
            _provRepository = new ProviderRepository(cnnStr);

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

            //check for new files to process
            try
            {
                List<Match> pendingMatches = _matchRepository.GetPendingMatches();

                if (pendingMatches.Any())
                {
                    Log.LogEvent("Processing " + pendingMatches.Count().ToString() + " batch(es)");

                    foreach (var match in pendingMatches)
                    {
                        try
                        {
                            //_adminRepository.NameRequests.Clear(); 

                            ProcessBatchMatch(match);

                            match.State = Entity.EntityState.Modified;
                            _matchRepository.Save(match);

                            _adminRepository.Save();
                        }
                        catch (Exception ex)
                        {
                            Log.LogEvent("ERROR : Processing batch matches : " + ex.Message + " : " + ex.StackTrace);

                            match.State = Entity.EntityState.Modified;
                            match.Status = Match.Statuses.Error;
                            match.Error = "Error processing batch match: " + ex.Message;
                            _matchRepository.Save(match);
                        }
                    }

                    Log.LogEvent("Finished batch matches.");
                }
            }
            catch (Exception ex)
            {
                Log.LogEvent("ERROR : Processing batch matches : " + ex.Message + " : " + ex.StackTrace);
            }

            _timer.Interval = 5000;
            _timer.Start();

            _sem.Release();
        }

        public void ProcessBatchMatch(Match bm)
        {
            Log.LogEvent("Processing batch match, Id=" + bm.MatchId.ToString());

            string cnnStr = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            string indexPath = ConfigurationManager.AppSettings["LuceneIndexFilePath"];
            NZOR.Matching.Batch.Matchers.NameMatcher nm = new Matching.Batch.Matchers.NameMatcher(indexPath, cnnStr);
            List<Matching.Batch.NameMatchResult> sr = nm.Match(bm.InputData);


            var externalLookupRepository = new Admin.Data.Sql.Repositories.ExternalLookupRepository(cnnStr);
            List<ExternalLookupService> extlookups = externalLookupRepository.ListLookupServices().ToList();
            ExternalLookupManager extMgr = new ExternalLookupManager(extlookups, _provRepository, _taxonRankLookup);

            string data = "SubmittedId,SubmittedName,MatchedNZORName,NZORId,Score,PartialName,Authors,Year,PreferredName,PreferredNameId,Classification,ClassificationRanks,ClassificationIds,ParentAccordingTo,PreferredAccordingTo,TaxonomicStatus,NomenclaturalStatus,Biostatus,VernacularNamesForScientific,VernacularNamesForScientificIds,ScientificNamesForVernaculars,ScientificNamesForVernacularsIds" + Environment.NewLine;

            string extMatches = "SubmittedId,SubmittedName,ExternalSource,FullName,DataUrl" + Environment.NewLine;

            foreach (Matching.Batch.NameMatchResult nmr in sr)
            {
                if (nmr.Message != string.Empty)
                {
                    Log.LogEvent(nmr.Message);
                }

                if (nmr.NameMatches.Count == 0)
                {
                    data += "\"" + nmr.SubmittedId.Replace("\"", "\"\"") + "\",";
                    data += "\"" + nmr.SubmittedScientificName.Replace("\"", "\"\"") + "\",";
                    data += "No Match,,0,,,,,,,,,,,,,,,,";

                    //data += "\"";
                    //foreach (Matching.Batch.ExternalLookup el in GetExternalLookups(nmr.SubmittedScientificName))
                    //{
                    //    string link = el.SearchUrl.Replace(" ", "+");
                    //    data += link.Replace("\"", "\"\"") + ",";
                    //}
                    //data += "\"";

                    //external lookups
                    //if (bm.BrokerMissingNames.HasValue && bm.BrokerMissingNames.Value)
                    if (bm.DoExternalLookup.HasValue && bm.DoExternalLookup.Value)
                    {
                        List<ExternalNameResult> names = extMgr.GetMatchingNames(nmr.SubmittedScientificName, true);

                        foreach (ExternalNameResult enr in names)
                        {
                            extMatches += "\"" + nmr.SubmittedId.Replace("\"", "\"\"") + "\",";
                            extMatches += "\"" + nmr.SubmittedScientificName.Replace("\"", "\"\"") + "\",";
                            extMatches += "\"" + enr.LookupService.Name + "\",";
                            extMatches += "\"" + enr.Name.FullName.Replace("\"", "\"\"") + "\",";
                            extMatches += "\"" + enr.DataUrl + "\"";
                            extMatches += Environment.NewLine;
                        }
                    }

                    data += Environment.NewLine;
                }
                else
                {
                    foreach (Matching.Batch.NameMatch match in nmr.NameMatches)
                    {
                        data += "\"" + nmr.SubmittedId.Replace("\"", "\"\"") + "\",";
                        data += "\"" + nmr.SubmittedScientificName.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.NzorFullName.Replace("\"", "\"\"") + "\",";
                        if (match.NzorId.Equals(Guid.Empty))
                        {
                            data += "\"" + "\",";
                        }
                        else
                        {
                            data += "\"" + match.NzorId.ToString() + "\",";
                        }
                        data += "\"" + match.Score + "\",";
                        data += "\"" + match.PartialName.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.Authors.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.Year.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.PreferredName.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.PreferredNameId + "\",";
                        data += "\"" + match.Classification.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.ClassificationRanks.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.ClassificationIds + "\",";
                        data += "\"" + match.ParentAccordingTo.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.PreferredAccordingTo.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.TaxonomicStatus.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.NomenclaturalStatus.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.Biostatus.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.VernacularNamesForScientific.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.VernacularNamesForScientificIds + "\",";
                        data += "\"" + match.ScientificNamesForVernacular.Replace("\"", "\"\"") + "\",";
                        data += "\"" + match.ScientificNamesForVernacularIds + "\",";

                        //data += "\"";
                        //foreach (Matching.Batch.ExternalLookup el in match.ExternalLookups)
                        //{
                        //    string link = el.SearchUrl.Replace(" ", "+");
                        //    data += link.Replace("\"", "\"\"") + ",";
                        //}
                        //data += "\"";

                        data += Environment.NewLine;
                    }
                }
            }

            bm.ResultData = data;
            if (bm.DoExternalLookup.HasValue && bm.DoExternalLookup.Value) bm.ExternalLookupResults = extMatches;

            if (bm.IsServiceMediated.HasValue && bm.IsServiceMediated.Value)
            {
                bm.Status = Match.Statuses.Completed;
            }
            else if (SendResultEmail(bm))
            {
                bm.Status = Match.Statuses.Sent;
            }

            //if (nm.ErrorMsg != "")
            //{
            //    //log
            //    NZOR.Utility.Log.LogEvent("ERROR : Processing batch matches : Id=" + md.BatchMatch.MatchId.ToString() + " : " + md.ErrorMsg);
            //}
            //else
            //{
            //    SendResultEmail(md);

            //    if (md.ErrorMsg != "")
            //    {
            //        //log
            //        NZOR.Utility.Log.LogEvent("ERROR : Processing batch matches : Id=" + md.BatchMatch.MatchId.ToString() + " : " + md.ErrorMsg);
            //    }
            //}
        }
        
        //private void AddNameRequest(NameMatchResult nmr, Match batchMatch)
        //{
            //NameRequest enr = _adminRepository.GetNameRequestByFullName(nmr.SubmittedScientificName);
            //if (enr == null)
            //{
            //    NameRequest nr = new NameRequest();
            //    nr.ApiKey = batchMatch.ApiKey;
            //    nr.AddedBy = "System";
            //    nr.AddedDate = DateTime.Now;
            //    nr.FullName = nmr.SubmittedScientificName;
            //    nr.NameRequestId = Guid.NewGuid();
            //    nr.RequestDate = DateTime.Now;
            //    nr.RequesterEmail = batchMatch.SubmitterEmail;
            //    nr.BatchMatchId = batchMatch.MatchId;
            //    nr.Status = NameRequest.Statuses.Pending;

            //    nr.State = Entity.EntityState.Added;

            //    _adminRepository.NameRequests.Add(nr);
            //}
        //}


        private List<ExternalLookup> GetExternalLookups(string name)
        {
            List<ExternalLookup> externalLookups = new List<ExternalLookup>();

            if (_externalLookupServices != null)
            {
                NZOR.Data.Entities.Common.NameParseResult nameParseResult = NZOR.Matching.NameParser.ParseName(name, _taxonRankLookup);

                string nameWithoutAuthors = nameParseResult.GetFullName(_taxonRankLookup, false, null);

                foreach (var externalLookupService in _externalLookupServices)
                {
                    var externalLookup = new ExternalLookup();
                    string lookupName = nameWithoutAuthors.Replace(" ", externalLookupService.SpaceCharacterSubstitute);

                    externalLookup.OrganisationName = externalLookupService.Name;
                    externalLookup.Type = externalLookupService.DataFormat;
                    externalLookup.SearchUrl = externalLookupService.NameLookupEndpoint + lookupName;

                    externalLookups.Add(externalLookup);
                }
            }

            return externalLookups;
        }

        private bool SendResultEmail(Match md)
        {
            string extResultsTxt = "";
            if (md.ExternalLookupResults != null && md.ExternalLookupResults != string.Empty)
            {
                extResultsTxt = @"<tr><td>
                                Any results from external lookup services can be downloaded from "
                                + System.Configuration.ConfigurationManager.AppSettings["ExternalMatchResultsDownloadUrl"] + md.MatchId.ToString()
                                + @"<br/><br/></td></tr>";
            }

            //send email...
            string body =
                @"<table>
                        <tr>
                            <td>Hello,</td>
                        </tr>
                        <tr>
                            <td><br/>Below is a link to a CSV file containing the results of the batch match you recently submitted.<br/><br/></td>
                        </tr>                    
                        <tr>
                            <td>"
                                + System.Configuration.ConfigurationManager.AppSettings["BatchMatchDownloadUrl"] + md.MatchId.ToString()
                                + @"<br/><br/>
                            </td>
                        </tr>"
                                + extResultsTxt 
                        + @"<tr>
                            <td>Regards,</td>
                        </tr>
                        <tr>
                            <td>The NZOR website administrator.</td>
                        </tr>
                  </table>";
            try
            {
                string from = System.Configuration.ConfigurationManager.AppSettings["SenderAddress"];

                MailMessage message = new MailMessage(from, md.SubmitterEmail, "NZOR Batch Match Results", body);
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                message.Priority = MailPriority.High;
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();

                client.Send(message);

                message.Dispose();
                client.Dispose();

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
