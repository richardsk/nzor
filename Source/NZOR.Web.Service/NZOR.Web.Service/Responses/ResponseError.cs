using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace NZOR.Web.Service.Responses
{
    [XmlRoot(ElementName = "Error")]
    public class ResponseError
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}