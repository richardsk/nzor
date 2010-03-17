﻿using System;
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
            xml = xml.Replace(FieldMapping.GET_DATE, DateTime.Now.ToString());

            string url = System.ServiceModel.OperationContext.Current.IncomingMessageHeaders.To.OriginalString;
            if (url.IndexOf("?") != -1) url = url.Substring(0, url.IndexOf("?"));
            xml = xml.Replace(FieldMapping.BASE_URL, url);

            return XElement.Parse(xml);
        }
    }
}
