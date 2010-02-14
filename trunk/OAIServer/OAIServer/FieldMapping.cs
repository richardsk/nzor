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


        protected String _field = "";
        protected String _fieldKey = "";

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

        public virtual String FieldKey()
        {
            return _fieldKey;
        }

        public override string ToString()
        {
            return _field;
        }

        public virtual void Load(XmlNode node)
        {
            this.Field = node.Attributes["field"].InnerText;
        }

        public virtual String GetValueSQL()
        {
            return "";
        }

        public virtual Object GetValue()
        {
            return null;
        }
    }

    public class DatabaseMapping : FieldMapping
    {
        public String Table = "";
        public String TableAlias = "";
        public String Column = "";
        public String SQL = "";

        public override void Load(XmlNode node)
        {
            base.Load(node);
            this.Table = node.Attributes["table"].InnerText;
            this.TableAlias = node.Attributes["tableAlias"].InnerText;
            this.Column = node.Attributes["column"].InnerText;
            this.SQL = node.Attributes["sql"].InnerText;
            if (this.SQL.Length > 0) this._fieldKey = Utility.NextColumnKey();
        }

        public override String GetValueSQL()
        {
            String val = null;

            if (SQL != null && SQL.Length > 0)
            {
                val = SQL;
            }
            else if (TableAlias != null && TableAlias.Length > 0)
            {
                val = TableAlias + "." + Column;
                if (this._fieldKey != "") val += " " + _fieldKey;
            }
            else
            {
                val = Table + "." + Column;
                if (this._fieldKey != "") val += " " + _fieldKey;
            }

            return val;
        }

        public override String FieldKey()
        {
            if (this._fieldKey != "") return _fieldKey;
            return GetValueSQL();
        }

        public override Object GetValue()
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

        public override String GetValueSQL()
        {
            return "'" + Value.ToString() + "'";
        }

        public override Object GetValue()
        {
            return Value;
        }
    }

    public class SQLMaxValueMapping : FieldMapping
    {
        public String Table = "";
        public String TableAlias = "";
        public String Column = "";
        public String SQL = "";
        public String DBCnnStr = "";

        public override void Load(XmlNode node)
        {
            base.Load(node);
            this.Table = node.Attributes["table"].InnerText;
            this.TableAlias = node.Attributes["tableAlias"].InnerText;
            this.Column = node.Attributes["column"].InnerText;
            this.SQL = node.Attributes["sql"].InnerText;
            if (this.SQL.Length > 0) this._fieldKey = Utility.NextColumnKey();
        }

        public override String GetValueSQL()
        {
            String val = "";

            if (SQL != null && SQL.Length > 0)
            {
                val = SQL;
            }
            else if (TableAlias != null && TableAlias.Length > 0)
            {
                val = TableAlias + "." + Column;
                if (this._fieldKey != "") val += " " + _fieldKey;
            }
            else
            {
                val = Table + "." + Column;
                if (this._fieldKey != "") val += " " + _fieldKey;
            }

            return val;
        }

        public override Object GetValue()
        {
            Object val = null;

            using (OleDbConnection cnn = new OleDbConnection(DBCnnStr))
            {
                cnn.Open();
                OleDbCommand cmd = cnn.CreateCommand();
                if (SQL != null && SQL.Length > 0)
                {
                    cmd.CommandText = SQL;
                }
                else
                {
                    cmd.CommandText = "select max(" + Column + ") from " + Table;
                }

                val = cmd.ExecuteScalar();
                cnn.Close();
            }

            return val;
        }
    }
}
