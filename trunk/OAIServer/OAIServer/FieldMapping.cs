using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Data.OleDb;

namespace OAIServer
{
    public enum MappingType
    {
        DatabaseMapping,
        FixedValueMapping
    }

    public class FieldMapping
    {
        public static string GET_DATE = "[GET_DATE]";
        public static string BASE_URL = "[BASE_URL]";
        public static string ADMIN_EMAIL = "[ADMIN_EMAIL]";
        public static string EARLIEST_DATE = "[EARLIEST_DATE]";
        public static string REPOSITORY_NAME = "[REPOSITORY_NAME]";
        public static string IDENTIFIER = "[IDENTIFIER]";
        public static string METADATA_PREFIX = "[METADATA_PREFIX]";
        public static string RECORD_STATUS = "[RECORD_STATUS]";
        public static string RECORD_DATE = "[RECORD_DATE]";
        public static string SET_SPECS = "[SET_SPECS]";
        public static string RECORD_METADATA = "[RECORD_METADATA]";
        public static string RECORDS = "[RECORDS]";
        public static string FROM_DATE = "[FROM_DATE]";
        public static string TO_DATE = "[TO_DATE]";
        public static string SET = "[SET]";
        public static string EXP_DATE = "[EXP_DATE]";
        public static string LIST_SIZE = "[LIST_SIZE]";
        public static string CURSOR = "[CURSOR]";
        public static string RESUMPTION_TOKEN = "[RESUMPTION_TOKEN]";
        public static string TOKEN = "[TOKEN]";
        public static string RESUMPTION_COLUMN = "[RESUMPTION_COLUMN]";
        public static string OAI_ERROR = "[OAI_ERROR]";


        protected String _field = "";

        public String Field
        {
            get 
            { 
                return _field;
            }
            set
            {
                _field = value;
            }
        }

        public override string ToString()
        {
            return _field;
        }

        public virtual void Load(XmlNode node)
        {
            this.Field = node.Attributes["field"].InnerText;
        }

        public virtual String GetValueSQL(DataConnection dc)
        {
            return "";
        }

        public virtual Object GetValue(DataConnection dc)
        {
            return null;
        }
    }

    public class DatabaseMapping : FieldMapping
    {
        public String TableId = "";
        public String Column = "";
        public String ColumnAlias = "";
        public String SQL = "";

        public override void Load(XmlNode node)
        {
            base.Load(node);
            this.TableId = node.Attributes["table"].InnerText;
            this.Column = node.Attributes["column"].InnerText;
            this.ColumnAlias = node.Attributes["columnAlias"].InnerText;
            this.SQL = node.Attributes["sql"].InnerText;
        }

        public override String GetValueSQL(DataConnection dc)
        {
            String val = null;

            if (this.ColumnOrAlias == "" && (SQL == null || SQL.Length == 0)) return null;

            if (SQL != null && SQL.Length > 0)
            {
                val = SQL;
                if (this.ColumnAlias != "") val += " " + ColumnAlias;
            }
            else 
            {
                val = dc.GetMappedTable(TableId).AliasOrName + "." + Column;
                if (this.ColumnAlias != "") val += " " + ColumnAlias;
            }

            return val;
        }

        public String ColumnOrAlias
        {
            get
            {
                if (ColumnAlias != null && ColumnAlias != "") return ColumnAlias;
                return Column;
            }
        }

        public override Object GetValue(DataConnection dc)
        {
            throw new Exception("Cannot get individual value of a DB Mapping");
        }
    }

    public class FixedValueMapping : FieldMapping
    {
        public Object Value;

        public override void Load(XmlNode node)
        {
            base.Load(node);
        }

        public override String GetValueSQL(DataConnection dc)
        {
            return "'" + Value.ToString() + "'";
        }

        public override Object GetValue(DataConnection dc)
        {
            return Value;
        }
    }

    public class SQLMaxValueMapping : DatabaseMapping
    {
        public override Object GetValue(DataConnection dc)
        {
            Object val = null;

            using (OleDbConnection cnn = new OleDbConnection(dc.DBConnStr))
            {
                cnn.Open();
                OleDbCommand cmd = cnn.CreateCommand();
                if (SQL != null && SQL.Length > 0)
                {
                    cmd.CommandText = SQL;
                }
                else
                {
                    cmd.CommandText = "select max(" + Column + ") from " + dc.GetMappedTable(TableId).Name;
                }

                val = cmd.ExecuteScalar();
                cnn.Close();
            }

            return val;
        }
    }

    public class SQLMinValueMapping : DatabaseMapping
    {
        public override Object GetValue(DataConnection dc)
        {
            Object val = null;

            using (OleDbConnection cnn = new OleDbConnection(dc.DBConnStr))
            {
                cnn.Open();
                OleDbCommand cmd = cnn.CreateCommand();
                if (SQL != null && SQL.Length > 0)
                {
                    cmd.CommandText = SQL;
                }
                else
                {
                    cmd.CommandText = "select min(" + Column + ") from " + dc.GetMappedTable(TableId).Name;
                }

                val = cmd.ExecuteScalar();
                cnn.Close();
            }

            return val;
        }
    }

}
