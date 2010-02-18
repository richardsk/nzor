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
        public XElement GetResultXml(String repository, String set)
        {
            WebOperationContext ctx = WebOperationContext.Current;

            string xml = File.ReadAllText(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Responses\\IdentifyResponse.xml"));
            xml = xml.Replace(FieldMapping.GET_DATE, DateTime.Now.ToString());

            string url = HttpContext.Current.Request.Url.AbsoluteUri;
            if (url.IndexOf("?") != -1) url = url.Substring(0, url.IndexOf("?"));
            xml = xml.Replace(FieldMapping.BASE_URL, url);

            Object val = OAIServer.GetFixedFieldValue(repository, set, FieldMapping.ADMIN_EMAIL);
            Utility.ReplaceXmlField(ref xml, FieldMapping.ADMIN_EMAIL, val);

            val = OAIServer.GetFixedFieldValue(repository, set, FieldMapping.EARLIEST_DATE);
            Utility.ReplaceXmlField(ref xml, FieldMapping.EARLIEST_DATE, val);

            val = OAIServer.GetFixedFieldValue(repository, set, FieldMapping.REPOSITORY_NAME);
            Utility.ReplaceXmlField(ref xml, FieldMapping.REPOSITORY_NAME, val);

            return XElement.Parse(xml);
        }
    }
}
