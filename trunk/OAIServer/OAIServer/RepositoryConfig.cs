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
        public String OleDBConnectionString = "";

        public List<MetadataFormat> MetadataFormats = new List<MetadataFormat>();
        public List<String> Sets = new List<String>();
        public MappedTable RootTable = null;
        public List<FieldMapping> Mappings = new List<FieldMapping>();
        

        public void Load(String fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            XmlNode n = doc.SelectSingleNode("//Services/Service");
            this.Name = n.Attributes["name"].InnerText;

            n = doc.SelectSingleNode("//Services/Service/DBConnectionString");
            this.OleDBConnectionString = n.InnerText;

            n = doc.SelectSingleNode("//Services/Service/AdminEmail");
            this.AdminEmail = n.InnerText;


            MetadataFormats.Clear();
            XmlNodeList ml = doc.SelectNodes("//Services/Service/MetadataFormats/MetadataFormat");

            foreach (XmlNode mn in ml)
            {
                MetadataFormat mf = new MetadataFormat();
                mf.Prefix = mn.Attributes["prefix"].InnerText;
                mf.Uri = mn.Attributes["uri"].InnerText;

                MetadataFormats.Add(mf);
            }

            Sets.Clear();
            XmlNodeList nl = doc.SelectNodes("//Services/Service/Sets/Set");

            foreach (XmlNode sn in nl)
            {
                Sets.Add(sn.Attributes["name"].InnerText);
            }

            XmlNode mtn = doc.SelectSingleNode("//Services/Service/Table");
            RootTable = new MappedTable();
            RootTable.Load(mtn, null);


            Mappings.Clear();
            XmlNodeList mpl = doc.SelectNodes("//Services/Service/Mappings/Mapping");

            foreach (XmlNode mpn in mpl)
            {
                FieldMapping fm = null;

                String type = mpn.Attributes["type"].InnerText;
                if (type == "DatabaseMapping")
                {
                    fm = new DatabaseMapping();
                    fm.Load(mpn);
                }
                else if (type == "SQLMaxValueMapping")
                {
                    fm = new SQLMaxValueMapping();
                    fm.Load(mpn);
                }
                else if (type == "FixedValueMapping")
                {
                    fm = new FixedValueMapping();
                    fm.Load(mpn);
                }

                Mappings.Add(fm);
            }            
        }

        public MappedTable GetMappedTable(String id)
        {
            if (RootTable.Id == id) return RootTable;

            return RootTable.GetMappedTable(id);
        }

        public FieldMapping GetMapping(String field)
        {
            FieldMapping fm = null;

            foreach (FieldMapping m in Mappings)
            {
                if (m.Field == field)
                {
                    fm = m;
                    break;
                }
            }

            return fm;
        }

        public Object GetFieldValue(String field)
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
                FieldMapping fm = GetMapping(field);
                if (fm != null)
                {
                    if (fm.GetType() == typeof(FixedValueMapping))
                    {
                        val = fm.GetValue(this);
                    }
                    else if (fm.GetType() == typeof(SQLMaxValueMapping))
                    {
                        val = fm.GetValue(this);
                    }
                }
            }

            return val;
        }
    }
}
