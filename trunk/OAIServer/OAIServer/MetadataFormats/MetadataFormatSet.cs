using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OAIServer
{
    public class MetadataFormatSet
    {
        public String Name = "";
        public String IndexingElement = "";

        public void Load(XmlNode node)
        {
            Name = node.Attributes["name"].InnerText;
            IndexingElement = node.Attributes["indexingElement"].InnerText;
        }
    }
}
