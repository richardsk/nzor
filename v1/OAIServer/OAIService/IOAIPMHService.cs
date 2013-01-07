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
    [ServiceContract()]
    public interface IOAIPMHService
    {
        [OperationContract]
        [WebGet(UriTemplate = "{repository}?verb=Identify")]
        XElement Identify(string repository);
        [OperationContract]
        [WebGet(UriTemplate = "{repository}?verb=GetRecord&identifier={identifier}&metadataPrefix={metadataPrefix}")]
        XElement GetRecord(string repository, string identifier, string metadataPrefix);
        [OperationContract]
        [WebGet(UriTemplate = "{repository}?verb=ListIdentifiers&from={from}&until={until}&set={set}&resumptionToken={resumptionToken}&metadataPrefix={metadataPrefix}")]
        XElement ListIdentifiers(string repository, string from, string until, string set, string resumptionToken, string metadataPrefix);
        [OperationContract]
        [WebGet(UriTemplate = "{repository}?verb=ListMetadataFormats&identifier={identifier}")]
        XElement ListMetadataFormats(string repository, string identifier);
        [OperationContract]
        [WebGet(UriTemplate = "{repository}?verb=ListRecords&from={from}&until={until}&set={set}&resumptionToken={resumptionToken}&metadataPrefix={metadataPrefix}")]
        XElement ListRecords(string repository, string from, string until, string set, string resumptionToken, string metadataPrefix);
        [OperationContract]
        [WebGet(UriTemplate = "{repository}?verb=ListSets")]
        XElement ListSets(string repository);

    }

}
