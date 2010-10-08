using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ServiceModel.Web;
using System.IO;
using System.Web;

namespace OAIServer
{
    public class IdentifyRequest
    {
        public XElement GetResultXml(String repository)
        {
            WebOperationContext ctx = WebOperationContext.Current;

            string xml = File.ReadAllText(Path.Combine(OAIServer.WebDir, "Responses\\IdentifyResponse.xml"));
            xml = xml.Replace(FieldMapping.GET_DATE_TIME, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));

            string url = System.ServiceModel.OperationContext.Current.IncomingMessageHeaders.To.OriginalString;
            if (url.IndexOf("?") != -1) url = url.Substring(0, url.IndexOf("?"));
            xml = xml.Replace(FieldMapping.BASE_URL, url);

            Object val = OAIServer.GetFixedFieldValue(repository, FieldMapping.ADMIN_EMAIL);
            Utility.ReplaceXmlField(ref xml, FieldMapping.ADMIN_EMAIL, val);

            val = OAIServer.GetFixedFieldValue(repository, FieldMapping.EARLIEST_DATE);
            Utility.ReplaceXmlField(ref xml, FieldMapping.EARLIEST_DATE, val);

            val = OAIServer.GetFixedFieldValue(repository, FieldMapping.REPOSITORY_NAME);
            Utility.ReplaceXmlField(ref xml, FieldMapping.REPOSITORY_NAME, val);

            return XElement.Parse(xml);
        }
    }
}
