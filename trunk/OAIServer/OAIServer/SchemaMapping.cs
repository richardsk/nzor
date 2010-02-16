using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OAIServer
{
    public class SchemaMapping
    {
        public String Name = "";
        public String XPath = "";

        public void Load(XmlNode node)
        {
            Name = node.Attributes["name"].InnerText;
            XPath = node.Attributes["xpath"].InnerText;
        }
    }
}
