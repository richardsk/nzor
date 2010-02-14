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

        public XElement Identify(string repository)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
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
            catch (Exception ex)
            {
                Log.LogError(ex);
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

        public XElement ListRecords()
        {
            throw new NotImplementedException();
        }

        public XElement ListSets()
        {
            throw new NotImplementedException();
        }

    }
}
