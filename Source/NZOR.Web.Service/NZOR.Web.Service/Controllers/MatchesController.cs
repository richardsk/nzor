using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using NZOR.Publish.Model.Matching;
using NZOR.Admin.Data.Entities.Matching;
using NZOR.Admin.Data.Sql.Repositories.Matching;
using NZOR.Matching.Batch.Matchers;
using NZOR.Publish.Data.Repositories;
using NZOR.Admin.Data.Sql.Repositories;
using NZOR.Admin.Data.Entities;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using NZOR.Web.Service.Helpers;
using System.Net;
using System.Web.Http;

namespace NZOR.Web.Service.APIs.Controllers
{
    public class MatchesController : System.Web.Mvc.Controller
    {
        private string _connectionString;
        private string _baseFolder;

        private readonly MatchRepository _matchRepository;
        private readonly NameRepository _nameRepository;
        private readonly NameMatcher _nameMatcher;
        private readonly AdminRepository _adminRepository;

        public MatchesController()
        {
            _baseFolder = System.Web.HttpContext.Current.Server.MapPath(@"~/App_Data/");
            _connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            _nameMatcher = new NameMatcher(System.IO.Path.Combine(_baseFolder, "Indexes/Names"), _connectionString);
            _matchRepository = new MatchRepository(_connectionString);
            _nameRepository = new NameRepository(System.IO.Path.Combine(_baseFolder, "Indexes/Names"));
            _adminRepository = new AdminRepository(_connectionString);
        }

        [Description("Submit a small file in the appropriate format for name matching.")]
        [System.Web.Http.HttpPost]
        public ActionResult SubmitBatchMatch()
        {
            var response = new List<NameMatchResult>();
            StreamReader rdr = new StreamReader(Request.InputStream);
            string content = rdr.ReadToEnd();

            //var task = this.Request.InputStream.r.Content.ReadAsStringAsync()
            //    .ContinueWith(result =>
            //    {
            //        content = result.Result;
            //    });

            //task.Wait();

            if (!String.IsNullOrWhiteSpace(content))
            {
                List<NZOR.Matching.Batch.NameMatchResult> matcherResults;

                try
                {
                    matcherResults = _nameMatcher.Match(content, 10);
                }
                catch (Exception ex)
                {
                    var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(ex.Message),
                    };
                    throw new HttpResponseException(errorResponse);
                }

                foreach (var matcherResult in matcherResults)
                {
                    NameMatchResult returnNameMatchResult = new NameMatchResult();

                    returnNameMatchResult.SubmittedId = matcherResult.SubmittedId;
                    returnNameMatchResult.SubmittedScientificName = matcherResult.SubmittedScientificName;
                    returnNameMatchResult.Message = matcherResult.Message;

                    foreach (var matcherName in matcherResult.NameMatches)
                    {
                        NameMatch returnNameMatch = new NameMatch();

                        returnNameMatch.Name = _nameRepository.SingleOrDefault(new Guid(matcherName.NzorId));
                        returnNameMatch.Score = matcherName.Score;

                        foreach (var externalLookup in matcherName.ExternalLookups)
                        {
                            returnNameMatch.ExternalLookups.Add(new NZOR.Publish.Model.Matching.ExternalLookup
                            {
                                OrganisationName = externalLookup.OrganisationName,
                                SearchUrl = externalLookup.SearchUrl,
                                Type = externalLookup.Type
                            });
                        }

                        returnNameMatchResult.NameMatches.Add(returnNameMatch);
                    }

                    response.Add(returnNameMatchResult);
                }

                if (Request.Params["format"] == "json") return Json(response, JsonRequestBehavior.AllowGet);
                return new XmlResult(response);
            }
            else
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("A valid body content is required"),
                };
                throw new HttpResponseException(errorResponse);
            }
        }


        [Description("Submit a file in the appropriate format for name matching that will be processed and retrieved via an email link.")]
        [System.Web.Http.HttpPost]
        public void SubmitBatchMatchWithEmail([System.Web.Http.FromUri]string email, bool? doExternalLookup)
        {
            if (!String.IsNullOrWhiteSpace(email))
            {
                ProcessBatchMatch(email, doExternalLookup);
            }
            else
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("A valid email is required"),
                };
                throw new HttpResponseException(errorResponse);
            }
        }

        //[HttpPost]
        //public ActionResult SubmitAutoBatchMatch(string apiKey, bool? brokerMissingNames)
        //{
        //    User u = _adminRepository.GetUserByApiKey(apiKey);
        //    if (u == null)
        //    {
        //        throw new System.Web.Http.HttpResponseException("Invalid API key", System.Net.HttpStatusCode.BadRequest);
        //    }

        //    Match m = ProcessBatchMatch(null, apiKey, brokerMissingNames);

        //    AutoBatchMatchResponse abm = new AutoBatchMatchResponse();
        //    abm.BatchMatchId = m.MatchId.ToString();
        //    abm.Status = m.Status;

        //    if (Request.Params["format"] == "json") return Json(abm, JsonRequestBehavior.AllowGet);            
        //    return new XmlResult(abm);
        //}

        public ActionResult PollBatchMatch(string apiKey, string batchMatchId)
        {
            User u = null;
            if (apiKey != null) u = _adminRepository.GetUserByApiKey(apiKey);
            if (u == null)
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("A valid Api key is required"),
                };
                throw new HttpResponseException(errorResponse);
            }

            Guid bid = Guid.Empty;
            if (!Guid.TryParse(batchMatchId, out bid))
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("A valid batch match Id is required"),
                };
                throw new HttpResponseException(errorResponse);
            }

            Match m = _matchRepository.GetMatch(bid, false);
            if (m == null)
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("A valid batch match Id is required"),
                };
                throw new HttpResponseException(errorResponse);
            }

            AutoBatchMatchResponse abm = new AutoBatchMatchResponse();
            abm.BatchMatchId = m.MatchId.ToString();
            if (m.Status == Match.Statuses.Completed) abm.DownloadUrl = System.Configuration.ConfigurationManager.AppSettings["BatchMatchDownloadUrl"] + batchMatchId;
            abm.Status = m.Status;
            if (m.Status == Match.Statuses.Error) abm.Status += ": " + m.Error;

            if (Request.Params["format"] == "json") return Json(abm, JsonRequestBehavior.AllowGet);
            return new XmlResult(abm);
        }

        private Match ProcessBatchMatch(string email, bool? doExternalLookup)
        {
            //string content = String.Empty;

            //var task = this.Request.Content.ReadAsStringAsync()
            //    .ContinueWith(result =>
            //        {
            //            content = result.Result;
            //        });

            //task.Wait();


            StreamReader rdr = new StreamReader(Request.InputStream);
            string content = rdr.ReadToEnd();

            Match match = null;

            if (!String.IsNullOrWhiteSpace(content))
            {
                match = new Match();

                match.State = Admin.Data.Entities.Entity.EntityState.Added;
                match.SubmitterEmail = email ?? "";
                match.ReceivedDate = DateTime.Now;
                match.InputData = content;
                match.DoExternalLookup = doExternalLookup;

                _matchRepository.Save(match);
            }
            else
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("A valid body content is required"),
                };
                throw new HttpResponseException(errorResponse);
            }

            return match;
        }

        [Description("Returns a specific match result by Id.")]
        public FileStreamResult GetBatchMatch(string id)
        {
            Guid parsedId;

            if (Guid.TryParse(id, out parsedId))
            {
                var match = _matchRepository.GetMatch(parsedId, true);

                if (match == null)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(String.Format("The provider record id '{0}' cannot be found.", id)),
                        ReasonPhrase = "Provider Record Id Not Found"
                    };
                    throw new HttpResponseException(response);
                }

                string resultData = match.ResultData;

                var result = new MemoryStream(Encoding.Unicode.GetBytes(resultData));

                FileStreamResult fsr = new FileStreamResult(result, "application/vnd.ms-excel");
                fsr.FileDownloadName = "Match Result.csv";

                return fsr;
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(String.Format("The id '{0}' is not a valid identifier.", id)),
                    ReasonPhrase = "Id Invalid"
                };
                throw new HttpResponseException(response);
            }
        }

        [Description("Returns external lookup results for a batch match, by batch Id.")]
        public FileStreamResult GetBatchExternalResults(string id)
        {
            Guid parsedId;

            if (Guid.TryParse(id, out parsedId))
            {
                var match = _matchRepository.GetMatch(parsedId, true);

                if (match == null)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(String.Format("The provider record id '{0}' cannot be found.", id)),
                        ReasonPhrase = "Provider Record Id Not Found"
                    };
                    throw new HttpResponseException(response);
                }

                string resultData = match.ExternalLookupResults;

                var result = new MemoryStream(Encoding.Unicode.GetBytes(resultData));

                FileStreamResult fsr = new FileStreamResult(result, "application/vnd.ms-excel");
                fsr.FileDownloadName = "External Match Results.csv";

                return fsr;
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(String.Format("The id '{0}' is not a valid identifier.", id)),
                    ReasonPhrase = "Id Invalid"
                };
                throw new HttpResponseException(response);
            }
        }

        [Description("Submit a batch of names to be brokered.  Must have valid external service ID/URL.")]
        [System.Web.Http.HttpPost]
        public void SubmitBrokeredNames(string email, string apiKey)
        {
            User u = null;
            if (apiKey != null) u = _adminRepository.GetUserByApiKey(apiKey);
            if (u == null)
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("A valid Api key is required"),
                };
                throw new HttpResponseException(errorResponse);
            }

            StreamReader rdr = new StreamReader(Request.InputStream);
            string content = rdr.ReadToEnd();

            if (!String.IsNullOrWhiteSpace(content))
            {
                try
                {
                    NZOR.Matching.Batch.BrokeredNameRequestProcessor bnr = new NZOR.Matching.Batch.BrokeredNameRequestProcessor(_connectionString);
                    bnr.ProcessBrokeredNameRequests(email, apiKey, content);
                }
                catch (Exception ex)
                {
                    var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(ex.Message),
                    };
                    throw new HttpResponseException(errorResponse);
                }
            }
            else
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("A valid body content is required"),
                };
                throw new HttpResponseException(errorResponse);
            }
        }

        [XmlRoot(ElementName = "NameMatchResults")]
        public class NameMatchResultResponse
        {
            public List<NameMatchResult> NameMatchResults { get; set; }

            public NameMatchResultResponse()
            {
                NameMatchResults = new List<NameMatchResult>();
            }
        }
    }
}