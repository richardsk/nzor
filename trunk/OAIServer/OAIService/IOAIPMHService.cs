using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Linq;

namespace OAIService
{
    // NOTE: If you change the interface name "IService1" here, you must also update the reference to "IService1" in Web.config.
    [ServiceContract]
    public interface IOAIPMHService
    {
        [WebGet(UriTemplate = "{repository}?verb=Identify")]
        [OperationContract]
        XElement Identify(string repository);
        [WebGet(UriTemplate = "{repository}?verb=GetRecord&identifier={id}&metadataPrefix={mdPrefix}")]
        [OperationContract]
        XElement GetRecord(string repository, string id, string mdPrefix);
        [OperationContract]
        XElement ListIdentifiers();
        [OperationContract]
        XElement ListMetadataFormats();
        [OperationContract]
        [WebGet(UriTemplate = "{repository}?verb=ListRecords&from={fromDate}&until={toDate}&set={set}&resumptionToken={resumptionToken}&metadataPrefix={mdPrefix}")]
        XElement ListRecords(string repository, string fromDate, string toDate, string set, string resumptionToken, string mdPrefix);
        [OperationContract]
        XElement ListSets();

    }

}
