using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OAIServer
{
    public class RepositoryConfig
    {
        public String Name = "";
        public String AdminEmail = "";

        public List<MetadataFormat> MetadataFormats = new List<MetadataFormat>();
        public List<String> Sets = new List<String>();
        public List<DataConnection> DataConnections = new List<DataConnection>();
        

        public void Load(String fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            XmlNode n = doc.SelectSingleNode("//Services/Service");
            this.Name = n.Attributes["name"].InnerText;

            n = doc.SelectSingleNode("//Services/Service/AdminEmail");
            this.AdminEmail = n.InnerText;


            MetadataFormats.Clear();
            XmlNodeList ml = doc.SelectNodes("//Services/Service/MetadataFormats/MetadataFormat");

            foreach (XmlNode mn in ml)
            {
                MetadataFormat mf = new MetadataFormat();
                mf.Load(mn);

                MetadataFormats.Add(mf);
            }

            Sets.Clear();
            XmlNodeList nl = doc.SelectNodes("//Services/Service/Sets/Set");

            foreach (XmlNode sn in nl)
            {
                Sets.Add(sn.Attributes["name"].InnerText);
            }

            XmlNodeList dcl = doc.SelectNodes("//Services/Service/DataConnections/DataConnection");
            foreach (XmlNode dcn in dcl)
            {
                DataConnection dc = new DataConnection();
                dc.Load(dcn);
                DataConnections.Add(dc);
            }
    
        }

        public DataConnection GetDataConnection(String set)
        {
            DataConnection dc = null;

            foreach (DataConnection d in DataConnections)
            {
                if (d.Set == set)
                {
                    dc = d;
                    break;
                }
            }

            return dc;
        }

        public MetadataFormat GetMetadataFormat(String prefix)
        {
            MetadataFormat mf = null;

            foreach (MetadataFormat m in MetadataFormats)
            {
                if (m.Prefix == prefix)
                {
                    mf = m;
                    break;
                }
            }

            return mf;
        }

        public Object GetFieldValue(String field, DataConnection dc)
        {
            Object val = null;

            if (field == FieldMapping.ADMIN_EMAIL)
            {
                val = AdminEmail;
            }
            else if (field == FieldMapping.REPOSITORY_NAME)
            {
                val = Name;
            }
            else 
            {
                FieldMapping fm = dc.GetMapping(field);
                if (fm != null)
                {
                    if (fm.GetType() == typeof(FixedValueMapping))
                    {
                        val = fm.GetValue(dc);
                    }
                    else if (fm.GetType() == typeof(SQLMaxValueMapping))
                    {
                        val = fm.GetValue(dc);
                    }
                }
            }

            return val;
        }
    }
}
