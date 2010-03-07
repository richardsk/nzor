using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using System.Web;
using System.Configuration;
using System.ServiceModel.Web;
using System.IO;
using OAIServer;

namespace OAIService
{
    // NOTE: If you change the class name "Service1" here, you must also update the reference to "Service1" in Web.config and in the associated .svc file.
    [System.ServiceModel.Activation.AspNetCompatibilityRequirements(RequirementsMode = System.ServiceModel.Activation.AspNetCompatibilityRequirementsMode.Allowed)] 
    public class OAIPMHService : IOAIPMHService
    {
        private bool IsDebug()
        {
            bool isDebug = false;
            try
            {
                isDebug = bool.Parse(ConfigurationManager.AppSettings["DEBUG"]);
            }
            catch (Exception)
            { }
            return isDebug;
        }

        public XElement Identify(string repository)
        {
            try
            {
                IdentifyRequest req = new IdentifyRequest();
                return req.GetResultXml(repository);
            }
            catch (OAIException oex)
            {
                return XElement.Parse(oex.ToString());
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                if (IsDebug()) return XElement.Parse("<Error>" + ex.Message + "</Error>");
            }
            return null;
        }

        public XElement GetRecord(string repository, string id, string mdPrefix)
        {
            try
            {
                GetRecordRequest req = new GetRecordRequest();
                return req.GetResultXml(repository, id, mdPrefix);
            }
            catch (OAIException oex)
            {
                return XElement.Parse(oex.ToString());
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                if (IsDebug()) return XElement.Parse("<Error>" + ex.Message + "</Error>");
            }

            return null;
        }

        public XElement ListIdentifiers()
        {
            throw new NotImplementedException();
        }

        public XElement ListMetadataFormats()
        {
            throw new NotImplementedException();
        }

        public XElement ListRecords(string repository, string fromDate, string toDate, string set, string resumptionToken, string mdPrefix)
        {
            try
            {
                ListRecordsRequest req = new ListRecordsRequest();
                return req.GetResultXml(repository, mdPrefix, set, fromDate, toDate, resumptionToken);
            }
            catch (OAIException oex)
            {
                return XElement.Parse(oex.ToString());
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                if (IsDebug()) return XElement.Parse("<Error>" + ex.Message + "</Error>");
            }
            return null;
        }

        public XElement ListSets()
        {
            throw new NotImplementedException();
        }

    }
}
