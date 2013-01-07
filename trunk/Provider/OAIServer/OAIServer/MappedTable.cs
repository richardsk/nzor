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
        public String Id = "";
        public String Name = "";
        public String Alias = "";
        public String Type = "";
        public String PK = "";
        public String FKFrom = "";
        public String FKTo = "";
        public String JoinCondition = "";
        public String IndexingElement = "";

        public MappedTable ParentTable = null;

        public List<MappedTable> JoinedTables = new List<MappedTable>();

        public String AliasOrName
        {
            get
            {
                if (Alias != null && Alias.Length > 0) return Alias;
                return this.Name;
            }
        }
            
        public void Load(XmlNode node, MappedTable parent)
        {
            this.Id = node.Attributes["id"].InnerText;
            this.Name = node.Attributes["name"].InnerText;
            this.Alias = node.Attributes["alias"].InnerText;
            this.PK = node.Attributes["pk"].InnerText;
            if (node.Attributes["type"] != null) this.Type = node.Attributes["type"].InnerText;
            if (node.Attributes["fkFrom"] != null) this.FKFrom = node.Attributes["fkFrom"].InnerText;
            if (node.Attributes["fkTo"] != null) this.FKTo = node.Attributes["fkTo"].InnerText;
            if (node.Attributes["joinCondition"] != null) this.JoinCondition = node.Attributes["joinCondition"].InnerText;
            if (node.Attributes["indexingElement"] != null) this.IndexingElement = node.Attributes["indexingElement"].InnerText;

            this.ParentTable = parent;

            JoinedTables.Clear();
            XmlNodeList nl = node.SelectNodes("TableJoin");
            foreach (XmlNode tjn in nl)
            {
                MappedTable mt = new MappedTable();
                mt.Load(tjn, this);
                JoinedTables.Add(mt);
            }
        }

        public MappedTable GetMappedTable(String id)
        {
            if (this.Id == id) return this;

            MappedTable mt = null;
            foreach (MappedTable ct in JoinedTables)
            {
                mt = ct.GetMappedTable(id);
                if (mt != null) break;
            }
            return mt;
        }

        public MappedTable GetMappedTableByPath(String path)
        {
            if (this.IndexingElement.ToLower() == path.ToLower()) return this;

            MappedTable mt = null;
            foreach (MappedTable ct in JoinedTables)
            {
                mt = ct.GetMappedTableByPath(path);
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

            bool joint = false;
            if (sql.Length > 0) joint = true;

            if (joint) sql += this.Type + " join ";            
            sql += this.Name + " ";
            if (this.Alias != "") sql += this.Alias + " ";
            if (joint)
            {
                sql += " on " + this.AliasOrName + "." + this.FKTo + " = " + this.ParentTable.AliasOrName + "." + this.FKFrom + " ";
                if (JoinCondition != null && JoinCondition != "") sql += " and " + JoinCondition + " ";
            }
            foreach (MappedTable mt in JoinedTables)
            {
                mt.GetFullSql(ref sql);
            }
        }
    }
}
