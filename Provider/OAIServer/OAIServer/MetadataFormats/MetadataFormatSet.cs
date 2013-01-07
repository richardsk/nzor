using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OAIServer
{
    public class MetadataFormatSet
    {
        public static String ALL_SET = "All";

        public String Name = "";
        public String IndexingElement = "";

        public void Load(XmlNode node)
        {
            Name = node.Attributes["name"].InnerText;
            IndexingElement = node.Attributes["indexingElement"].InnerText;
        }
    }
}
