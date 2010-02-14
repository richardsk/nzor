using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ServiceModel.Web;
using System.IO;
using System.Web;
using System.Data;
using System.Data.OleDb;

namespace OAIServer
{
    public class GetRecordRequest
    {
        DataSet _results = null;
        RepositoryConfig _rep = null;

        private void GetResultData(String id)
        {
            _results = null;
            
            using (OleDbConnection cnn = new OleDbConnection(_rep.OleDBConnectionString))
            {
                cnn.Open();
                OleDbCommand cmd = cnn.CreateCommand();

                String sql = "select ";

                foreach (FieldMapping fm in _rep.Mappings)
                {
                    if (fm.GetType() == typeof(DatabaseMapping))
                    {
                        sql += fm.GetValueSQL() + ", ";
                    }
                }
                sql = sql.Trim();
                sql = sql.TrimEnd(',');

                
                String fromSql = "";
                _rep.RootTable.GetFullSql(ref fromSql);
                sql += " from " + fromSql;


                sql += " where ";
                if (_rep.RootTable.Alias != null && _rep.RootTable.Alias.Length > 0) sql += _rep.RootTable.Alias;
                else sql += _rep.RootTable.Name;
                sql += "." + _rep.RootTable.PK + " = '" + id + "'";

                cmd.CommandText = sql;

                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                _results = new DataSet();
                da.Fill(_results);
                
                cnn.Close();
            }
        }

        private String GetFieldValue(String field)
        {
            String val = "";
            if (_results == null || _results.Tables.Count == 0) return "";

            FieldMapping fm = _rep.GetMapping(field);
            DataColumn col = _results.Tables[0].Columns[fm.FieldKey()];
            if (col != null)
            {
                val = _results.Tables[0].Rows[0][col].ToString();
            }

            return val;
        }

        public XElement GetResultXml(String repository, String id, String metadataPrefix)
        {
            WebOperationContext ctx = WebOperationContext.Current;
            
            _rep = OAIServer.GetConfig(repository);

            GetResultData(id);

            string xml = File.ReadAllText(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Responses\\GetRecordResponse.xml"));
            xml = xml.Replace(FieldMapping.GET_DATE, DateTime.Now.ToString());

            string url = HttpContext.Current.Request.Url.AbsoluteUri;
            if (url.IndexOf("?") != -1) url = url.Substring(0, url.IndexOf("?"));
            xml = xml.Replace(FieldMapping.BASE_URL, url);

            xml = xml.Replace(FieldMapping.IDENTIFIER, id);
            xml = xml.Replace(FieldMapping.METADATA_PREFIX, metadataPrefix);

            String val = GetFieldValue(FieldMapping.RECORD_STATUS);
            xml = xml.Replace(FieldMapping.RECORD_STATUS, "status=\"" + val + "\"");
            
            val = GetFieldValue(FieldMapping.RECORD_DATE);
            Utility.ReplaceXmlField(ref xml, FieldMapping.RECORD_DATE, val);

            //SETs
            XElement xVal = GetSetsXml(repository);
            Utility.ReplaceXmlField(ref xml, FieldMapping.SET_SPECS, xVal);

            //Record Metadata
            xVal = GetRecordMetadata(repository, id, metadataPrefix);
            Utility.ReplaceXmlField(ref xml, FieldMapping.RECORD_METADATA, xVal);

            return XElement.Parse(xml);
        }

        public XElement GetSetsXml(String repository)
        {
            XElement val = null;

            return val;
        }

        public XElement GetRecordMetadata(String repository, String recordId, String metadataPrefix)
        {
            XElement val = null;

            return val;
        }
    }
}
