using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace OAIServer
{
    public class MappedTable
    {
        public String Name = "";
        public String Alias = "";
        public String Type = "";
        public String PK = "";
        public String FKFrom = "";
        public String FKTo = "";

        public List<MappedTable> JoinedTables = new List<MappedTable>();

        public void Load(XmlNode node)
        {
            this.Name = node.Attributes["name"].InnerText;
            this.Alias = node.Attributes["alias"].InnerText;
            this.PK = node.Attributes["pk"].InnerText;
            if (node.Attributes["type"] != null) this.Type = node.Attributes["type"].InnerText;
            if (node.Attributes["fkFrom"] != null) this.FKFrom = node.Attributes["fkFrom"].InnerText;
            if (node.Attributes["fkTo"] != null) this.FKTo = node.Attributes["fkTo"].InnerText;

            JoinedTables.Clear();
            XmlNodeList nl = node.SelectNodes("TableJoin");
            foreach (XmlNode tjn in nl)
            {
                MappedTable mt = new MappedTable();
                mt.Load(tjn);
                JoinedTables.Add(mt);
            }
        }

        public MappedTable GetMappedTable(String name, String alias)
        {
            if (this.Name == name && this.Alias == alias) return this;

            MappedTable mt = null;
            foreach (MappedTable ct in JoinedTables)
            {
                mt = ct.GetMappedTable(name, alias);
                if (mt != null) break;
            }
            return mt;
        }

        public String GetSql()
        {
            String sql = this.Name;
            if (this.Alias != null && this.Alias.Length > 0) sql += " " + this.Alias;
            return sql;
        }

        public void GetFullSql(ref String sql)
        {
            if (sql == null) sql = "";
            if (sql.Length > 0) sql += this.Type + " join ";
            on
            sql += this.Name + " ";
            if (this.Alias != "") sql += this.Alias + " ";

            foreach (MappedTable mt in JoinedTables)
            {
                mt.GetFullSql(ref sql);
            }
        }
    }
}
