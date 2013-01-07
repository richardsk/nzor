using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Xsl;
using System.Xml.Linq;
using System.Xml;

namespace NZOR.Harvest.Transformers
{
    public class OaiTransformer
    {
        public XDocument Transform(XDocument document, String transformUrl)
        {
            XslCompiledTransform transform = new XslCompiledTransform();

            transform.Load(transformUrl);

            // Apply stylesheet and create a new document.
            StringBuilder transformedXml = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(transformedXml))
            {
                transform.Transform(document.CreateReader(), writer);

                writer.Flush();
            }

            return XDocument.Parse(transformedXml.ToString());
        }
    }
}
