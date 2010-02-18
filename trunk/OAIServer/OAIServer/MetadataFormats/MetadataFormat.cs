using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
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

        protected MetadataFormatMapping _mapping = null;
        protected RepositoryConfig _rep = null;
        protected DataSet _results = null;

        public virtual void Load(XmlNode node)
        {
            Prefix = node.Attributes["prefix"].InnerText;
            Uri = node.Attributes["uri"].InnerText;
            MappingFile = node.Attributes["mappingFile"].InnerText;

            LoadMapping();
        }

        protected virtual void LoadMapping()
        {
            if (_mapping == null)
            {
                _mapping = new MetadataFormatMapping();

                XmlDocument md = new XmlDocument();
                md.Load(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Config\\" + MappingFile));

                XmlNode node = md.SelectSingleNode("//MetadataFormatMapping");
                _mapping.Load(node);
            }
        }

        public virtual String ProcessResults(DataSet results, RepositoryConfig rep)
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
            _results = results;

            LoadMapping();

            //XmlDocument doc = new XmlDocument();

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(s);
            Xml.XmlSampleGenerator genr = new Xml.XmlSampleGenerator(schemas, new XmlQualifiedName("DataSet"));
            genr.AutoGenerateValues = false;
            //genr.IndexingElement = IndexingElement;

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
                SchemaMapping sm = _mapping.GetMapping(path);
                if (sm != null)
                {
                    String val = GetFieldValue(sm.Set, sm.Field);
                    if (val != null) el.FixedValue = val.ToString();
                }
            }
        }

        internal void AttributeAdded(Xml.InstanceAttribute attr, String path)
        {
            if (attr != null)
            {
                SchemaMapping sm = _mapping.GetMapping(path);
                if (sm != null)
                {
                    String val = GetFieldValue(sm.Set, sm.Field);
                    if (val != null) attr.FixedValue = val.ToString();
                }
            }
        }

        protected String GetFieldValue(String set, String dbField)
        {
            String val = "";
            if (_results == null || _results.Tables.Count == 0) return "";

            if (_results.Tables[set] == null || _results.Tables[set].Rows.Count == 0) return "";

            DatabaseMapping fm = (DatabaseMapping)_rep.GetDataConnection(set).GetMapping(dbField);
            DataColumn col = _results.Tables[set].Columns[fm.ColumnOrAlias];
            if (col != null)
            {
                val = _results.Tables[set].Rows[0][col].ToString();
            }

            return val;
        }


        public void XsdValEvent(object sender, ValidationEventArgs e)
        {
            String x = e.Message;
        }
    }
}
