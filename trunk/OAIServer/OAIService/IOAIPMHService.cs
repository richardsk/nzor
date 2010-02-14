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
        [WebGet(UriTemplate = "{service}?verb=Identify")]
        [OperationContract]
        XElement Identify(string service);
        [WebGet(UriTemplate = "{service}?verb=GetRecord&identifier={id}&metadataPrefix={mdPrefix}")]
        [OperationContract]
        XElement GetRecord(string service, string id, string mdPrefix);
        [OperationContract]
        XElement ListIdentifiers();
        [OperationContract]
        XElement ListMetadataFormats();
        [OperationContract]
        XElement ListRecords();
        [OperationContract]
        XElement ListSets();

    }

}
