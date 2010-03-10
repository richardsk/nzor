using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OAIServer
{
    public class DataConnection
    {
        public String DBConnStr = "";
        public String Set = "";

        public RepositoryConfig Repository = null;
        public MappedTable RootTable = null;
        public List<FieldMapping> Mappings = new List<FieldMapping>();

        public void Load(XmlNode node)
        {
            Set = node.Attributes["set"].InnerText;

            XmlNode el = node.SelectSingleNode("DBConnectionString");
            if (el != null) DBConnStr = el.InnerText;

            el = node.SelectSingleNode("Table");
            if (el != null)
            {
                RootTable = new MappedTable();
                RootTable.Load(el, null);
            }

            Mappings.Clear();
            XmlNodeList mpl = node.SelectNodes("Mappings/Mapping");

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
                else if (type == "SQLMinValueMapping")
                {
                    fm = new SQLMinValueMapping();
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
    }
}
