using NZOR.Publish.Model.Administration;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Model.Matching;
using NZOR.Publish.Model.Names;
using NZOR.Publish.Model.Search;
using NZOR.Publish.Model.Search.Names;
using NZOR.Publish.Model.Users;
using NZOR.Web.Service.Client.Responses;
using RestSharp;
using System;
using System.Collections.Generic;

namespace NZOR.Web.Service.Client
{
    /// <summary>
    /// Client for accessing the NZOR Web API.
    /// </summary>
    public class ServiceClient
    {
        private string _apiKey;
        private Uri _serviceUrl;

        public ServiceClient(string apiKey, string serviceUrl)
        {
            _apiKey = apiKey;

            if (!serviceUrl.EndsWith("/")) { serviceUrl += "/"; }

            _serviceUrl = new Uri(serviceUrl);
        }

        /// <summary>
        /// Returns a list of autocomplete matches for a name query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<string> LookupNames(string query, int take)
        {
            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("names/lookups?query={query}&take={take}");

            webRequest.AddHeader("Accept", "application/json");
            webRequest.AddParameter("query", query, ParameterType.UrlSegment);
            webRequest.AddParameter("take", take, ParameterType.UrlSegment);

            RestResponse<List<String>> webResponse = webClient.Execute<List<String>>(webRequest);

            return webResponse.Data;
        }

        public NamesSearchResponse Search(SearchRequest searchRequest)
        {
            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("names/search?query={query}&filter={filter}&page={page}&pagesize={pagesize}&orderby={orderBy}");

            webRequest.AddHeader("Accept", "application/json");
            webRequest.AddParameter("query", searchRequest.Query ?? String.Empty, ParameterType.UrlSegment);
            webRequest.AddParameter("filter", searchRequest.Filter ?? String.Empty, ParameterType.UrlSegment);
            webRequest.AddParameter("page", searchRequest.Page, ParameterType.UrlSegment);
            webRequest.AddParameter("pagesize", searchRequest.PageSize, ParameterType.UrlSegment);
            webRequest.AddParameter("orderBy", searchRequest.OrderBy ?? String.Empty, ParameterType.UrlSegment);

            RestResponse<NamesSearchResponse> webResponse = webClient.Execute<NamesSearchResponse>(webRequest);

            if (webResponse.Data == null)
            {
                if (webResponse.ErrorException != null)
                {
                    throw webResponse.ErrorException;
                }
                else
                {
                    throw new Exception(webResponse.Content);
                }
            }

            return webResponse.Data;
        }

        /// <summary>
        /// Gets a name with a specified Id.
        /// </summary>
        /// <param name="nameId">The Id of the name to return</param>
        /// <returns></returns>
        public Name GetName(string nameId)
        {
            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("names/{nameId}");

            webRequest.AddHeader("Accept", "application/json");
            webRequest.AddParameter("nameId", nameId, ParameterType.UrlSegment);

            RestResponse<Name> webResponse = webClient.Execute<Name>(webRequest);

            return webResponse.Data;
        }

        /// <summary>
        /// Gets a name with a specified provider record id.
        /// </summary>
        /// <param name="nameId">The id of the provider name for the NZOR name to return</param>
        /// <returns></returns>
        public Name GetNameByProviderId(string providerRecordId)
        {
            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("names/byproviderid?providerRecordId={providerRecordId}");

            webRequest.AddHeader("Accept", "application/json");
            webRequest.AddParameter("providerRecordId", providerRecordId, ParameterType.UrlSegment);

            RestResponse<Name> webResponse = webClient.Execute<Name>(webRequest);

            return webResponse.Data;
        }

        /// <summary>
        /// Submits a list of names for matching by the NZOR service.
        /// </summary>
        /// <param name="matchList">The names to match as csv</param>
        /// <param name="email">The notification email address</param>
        /// <remarks>
        /// Results are processed by the service and a notification is sent to the specified email address.
        /// with a download link.
        /// </remarks>
        public void SubmitMatchList(string matchList, string email, bool? doExternalLookup)
        {
            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("matches/submitbatchmatchwithemail?email={email}&doexternallookup={doexternallookup}");

            webRequest.Method = Method.POST;
            webRequest.AddParameter("email", email, ParameterType.UrlSegment);
            webRequest.AddParameter("doexternallookup", doExternalLookup, ParameterType.UrlSegment);
            webRequest.AddParameter(String.Empty, matchList, ParameterType.RequestBody);

            RestResponse webResponse = webClient.Execute(webRequest);
        }

        /// <summary>
        /// Submits a list of names for matching by the NZOR service.
        /// </summary>
        /// <param name="matchList">The names to match as csv</param>
        /// <remarks>
        /// Results are processed by the service and returned as a list of match results.
        /// </remarks>
        public List<NameMatchResult> SubmitMatchList(string matchList)
        {
            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("matches/submitbatchmatch");

            webRequest.Method = Method.POST;
            webRequest.AddParameter(String.Empty, matchList, ParameterType.RequestBody);

            RestResponse<List<NameMatchResult>> webResponse = webClient.Execute<List<NameMatchResult>>(webRequest);

            if (webResponse.ResponseStatus == ResponseStatus.Error)
            {
                throw new Exception(webResponse.Content);
            }

            return webResponse.Data;
        }
        
        ///// <summary>
        ///// Submit a list of names for matching from a service (ie no email mediation, caller will have to poll for response).
        ///// </summary>
        ///// <param name="matchList">The names to match as csv</param>
        ///// <param name="brokerMissingNames">True if any names not in NZOR should be maintained in the global cache</param>
        ///// <returns>Id of batch match to poll for completed results</returns>
        //public string SubmitMatchListAuto(string matchList, bool brokerMissingNames)
        //{
        //    var webClient = new RestClient(_serviceUrl.ToString());
        //    var webRequest = new RestRequest("matches/submitautobatchmatch?apikey={apiKey}&brokermissingnames={brokerMissingNames}");

        //    webRequest.Method = Method.POST;
        //    webRequest.AddParameter("apiKey", _apiKey, ParameterType.UrlSegment);
        //    webRequest.AddParameter("brokerMissingNames", brokerMissingNames, ParameterType.UrlSegment);
        //    webRequest.AddParameter(String.Empty, matchList, ParameterType.RequestBody);

        //    RestResponse<AutoBatchMatchResponse> webResponse = webClient.Execute<AutoBatchMatchResponse>(webRequest);

        //    return webResponse.Data.BatchMatchId;
        //}

        /// <summary>
        /// Poll for a submitted batch match, by id.
        /// </summary>
        /// <param name="batchMatchId">Id to look up</param>
        /// <returns></returns>
        public AutoBatchMatchResponse PollBatchMatch(string batchMatchId)
        {
            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("matches/pollbatchmatch?apikey={apiKey}&batchmatchid={batchMatchId}");

            webRequest.AddParameter("apiKey", _apiKey, ParameterType.UrlSegment);
            webRequest.AddParameter("batchMatchId", batchMatchId, ParameterType.UrlSegment);
            
            RestResponse<AutoBatchMatchResponse> webResponse = webClient.Execute<AutoBatchMatchResponse>(webRequest);

            return webResponse.Data;
        }

        public void SubmitBrokeredNames(string namesData, string email, string apiKey)
        {
            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("matches/submitbrokerednames?email={email}&apikey={apikey}");

            webRequest.Method = Method.POST;
            webRequest.AddParameter("email", email, ParameterType.UrlSegment);
            webRequest.AddParameter("apikey", apiKey, ParameterType.UrlSegment);
            webRequest.AddParameter(String.Empty, namesData, ParameterType.RequestBody);

            RestResponse webResponse = webClient.Execute(webRequest);

            if (webResponse.ResponseStatus == ResponseStatus.Error)
            {
                throw new Exception(webResponse.Content);
            }
        }

        public RegistrationResponse RegisterUser(string email, string name, string password, string organisation = "")
        {
            if (email == null) return null;

            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("users/registeruser?email={email}&name={name}&password={password}&organisation={organisation}");
            webRequest.AddParameter("email", email, ParameterType.UrlSegment);
            webRequest.AddParameter("name", name, ParameterType.UrlSegment);
            webRequest.AddParameter("password", password, ParameterType.UrlSegment);
            webRequest.AddParameter("organisation", organisation, ParameterType.UrlSegment);

            RestResponse<RegistrationResponse> webResponse = webClient.Execute<RegistrationResponse>(webRequest);

            if (webResponse.ResponseStatus == ResponseStatus.Error)
            {
                throw new Exception(webResponse.Content);
            }

            return webResponse.Data;
        }

        public RegistrationResponse CompleteRegistration(string userId)
        {
            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("users/complete?userId={userId}");

            webRequest.AddParameter("userId", userId, ParameterType.UrlSegment);

            RestResponse<RegistrationResponse> webResponse = webClient.Execute<RegistrationResponse>(webRequest);

            if (webResponse.ResponseStatus == ResponseStatus.Error)
            {
                throw new Exception(webResponse.Content);
            }

            return webResponse.Data;
        }

        public RegistrationResponse ResendAPIKey(string userId)
        {
            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("users/resendapikey?userId={userId}");

            webRequest.AddParameter("userId", userId, ParameterType.UrlSegment);

            RestResponse<RegistrationResponse> webResponse = webClient.Execute<RegistrationResponse>(webRequest);

            if (webResponse.ResponseStatus == ResponseStatus.Error)
            {
                throw new Exception(webResponse.Content);
            }

            return webResponse.Data;
        }

        public void SubmitFeedback(Feedback feedback)
        {
            if (feedback == null) return;

            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("feedback/submitfeedback");

            webRequest.AddParameter("sender", feedback.Sender);
            webRequest.AddParameter("senderEmail", feedback.SenderEmail);
            webRequest.AddParameter("message", feedback.Message);
            webRequest.AddParameter("nameId", feedback.NameId);

            string provs = "";
            foreach (string p in feedback.ProvidersToEmail) provs += p + ",";
            if (provs.Length > 0) webRequest.AddParameter("providersToEmail", provs);

            RestResponse webResponse = webClient.Execute(webRequest);

            if (webResponse.ResponseStatus == ResponseStatus.Error)
            {
                throw new Exception(webResponse.ErrorMessage);
            }
        }

        public StatisticsResponse GetStatistics(DateTime? fromDate, int? page, int? pageSize)
        {
            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("statistics?fromdate={fromDate}&page={page}&pageSize={pageSize}");

            webRequest.AddHeader("Accept", "application/json");
            webRequest.AddParameter("fromDate", fromDate.HasValue ? fromDate.ToString() : String.Empty, ParameterType.UrlSegment);
            webRequest.AddParameter("page", page.HasValue ? page.ToString() : String.Empty, ParameterType.UrlSegment);
            webRequest.AddParameter("pageSize", pageSize.HasValue ? pageSize.Value.ToString() : String.Empty, ParameterType.UrlSegment);

            RestResponse<StatisticsResponse> webResponse = webClient.Execute<StatisticsResponse>(webRequest);

            return webResponse.Data;
        }

        public SettingResponse GetSetting(string name)
        {
            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("admin/getsetting?name={name}");

            webRequest.AddHeader("Accept", "text");
            webRequest.AddParameter("name", name, ParameterType.UrlSegment);
                        
            RestResponse<SettingResponse> webResponse = webClient.Execute<SettingResponse>(webRequest);

            return webResponse.Data;
        }

        public void SetSetting(string name, string value)
        {
            var webClient = new RestClient(_serviceUrl.ToString());
            var webRequest = new RestRequest("admin/setsetting?name={name}&value={value}");

            webRequest.AddHeader("Accept", "application/json");
            webRequest.AddParameter("name", name, ParameterType.UrlSegment);
            webRequest.AddParameter("value", value, ParameterType.UrlSegment);

            RestResponse webResponse = webClient.Execute(webRequest);

            if (webResponse.ResponseStatus == ResponseStatus.Error)
            {
                throw new Exception(webResponse.ErrorMessage);
            }
        }
    }
}
