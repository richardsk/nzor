using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.IO;
using System.Web;
using System.Xml.Linq;

namespace OAIServer
{
    public class ListSetsRequest
    {

        public XElement GetResultXml(String repository)
        {
            WebOperationContext ctx = WebOperationContext.Current;

            RepositoryConfig rep = OAIServer.GetConfig(repository);

            if (rep == null) throw new OAIException(OAIError.badArgument);

            string xml = File.ReadAllText(Path.Combine(OAIServer.WebDir, "Responses\\ListSetsResponse.xml"));
            xml = xml.Replace(FieldMapping.GET_DATE_TIME, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));

            string setList = "";
            string snippet = File.ReadAllText(Path.Combine(OAIServer.WebDir, "Responses\\ListSetsSnippet.xml"));
            foreach (string s in rep.Sets)
            {
                setList += snippet.Replace(FieldMapping.SET_NAME, s);
            }

            xml = xml.Replace(FieldMapping.SET_LIST, setList);

            string url = System.ServiceModel.OperationContext.Current.IncomingMessageHeaders.To.OriginalString;
            if (url.IndexOf("?") != -1) url = url.Substring(0, url.IndexOf("?"));
            xml = xml.Replace(FieldMapping.BASE_URL, url);

            return XElement.Parse(xml);
        }
    }
}
