using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Data;
using System.Net;
using System.Web;
using System.IO;
using System.Reflection;

namespace OAIServer
{
    public class MetadataFormat
    {
        public String Prefix = "";
        public String Uri = "";
        public String MappingFile = "";
        public String TargetNamespace = "";

        private List<SchemaMapping> _mappings = null;
        private RepositoryConfig _rep = null;

        public void Load(XmlNode node)
        {
            Prefix = node.Attributes["prefix"].InnerText;
            Uri = node.Attributes["uri"].InnerText;
            MappingFile = node.Attributes["mappingFile"].InnerText;
            TargetNamespace = node.Attributes["targetNamespace"].InnerText;
        }

        protected void LoadMappings()
        {
            if (_mappings == null)
            {
                _mappings = new List<SchemaMapping>();

                XmlDocument md = new XmlDocument();
                md.Load(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Config\\" + MappingFile));

                XmlNodeList nodes = md.SelectNodes("//Mappings/Mapping");
                foreach (XmlNode node in nodes)
                {
                    SchemaMapping sm = new SchemaMapping();
                    sm.Load(node);
                    _mappings.Add(sm);
                }
            }
        }

        protected SchemaMapping GetMapping(String xPath)
        {
            SchemaMapping sm = null;
            foreach (SchemaMapping m in _mappings)
            {
                if (m.XPath == xPath)
                {
                    sm = m;
                    break;
                }
            }
            return sm;
        }

        public String ProcessResults(DataSet results, RepositoryConfig rep)
        {
            XmlSchema s = null;

            if (Uri.IndexOf("://") != -1)
            {
                WebRequest req = HttpWebRequest.Create(Uri);
                WebResponse resp = req.GetResponse();
                
                s = XmlSchema.Read(resp.GetResponseStream(), new ValidationEventHandler(this.XsdValEvent));

                resp.Close();
            }
            else
            {
                //file based
                s = XmlSchema.Read(new StreamReader(Uri), new ValidationEventHandler(this.XsdValEvent));
            }

            _rep = rep;
            LoadMappings();

            //XmlDocument doc = new XmlDocument();

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(s);
            Xml.XmlSampleGenerator genr = new Xml.XmlSampleGenerator(schemas, new XmlQualifiedName("DataSet", TargetNamespace));
            genr.AutoGenerateValues = false;

            genr.ElementAdded += new Xml.XmlSampleGenerator.ElementAddedHanlder(this.ElementAdded);
            genr.AttributeAdded += new Xml.XmlSampleGenerator.AttributeAddedHanlder(this.AttributeAdded);

            MemoryStream ms = new MemoryStream();
            XmlTextWriter wr = new XmlTextWriter(ms, Encoding.UTF8);

            genr.WriteXml(wr);
            
            ms.Position = 0;
            StreamReader rdr = new StreamReader(ms);
            String xml = rdr.ReadToEnd();

            return xml;

            //ProcessElement("//", s.Items[0], doc, null);
            
            //return doc.OuterXml;
        }

        internal void ElementAdded(Xml.InstanceElement el, String path)
        {
            if (el != null)
            {
                SchemaMapping sm = GetMapping(path);
                if (sm != null)
                {
                    Object val = _rep.GetFieldValue(sm.Name);
                    if (val != null) el.FixedValue = val.ToString();
                }
            }
        }

        internal void AttributeAdded(Xml.InstanceAttribute attr, String path)
        {
            if (attr != null)
            {
                SchemaMapping sm = GetMapping(path);
                if (sm != null)
                {
                    Object val = _rep.GetFieldValue(sm.Name);
                    if (val != null) attr.FixedValue = val.ToString();
                }
            }
        }


        protected void ProcessElement(String path, XmlSchemaObject el, XmlDocument doc, XmlElement currentNode)
        {
            if (el.GetType() == typeof(XmlSchemaElement))
            {
                XmlSchemaElement sel = (XmlSchemaElement)el;
                XmlElement newNode = doc.CreateElement(sel.Name);
                if (currentNode == null)
                {
                    doc.AppendChild(newNode);
                    currentNode = newNode;
                }
                else
                {
                    currentNode.AppendChild(newNode);
                }

                path += newNode.Name;

                SchemaMapping sm = GetMapping(path);
                if (sm != null)
                {
                    Object val = _rep.GetFieldValue(sm.Name);
                    if (val != null) newNode.Value = val.ToString();
                }
            }

            foreach (PropertyInfo property in el.GetType().GetProperties())
            {
                if (property.PropertyType.FullName == "System.Xml.Schema.XmlSchemaObjectCollection")
                {
                    XmlSchemaObjectCollection childObjectCollection = (XmlSchemaObjectCollection)property.GetValue(el, null);

                    foreach (XmlSchemaObject schemaObject in childObjectCollection)
                    {
                        ProcessElement(path, schemaObject, doc, currentNode);
                    }
                }
            }
        }

        public void XsdValEvent(object sender, ValidationEventArgs e)
        {
            String x = e.Message;
        }
    }
}
