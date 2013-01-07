using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using NZOR.Admin.Data.Entities;
using NZOR.Admin.Data.Sql.Repositories;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Data.Sql.Repositories.Provider;

namespace NZOR.Harvest
{
    public class Harvester
    {
        public int Progress = 0;
        public string StatusText = "";
        public int TotalHarvested = 0;

        public List<String> Log = new List<string>();
        public List<String> Errors = new List<string>();

        private string _cnnStr = "";

        /// <summary>
        /// Iterates through all providers and their endpoints and checks the schedules to see if they need harvesting.  If they do an OAI harvest is run on them.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="adminConnectionString"></param>
        public void RunHarvesting(string connectionString, bool processUpdates)
        {
            _cnnStr = connectionString;

            TotalHarvested = 0;
            bool anyHarvested = false;

            Progress = 0;
            StatusText = "";

            Log.Clear();
            Errors.Clear();

            try
            {
                NZOR.Admin.Data.Repositories.IProviderRepository pr = new ProviderRepository(_cnnStr);
                List<Provider> providers = pr.GetProviders();

                Dictionary<Guid, List<DataSourceEndpoint>> datasourceEndpoints = new Dictionary<Guid, List<DataSourceEndpoint>>();
                int totalEndpoints = 0;
                int numHarvested = 0;

                foreach (Provider p in providers)
                {
                    foreach (DataSource ds in p.DataSources)
                    {
                        List<DataSourceEndpoint> dseList = pr.GetDatasetEndpoints(ds.DataSourceId);
                        datasourceEndpoints.Add(ds.DataSourceId, dseList);

                        totalEndpoints += dseList.Count;
                    }
                }

                foreach (Provider p in providers)
                {
                    foreach (DataSource ds in p.DataSources)
                    {
                        List<DataSourceEndpoint> dseList = datasourceEndpoints[ds.DataSourceId];

                        foreach (DataSourceEndpoint dse in dseList)
                        {
                            bool doHarvest = (!dse.LastHarvestDate.HasValue);

                            if (dse.LastHarvestDate.HasValue && dse.Schedule != null)
                            {
                                if (dse.LastHarvestDate.Value.AddDays(dse.Schedule.FrequencyDays) < DateTime.Now)
                                {
                                    doHarvest = true;
                                }
                            }

                            if (doHarvest)
                            {
                                Log.Add(DateTime.Now.ToString() + " : Harvesting " + dse.Name);

                                DateTime harvestTime = DateTime.Now;

                                if (HarvestEndpoint(dse, p, ds.Code, _cnnStr, processUpdates))
                                {
                                    dse.LastHarvestDate = harvestTime;
                                    dse.State = Entity.EntityState.Modified;

                                    pr.SaveDataSourceEndpoint(dse);

                                    anyHarvested = true;
                                }
                            }

                            numHarvested++;
                            Progress = numHarvested * 100 / totalEndpoints;
                            StatusText = "Harvested " + numHarvested.ToString() + " of " + totalEndpoints.ToString() + " provider endpoints";

                        }
                    }

                }

                StatusText = "Completed";
            }
            catch (Exception ex)
            {
                Errors.Add(DateTime.Now.ToString() + ": Harvesting failed.  " + ex.Message + " : " + ex.StackTrace);
                StatusText = "Error";
            }

            if (Errors.Count > 0 && !anyHarvested) StatusText = "Error";

            Progress = 100;
        }

        public void RunInitialHarvest(Admin.Data.Entities.ProviderCode providerCode, string providerDBConnectionString, string nzorDBConnectionString)
        {
            Progress = 0;
            StatusText = "";

            string sql = "";
            if (providerCode == Admin.Data.Entities.ProviderCode.NZFLORA)
            {
                sql = NZOR.Data.Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.NZFLORA XML Initial Dataset.sql");
            }
            if (providerCode == Admin.Data.Entities.ProviderCode.NZIB)
            {
                sql = NZOR.Data.Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.NZIB XML Initial Dataset.sql");
            }
            if (providerCode == Admin.Data.Entities.ProviderCode.NZFUNGI)
            {
                sql = NZOR.Data.Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.NZFUNGI XML Initial Dataset.sql");
            }
            if (providerCode == Admin.Data.Entities.ProviderCode.NZAC)
            {
                sql = NZOR.Data.Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.NZAC XML Initial Dataset.sql");
            }
            if (providerCode == Admin.Data.Entities.ProviderCode.NZOR_Hosted)
            {
                sql = NZOR.Data.Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.NZOR_Hosted XML Initial Dataset.sql");
            }
            if (providerCode == Admin.Data.Entities.ProviderCode.NZOR_Test)
            {
                sql = NZOR.Data.Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.NZOR Test XML Initial Dataset.sql");
            }
            if (providerCode == Admin.Data.Entities.ProviderCode.NZOR_Test_2)
            {
                sql = NZOR.Data.Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.NZOR Test 2 XML Initial Dataset.sql");
            }

            if (sql != "")
            {
                XDocument doc = null;
                string xml = "";

                StatusText = "Loading data...";
                using (SqlConnection cnn = new SqlConnection(providerDBConnectionString))
                {
                    cnn.Open();

                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;
                        cmd.CommandTimeout = 200000;

                        System.Xml.XmlReader rdr = cmd.ExecuteXmlReader();
                        rdr.Read();

                        while (rdr.ReadState != System.Xml.ReadState.EndOfFile)
                        {
                            xml += rdr.ReadOuterXml();
                        }
                    }

                    cnn.Close();
                }

                if (xml != "")
                {
                    doc = XDocument.Parse(xml);

                    //System.Xml.XmlWriter wr = System.Xml.XmlTextWriter.Create("c:\\temp\\data.xml");
                    //doc.WriteTo(wr);
                    //wr.Flush();
                    //wr.Close();

                    NameRepository nameRepository = new NameRepository(nzorDBConnectionString);
                    ReferenceRepository referenceRepository = new ReferenceRepository(nzorDBConnectionString);
                    ConceptRepository conceptRepository = new ConceptRepository(nzorDBConnectionString);
                    LookUpRepository lookUpRepository = new LookUpRepository(nzorDBConnectionString);
                    TaxonPropertyRepository taxonPropRepository = new TaxonPropertyRepository(nzorDBConnectionString);
                    AnnotationRepository annotationRepository = new AnnotationRepository(nzorDBConnectionString);
                    ProviderRepository provRepository = new ProviderRepository(nzorDBConnectionString);

                    Importer importer = new Importer(nameRepository, referenceRepository, conceptRepository, lookUpRepository, taxonPropRepository, annotationRepository, provRepository, "http://data.nzor.org.nz/schema/provider/103/nzor_provider.xsd");

                    Progress = 10;
                    StatusText = "Parsing XML...";

                    importer.Import(doc);

                    Progress = 20;
                    StatusText = "Importing data...";

                    importer.Save();

                }
            }

            Progress = 100;
            StatusText = "Completed";
        }

        public bool HarvestEndpoint(DataSourceEndpoint dse, Provider provider, string dataSourceCode, string nzorDBConnectionString, bool processUpdates)
        {
            XDocument doc = null;
            StatusText = "";
            bool successfulHarvest = true;
            
            DateTime dateBeforeHarvest = DateTime.Now;

            try
            {
                //call OAI endpoint
                string urlStr = dse.Url + "?verb=ListRecords&metadataPrefix=nzor";
                if (dse.LastHarvestDate.HasValue) urlStr += "&from=" + dse.LastHarvestDate.Value.ToString("s");
                Uri url = null;
                if (Uri.TryCreate(urlStr, UriKind.Absolute, out url))
                {
                    string proxyServer = System.Configuration.ConfigurationManager.AppSettings["proxyServer"];
                    string proxyPort = System.Configuration.ConfigurationManager.AppSettings["proxyPort"];

                    if (proxyServer != null && proxyPort != null && proxyServer.Length > 0) System.Net.WebRequest.DefaultWebProxy = new System.Net.WebProxy(proxyServer + ":" + proxyPort);

                    WebRequest req = HttpWebRequest.Create(url);
                    req.Timeout = 500000;

                    using (System.IO.Stream str = req.GetResponse().GetResponseStream())
                    {
                        doc = XDocument.Load(str);
                    }

                    string token = null;
                    if (doc.Descendants(XName.Get("resumptionToken", "http://www.openarchives.org/OAI/2.0/")).Count() > 0)
                    {
                        token = doc.Descendants(XName.Get("resumptionToken", "http://www.openarchives.org/OAI/2.0/")).First().Value;
                    }

                    //repeat until no resumption token
                    while (token != null && token != "")
                    {
                        req = HttpWebRequest.Create(url + "&resumptionToken=" + token);
                        req.Timeout = 500000;

                        using (System.IO.Stream str = req.GetResponse().GetResponseStream())
                        {
                            XDocument nextDoc = XDocument.Load(str);

                            doc.Descendants(XName.Get("ListRecords", "http://www.openarchives.org/OAI/2.0/")).First().Add(nextDoc.Descendants(XName.Get("record", "http://www.openarchives.org/OAI/2.0/")));

                            token = null;
                            if (nextDoc.Descendants(XName.Get("resumptionToken", "http://www.openarchives.org/OAI/2.0/")).Count() > 0)
                            {
                                token = nextDoc.Descendants(XName.Get("resumptionToken", "http://www.openarchives.org/OAI/2.0/")).First().Value;
                            }
                        }
                    }

                    //get deprecated records OAI status=deleted
                    IEnumerable<XElement> deprecated = from h in doc.Descendants(XName.Get("header", "http://www.openarchives.org/OAI/2.0/")) where h.Attribute("status") != null && h.Attribute("status").Value == "deleted" select h;

                    List<Data.Entities.Integration.DeprecatedRecord> depRecords = new List<Data.Entities.Integration.DeprecatedRecord>();
                    foreach (XElement depEl in deprecated)
                    {
                        XElement idEl = depEl.Element(XName.Get("identifier", "http://www.openarchives.org/OAI/2.0/"));
                        XElement specEl = depEl.Element(XName.Get("setSpec", "http://www.openarchives.org/OAI/2.0/"));

                        if (idEl != null && specEl != null)
                        {
                            Data.Entities.Integration.DeprecatedRecord dr = new Data.Entities.Integration.DeprecatedRecord();
                            dr.ProviderCode = provider.Code;
                            dr.ProviderRecordId = idEl.Value;
                            dr.DataSourceCode = dataSourceCode;

                            switch (specEl.Value)
                            {
                                case "TaxonName":
                                case "VernacularName":
                                    dr.RecordType = Admin.Data.Entities.NZORRecordType.Name;
                                    break;
                                case "TaxonNameUse":
                                case "VernacularUse":
                                case "TaxonConcept":
                                case "VernacularConcept":
                                    dr.RecordType = Admin.Data.Entities.NZORRecordType.Concept;
                                    break;
                                case "Publication":
                                    dr.RecordType = Admin.Data.Entities.NZORRecordType.Reference;
                                    break;
                                case "Biostatus":
                                    dr.RecordType = Admin.Data.Entities.NZORRecordType.TaxonProperty;
                                    break;
                                default:
                                    dr.RecordType = Admin.Data.Entities.NZORRecordType.None;
                                    break;
                            }

                            depRecords.Add(dr);
                        }
                    }

                    //apply xml transform
                    Transformers.OaiTransformer transformer = new Transformers.OaiTransformer();

                    string txDocName = ConfigurationManager.AppSettings["OAITransformFile"];
                    Log.Add("Transformation document: " + txDocName);

                    XDocument txDoc = transformer.Transform(doc, txDocName);
                    
                    //import data
                    if (txDoc != null)
                    {
                        if (txDoc.Descendants("Metadata").SingleOrDefault() != null)
                        {
                            NameRepository nameRepository = new NameRepository(nzorDBConnectionString);
                            ReferenceRepository referenceRepository = new ReferenceRepository(nzorDBConnectionString);
                            ConceptRepository conceptRepository = new ConceptRepository(nzorDBConnectionString);
                            LookUpRepository lookUpRepository = new LookUpRepository(nzorDBConnectionString);
                            TaxonPropertyRepository taxonPropRepository = new TaxonPropertyRepository(nzorDBConnectionString);
                            AnnotationRepository annotationRepository = new AnnotationRepository(nzorDBConnectionString);
                            ProviderRepository provRepository = new ProviderRepository(nzorDBConnectionString);

                            string provSchemaUrl = ConfigurationManager.AppSettings.Get("ProviderSchemaUrl");
                            Importer importer = new Importer(nameRepository, referenceRepository, conceptRepository, lookUpRepository, taxonPropRepository, annotationRepository, provRepository, provSchemaUrl);

                            StatusText = "Parsing XML...";

                            importer.Import(txDoc);

                            
                            TotalHarvested += importer.TotalHarvested;

                            StatusText = "Importing data...";

                            importer.Save();

                            Log.Add(DateTime.Now.ToString() + " : Imported " + importer.TotalHarvested.ToString() + " records from " + dse.Name);

                            if (processUpdates)
                            {
                                //process any changes after import                
                                string intConfig = System.Configuration.ConfigurationManager.AppSettings["Integration Config File"];
                                System.Xml.XmlDocument config = new System.Xml.XmlDocument();
                                config.Load(intConfig);

                                NZOR.Integration.UpdateProcessor proc = new Integration.UpdateProcessor();
                                proc.ProcessUpdatedProviderData(dateBeforeHarvest, config, depRecords);

                                if (proc.Messages.Count > 0) Log.AddRange(proc.Messages);
                            }
                        }
                        else
                        {
                            Log.Add("No metadata records found from endpoint dataset " + dse.Name + ".  XML : " + doc + "; TxDoc : " + txDoc.ToString());
                            if (doc.ToString().IndexOf("<Error>") != -1)
                            {
                                Errors.Add("ERROR: " + doc);
                                successfulHarvest = false;
                            }
                        }
                    }
                    else if (doc.ToString().IndexOf("nzor:DataSet") != -1)
                    {
                        Errors.Add("ERROR : Failed to transform data from endpoint dataset " + dse.Name + ".");
                        successfulHarvest = false;
                    }
                }
                else
                {
                    Errors.Add("ERROR : Endpoint for dataset " + dse.Name + " is invalid - " + urlStr);
                    successfulHarvest = false;
                }
            }
            catch (Exception ex)
            {
                Errors.Add("ERROR : Failed to harvest endpoint for dataset " + dse.Name + " : " + ex.Message + " : " + ex.StackTrace);
                successfulHarvest = false;
            }
            
            return successfulHarvest;
        }
    }
}
