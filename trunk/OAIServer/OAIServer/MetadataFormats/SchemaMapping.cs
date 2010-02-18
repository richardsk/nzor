using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OAIServer
{
    public class SchemaMapping
    {
        public String Set = "";
        public String Field = "";
        public String XPath = "";

        public void Load(XmlNode node)
        {
            Set = node.Attributes["set"].InnerText;
            Field = node.Attributes["field"].InnerText;
            XPath = node.Attributes["xpath"].InnerText;
        }
    }
}
