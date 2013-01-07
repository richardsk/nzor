using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;
using System.Data.Objects.DataClasses;

namespace NZORConsumer
{
    public class ConsumerClient
    {
        public static bool Ping(string serviceUrl, out string error, out NZORServiceMessage processingMessage)
        {
            bool ok = true;
            error = "";
            processingMessage = new NZORServiceMessage();

            try
            {
                Uri url = new Uri(new Uri(serviceUrl), "vocabularies");

                processingMessage.Urls.Add(url.ToString());

                WebRequest r = HttpWebRequest.Create(url);
                r.ContentType = "text/xml";
                HttpWebResponse resp = (HttpWebResponse)r.GetResponse();
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    ok = false;
                    error = "HTTP response not OK. (" + resp.StatusDescription + ")";
                }
                else
                {
                    StreamReader rdr = new StreamReader(resp.GetResponseStream());
                    string xml = rdr.ReadToEnd();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    processingMessage.XmlResult += xml + Environment.NewLine;

                    if (doc.SelectSingleNode("/Response") == null)
                    {
                        error = "URL does not point to an NZOR service.";
                        ok = false;
                    }
                }
                resp.Close();
            }                
            catch (Exception ex)
            {
                error = ex.Message;
                ok = false;
            }

            return ok;
        }

        public static string[] GetAutoCompleteList(string serviceUrl, string startText, int maxResults, out NZORServiceMessage processMessage)
        {
            processMessage = new NZORServiceMessage();
            List<string> strList = new List<string>();

            try
            {
                Uri url = new Uri(new Uri(serviceUrl), "/names/lookups?query=" + startText + "&take=" + maxResults.ToString());
                WebRequest r = HttpWebRequest.Create(url);
                r.ContentType = "text/xml";
                WebResponse resp = r.GetResponse();

                processMessage.Urls.Add(url.ToString());

                StreamReader rdr = new StreamReader(resp.GetResponseStream());
                string xml = rdr.ReadToEnd();
                rdr.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                processMessage.XmlResult += xml + Environment.NewLine;
                                
                XmlNodeList nList = doc.SelectNodes("//string");
                foreach (XmlNode n in nList)
                {
                    strList.Add(n.InnerText);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error searching names: " + ex.Message);
            }

            return strList.ToArray();
        }

        public static Model.NameResultList Search(string searchText, string serviceUrl, int maxRows, out int totalCount, out NZORServiceMessage processMessage)
        {
            Model.NameResultList names = new Model.NameResultList();
            totalCount = 0;
            processMessage = new NZORServiceMessage();

            try
            {
                Uri url = new Uri(new Uri(serviceUrl), "/names/search?query=" + searchText + "*&pageSize=" + maxRows.ToString() + "&orderby=FullNameSort");
                WebRequest r = HttpWebRequest.Create(url);
                r.ContentType = "text/xml";
                WebResponse resp = r.GetResponse();

                processMessage.Urls.Add(url.ToString());

                StreamReader rdr = new StreamReader(resp.GetResponseStream());
                string xml = rdr.ReadToEnd();
                rdr.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                processMessage.XmlResult += xml + Environment.NewLine;

                int.TryParse(doc.SelectSingleNode("//Total").InnerText, out totalCount);

                XmlNodeList nList = doc.SelectNodes("//NameSearchResult/Name");
                foreach (XmlNode n in nList)
                {
                    Model.NameResult hn = LoadNameFromXNode(n);
                    names.Add(hn);
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error searching names: " + ex.Message);
            }

            return names;
        }

        public static List<string> ListKingdoms(string serviceUrl, out NZORServiceMessage processMessage)
        {
            List<string> vals= new List<string>();
            processMessage = new NZORServiceMessage();

            try
            {
                Uri url = new Uri(new Uri(serviceUrl), "/names/search?query=*&filter=rank:kingdom");
                
                processMessage.Urls.Add(url.ToString());

                WebRequest r = HttpWebRequest.Create(url);
                r.ContentType = "text/xml";
                WebResponse resp = r.GetResponse();

                StreamReader rdr = new StreamReader(resp.GetResponseStream());                
                string xml = rdr.ReadToEnd();
                rdr.Close();

                processMessage.XmlResult += xml + Environment.NewLine;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNodeList nList = doc.SelectNodes("//NameSearchResult/Name");
                foreach (XmlNode n in nList)
                {
                    vals.Add(n["PartialName"].InnerText + "  (" + n["GoverningCode"].InnerText + ")");
                }
            }
            catch (Exception ex)
            {
                processMessage.Error = "Error : " + ex.Message;
            }
            
            return vals;
        }

        public static List<string> ListOrigins(string serviceUrl, out NZORServiceMessage processMessage)
        {
            List<string> vals= new List<string>();
            processMessage = new NZORServiceMessage();

            try
            {
                Uri url = new Uri(new Uri(serviceUrl), "/vocabularies/biostatus");

                processMessage.Urls.Add(url.ToString());

                WebRequest r = HttpWebRequest.Create(url);
                r.ContentType = "text/xml";
                WebResponse resp = r.GetResponse();

                StreamReader rdr = new StreamReader(resp.GetResponseStream());                
                string xml = rdr.ReadToEnd();
                rdr.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                
                processMessage.XmlResult += xml + Environment.NewLine;

                XmlNodeList nList = doc.SelectNodes("//Vocabulary[Title='Origin']/Values/Value/Name");
                foreach (XmlNode n in nList)
                {
                    vals.Add(n.InnerText);
                }
            }
            catch (Exception ex)
            {
                processMessage.Error = "Error : " + ex.Message;
            }
            
            return vals;
        }

        public static List<string> ListOccurrences(string serviceUrl, out NZORServiceMessage processMessage)
        {
            List<string> vals = new List<string>();
            processMessage = new NZORServiceMessage();

            try
            {
                Uri url = new Uri(new Uri(serviceUrl), "/vocabularies/biostatus");
                processMessage.Urls.Add(url.ToString());

                WebRequest r = HttpWebRequest.Create(url);
                r.ContentType = "text/xml";
                WebResponse resp = r.GetResponse();
                
                StreamReader rdr = new StreamReader(resp.GetResponseStream());
                string xml = rdr.ReadToEnd();
                rdr.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                processMessage.XmlResult += xml + Environment.NewLine;

                XmlNodeList nList = doc.SelectNodes("//Vocabulary[Title='Occurrence']/Values/Value/Name");
                foreach (XmlNode n in nList)
                {
                    vals.Add(n.InnerText);
                }
            }
            catch (Exception ex)
            {
                processMessage.Error = "Error : " + ex.Message;
            }

            return vals;
        }

        public static List<string> ListGeographicSchemas(string serviceUrl, out NZORServiceMessage processMessage)
        {
            List<string> vals = new List<string>();
            processMessage = new NZORServiceMessage();

            try
            {
                Uri url = new Uri(new Uri(serviceUrl), "/vocabularies/geographicschemas");
                processMessage.Urls.Add(url.ToString());

                WebRequest r = HttpWebRequest.Create(url);
                r.ContentType = "text/xml";
                WebResponse resp = r.GetResponse();

                StreamReader rdr = new StreamReader(resp.GetResponseStream());
                string xml = rdr.ReadToEnd();
                rdr.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                processMessage.XmlResult += xml + Environment.NewLine;

                XmlNodeList nList = doc.SelectNodes("//GeographicSchema/Name");
                foreach (XmlNode n in nList)
                {
                    vals.Add(n.InnerText);
                }
            }
            catch (Exception ex)
            {
                processMessage.Error = "Error : " + ex.Message;
            }

            return vals;
        }

        public static List<string> ListGeoRegions(string serviceUrl, string geographicSchema, out NZORServiceMessage processMessage)
        {
            List<string> vals = new List<string>();
            processMessage = new NZORServiceMessage();

            try
            {
                Uri url = new Uri(new Uri(serviceUrl), "/vocabularies/geographicschemas");
                processMessage.Urls.Add(url.ToString());

                WebRequest r = HttpWebRequest.Create(url);
                r.ContentType = "text/xml";
                WebResponse resp = r.GetResponse();

                StreamReader rdr = new StreamReader(resp.GetResponseStream());
                string xml = rdr.ReadToEnd();
                rdr.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                processMessage.XmlResult += xml + Environment.NewLine;

                XmlNodeList nList = doc.SelectNodes("//GeographicSchema[Name='" + geographicSchema + "']/GeoRegions/GeoRegion/Name");
                foreach (XmlNode n in nList)
                {
                    vals.Add(n.InnerText);
                }
            }
            catch (Exception ex)
            {
                processMessage.Error = "Error : " + ex.Message;
            }

            return vals;
        }

        public static List<string> ListBiomes(string serviceUrl, out NZORServiceMessage processMessage)
        {
            List<string> vals = new List<string>();
            processMessage = new NZORServiceMessage();

            try
            {
                Uri url = new Uri(new Uri(serviceUrl), "/vocabularies/biostatus");
                processMessage.Urls.Add(url.ToString());

                WebRequest r = HttpWebRequest.Create(url);
                r.ContentType = "text/xml";
                WebResponse resp = r.GetResponse();

                StreamReader rdr = new StreamReader(resp.GetResponseStream());
                string xml = rdr.ReadToEnd();
                rdr.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                processMessage.XmlResult += xml + Environment.NewLine;

                XmlNodeList nList = doc.SelectNodes("//Vocabulary[Title='Biome']/Values/Value/Name");
                foreach (XmlNode n in nList)
                {
                    vals.Add(n.InnerText);
                }
            }
            catch (Exception ex)
            {
                processMessage.Error = "Error : " + ex.Message;
            }

            return vals;
        }

        public static Model.NameResultList GetUpdatedNames(string serviceUrl, DateTime modifiedSince, bool? acceptedNamesOnly,  
            string biostatus, string ancestorNameId, int pageNumber, ref int totalCount, out NZORServiceMessage processMessage)
        {
            Model.NameResultList results = new Model.NameResultList();
            processMessage = new NZORServiceMessage();

            string url = "/names?fromModifiedDate=" + modifiedSince.ToString("yyyy-MM-dd") + "&pageSize=100&page=" + pageNumber;

            if (acceptedNamesOnly.HasValue && acceptedNamesOnly.Value) url += "&status=Current";

            if (biostatus != null && biostatus.Length > 0)
            {
                url += "&biostatus=" + biostatus;
            }

            if (ancestorNameId != null && ancestorNameId.Length > 0)
            {
                url += "&ancestorNameId=" + ancestorNameId;
            }

            Uri u = new Uri(new Uri(serviceUrl), url);
            WebRequest r = HttpWebRequest.Create(u);
            r.Timeout = 200000;
            r.ContentType = "text/xml";
            WebResponse resp = null;
            try
            {
                resp = r.GetResponse();
            }
            catch (WebException wex)
            {
                string msg = "Failed to get response from web service.  Status: " + wex.Status.ToString();
                if (wex.Response != null)
                {
                    msg += " : Response Status Code : " + ((HttpWebResponse)wex.Response).StatusCode;
                    msg += " : Response Status Description : " + ((HttpWebResponse)wex.Response).StatusDescription;
                }
                throw new Exception(msg, wex);
            }

            StreamReader rdr = new StreamReader(resp.GetResponseStream());
            string xml = rdr.ReadToEnd();
            rdr.Close();

            processMessage.Urls.Add(u.ToString());
            processMessage.XmlResult += xml + Environment.NewLine;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            int.TryParse(doc.SelectSingleNode("//Total").InnerText, out totalCount);

            XmlNodeList nList = doc.SelectNodes("//Names/Name");
            foreach (XmlNode n in nList)
            {
                Model.NameResult hn = LoadNameFromXNode(n);
                results.Add(hn);
            }

            return results;
        }

        private static Model.NameResult LoadNameFromXNode(XmlNode nameNode)
        {
            Model.NameResult hn = new Model.NameResult();
            hn.Name = new Model.Name();
            hn.Name.NZORId = nameNode["NameId"].InnerText;
            hn.Name.FullName = nameNode["FullName"].InnerText;
            hn.Name.TaxonRank = nameNode["Rank"].InnerText;
            hn.Name.Authors = nameNode["Authors"].InnerText;
            hn.Name.GoverningCode = nameNode["GoverningCode"].InnerText;
            hn.Name.Year = nameNode["Year"].InnerText;

            XmlNode ann = nameNode.SelectSingleNode("AcceptedName");
            if (ann != null)
            {
                hn.Name.AcceptedNameId = ann["NameId"].InnerText;
            }
            XmlNode pnn = nameNode.SelectSingleNode("ParentName");
            if (pnn != null)
            {
                hn.Name.ParentNameId = pnn["NameId"].InnerText;
            }

            XmlNodeList provNames = nameNode.SelectNodes("ProviderNames/ProviderNameLink");
            foreach (XmlNode pn in provNames)
            {
                Model.Provider p = new Model.Provider();
                p.ProviderId = pn["ProviderCode"].InnerText;
                p.Name = "Unknown";

                if (hn.Providers.Where(pr => pr.ProviderId == p.ProviderId).FirstOrDefault() == null)
                {
                    hn.Providers.Add(p);
                }
            }

            return hn;
        }


        public static Model.BatchMatch SubmitBatchMatch(string serviceUrl, string fileName, bool brokerNames, out NZORServiceMessage processMessage)
        {
            Model.BatchMatch bm = null;
            processMessage = new NZORServiceMessage();

            try
            {
                string data = File.ReadAllText(fileName);

                Uri url = new Uri(new Uri(serviceUrl), "/matches/submitautobatchmatch?apikey=" + Settings.APIKey + "&brokermissingnames=" + brokerNames.ToString());
                processMessage.Urls.Add(url.ToString());

                WebRequest r = HttpWebRequest.Create(url);
                r.ContentType = "text/xml";
                r.Method = "POST";
                r.ContentLength = UTF8Encoding.UTF8.GetByteCount(data);
                StreamWriter wr = new StreamWriter(r.GetRequestStream());
                wr.Write(data);
                wr.Flush();

                WebResponse resp = r.GetResponse();
                StreamReader rdr = new StreamReader(resp.GetResponseStream());
                string xml = rdr.ReadToEnd();
                rdr.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                processMessage.XmlResult += xml + Environment.NewLine;

                bm = LoadBatchMatch(doc);
                bm.DateSubmitted = DateTime.Now;
                bm.Filename = fileName;
            }
            catch (Exception ex)
            {
                processMessage.Error = "Error : " + ex.Message;
            }

            return bm;
        }

        public static Model.BatchMatch GetBatchMatch(string serviceUrl, string batchMatchId, out NZORServiceMessage processMessage)
        {
            Model.BatchMatch bm = null;
            processMessage = new NZORServiceMessage();

            try
            {
                Uri url = new Uri(new Uri(serviceUrl), "/matches/pollbatchmatch?apikey=" + Settings.APIKey + "&batchMatchId=" + batchMatchId);
                processMessage.Urls.Add(url.ToString());

                WebRequest r = HttpWebRequest.Create(url);
                r.ContentType = "text/xml";
                WebResponse resp = r.GetResponse();

                StreamReader rdr = new StreamReader(resp.GetResponseStream());
                string xml = rdr.ReadToEnd();
                rdr.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                processMessage.XmlResult += xml + Environment.NewLine;

                bm = LoadBatchMatch(doc);
            }
            catch (Exception ex)
            {
                processMessage.Error = "Error : " + ex.Message;
            }

            return bm;
        }

        private static Model.BatchMatch LoadBatchMatch(XmlDocument doc)
        {
            Model.BatchMatch bm = null;

            XmlNode node = doc.SelectSingleNode("//AutoBatchMatchResponse/BatchMatchId");
            if (node != null)
            {
                bm = new Model.BatchMatch();
                bm.MatchId = node.InnerText;

                node = doc.SelectSingleNode("//AutoBatchMatchResponse/Status");
                if (node != null) bm.Status = node.InnerText;

                node = doc.SelectSingleNode("//AutoBatchMatchResponse/DownloadUrl");
                if (node != null) bm.DownloadUrl = node.InnerText;
            }
            return bm;
        }

        public static List<Model.NameRequest> GetNameRequests(string serviceUrl, out NZORServiceMessage processMessage)
        {
            List<Model.NameRequest> requests = new List<Model.NameRequest>();
            processMessage = new NZORServiceMessage();

            try
            {
                Uri url = new Uri(new Uri(serviceUrl), "/admin/getnamerequests?apikey=" + Settings.APIKey);
                processMessage.Urls.Add(url.ToString());

                WebRequest r = HttpWebRequest.Create(url);
                r.ContentType = "text/xml";
                WebResponse resp = r.GetResponse();

                StreamReader rdr = new StreamReader(resp.GetResponseStream());
                string xml = rdr.ReadToEnd();
                rdr.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                processMessage.XmlResult += xml + Environment.NewLine;

                XmlNodeList nodes = doc.SelectNodes("//NameRequestResponse");
                foreach (XmlNode node in nodes)
                {
                    Model.NameRequest nr = new Model.NameRequest();
                    XmlNode n = node.SelectSingleNode("BatchMatchId");
                    if (n != null) nr.BatchMatchId = n.InnerText;
                    DateTime dt = DateTime.MinValue;
                    n = node.SelectSingleNode("RequestDate");
                    if (n != null && DateTime.TryParse(n.InnerText, out dt)) nr.DateRequested = dt;
                    nr.FullName = node.SelectSingleNode("FullName").InnerText;
                    n = node.SelectSingleNode("NameRequestId");
                    if (n != null) nr.NameRequestId = n.InnerText;
                    n = node.SelectSingleNode("BrokeredNames");
                    if (n != null) nr.ProviderNameResults = n.OuterXml;
                    n = node.SelectSingleNode("Status");
                    if (n != null) nr.Status = n.InnerText;
                    
                    requests.Add(nr);
                }
            }
            catch (Exception ex)
            {
                processMessage.Error = "Error : " + ex.Message;
            }

            return requests;
        }

    }

}
