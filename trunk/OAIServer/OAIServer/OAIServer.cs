using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace OAIServer
{
    public class OAIServer
    {
        public static List<RepositoryConfig> ConfigList = new List<RepositoryConfig>();
        public static String WebDir = "";

        private static ExpiringList _OAISessions = null;

        public static OAIRequestSession GetResumptionSession(String resumptionToken, String fromDate, String toDate, String metadataPrefix, String repository, String set)
        {
            OAIRequestSession req = null;

            req = new OAIRequestSession();

            if (resumptionToken != null && resumptionToken.Length > 0)
            {
                ExpiringListItem eli = _OAISessions.Get(resumptionToken.ToUpper());
                if (eli == null || eli.Value == null) throw new OAIException(OAIError.badResumptionToken);
                OAIRequestSession storedReq = (OAIRequestSession)eli.Value;
                if (storedReq.ResumptionToken.ToUpper() != resumptionToken.ToUpper()) throw new OAIException(OAIError.badResumptionToken);

                req.NumRecords = storedReq.NumRecords;
                req.Cursor = storedReq.Cursor;
                foreach (String key in storedReq.NextRecordPositions.Keys)
                {
                    req.NextRecordPositions.Add(key, storedReq.NextRecordPositions[key]);
                }
            }

            req.CallerIP = System.ServiceModel.OperationContext.Current.IncomingMessageProperties[System.ServiceModel.Channels.RemoteEndpointMessageProperty.Name].ToString();
            req.CallDate = DateTime.Now;
            req.FromDate = fromDate;
            req.ToDate = toDate;
            req.MetadataPrefix = metadataPrefix;
            req.Repository = repository;
            req.Set = set;
            req.ResumptionToken = Guid.NewGuid().ToString();
            
            int hours = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TokenExpirationHours"]);
            req.ResumptionExpiry = DateTime.Now.AddHours(hours);

            _OAISessions.Add(new ExpiringListItem(req.ResumptionToken.ToUpper(), req, req.ResumptionExpiry));
            
            _OAISessions.Save(System.IO.Path.Combine(WebDir, "Config"));

            return req;
        }

        public static void SaveSession()
        {
            _OAISessions.Save(System.IO.Path.Combine(WebDir, "Config"));
        }

        public static void Load(String webDir)
        {
            WebDir = webDir;
            ConfigList.Clear();
            
            String[] files = System.IO.Directory.GetFiles(System.IO.Path.Combine(WebDir, "Services"), "*.xml");
            foreach (String f in files)
            {
                RepositoryConfig rc = LoadConfig(f);
                if (rc != null) ConfigList.Add(rc);
            }

            _OAISessions = null;
            try
            {
                _OAISessions = ExpiringList.Load(System.IO.Path.Combine(WebDir, "Config"), "OAISessions");                
            }
            catch (Exception)
            {
                //mustn't exist yet
            }
            if (_OAISessions == null) _OAISessions = new ExpiringList("OAISessions");

        }

        private static RepositoryConfig LoadConfig(String filename)
        {
            RepositoryConfig rc = null;
            
            try
            {
                rc = new RepositoryConfig();
                rc.Load(filename);
            }
            catch(Exception ex)
            {
                Log.LogError(ex);
            }

            return rc;
        }

        public static RepositoryConfig GetConfig(String repository)
        {
            RepositoryConfig rep = null;

            foreach (RepositoryConfig r in ConfigList)
            {
                if (r.Name == repository)
                {
                    rep = r;
                    break;
                }
            }

            return rep;
        }

        public static Object GetFixedFieldValue(String repository, string field)
        {
            Object val = null;

            RepositoryConfig rep = GetConfig(repository);
            
            if (repository == null) throw new OAIException(OAIError.badArgument);

            if (field == FieldMapping.ADMIN_EMAIL)
            {
                val = rep.AdminEmail;
            }
            else if (field == FieldMapping.PROVIDER_ID)
            {
                val = rep.ProviderId;
            }
            else if (field == FieldMapping.PROVIDER_NAME)
            {
                val = rep.ProviderName;
            }
            else if (field == FieldMapping.ORGANISATION_URL)
            {
                val = rep.OrganisationUrl;
            }
            else if (field == FieldMapping.METADATA_DATE)
            {
                val = rep.MetadataDate;
            }
            else if (field == FieldMapping.DISCLAIMER)
            {
                val = rep.Disclaimer;
            }
            else if (field == FieldMapping.ATTRIBUTION)
            {
                val = rep.Attribution;
            }
            else if (field == FieldMapping.LICENSING)
            {
                val = rep.Licensing;
            }
            else if (field == FieldMapping.REPOSITORY_NAME)
            {
                val = repository;
            }
            else
            {
                foreach (DataConnection dc in rep.DataConnections)
                {
                    foreach (FieldMapping fm in dc.Mappings)
                    {
                        if (fm.GetType() == typeof(FixedValueMapping))
                        {
                            FixedValueMapping fvm = (FixedValueMapping)fm;
                            if (fvm.Field == field)
                            {
                                val = fvm.Value;
                                break;
                            }
                        }
                        else if (fm.GetType() == typeof(SQLMaxValueMapping) || fm.GetType() == typeof(SQLMinValueMapping))
                        {
                            DatabaseMapping dbm = (DatabaseMapping)fm;
                            if (dbm.Field == field)
                            {
                                val = dbm.GetValue(dc);
                                break;
                            }
                        }
                    }

                    if (val != null) break;
                }
            }

            return val;
        }

        public static Object GetFieldValue(String recordId, String field)
        {
            Object val = null;

            return val;
        }

    }
}
