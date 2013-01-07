using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZORConsumer
{
    public class NZORServiceMessage
    {
        public List<string> Urls = new List<string>();
        public string XmlResult = "";
        public string Error = "";

        public NZORServiceMessage()
        {
        }

        public NZORServiceMessage(string url, string xmlResult)
        {
            Urls.Add(url);
            XmlResult = xmlResult;
        }
    }

    public delegate void ProcessingMessage(NZORServiceMessage msg, bool append);
}
