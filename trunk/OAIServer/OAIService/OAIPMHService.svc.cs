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
    //[System.ServiceModel.Activation.AspNetCompatibilityRequirements(RequirementsMode = System.ServiceModel.Activation.AspNetCompatibilityRequirementsMode.Allowed)]
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
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

        public XElement GetRecord(string repository, string identifier, string metadataPrefix)
        {
            try
            {
                GetRecordRequest req = new GetRecordRequest();
                return req.GetResultXml(repository, identifier, metadataPrefix);
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

        [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
        public XElement ListRecords(string repository, string from, string until, string set, string resumptionToken, string metadataPrefix)
        {
            try
            {
                OAIRequestSession session = OAIServer.OAIServer.GetResumptionSession(resumptionToken, from, until, metadataPrefix, repository, set);
                ListRecordsRequest req = new ListRecordsRequest(session);
                XElement result = req.GetResultXml(repository, metadataPrefix, set, from, until, resumptionToken);
                OAIServer.OAIServer.SaveSession();
                return result;
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
