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

        private DataSet GetResultData(String id)
        {
            DataSet ds = null;

            foreach (DataConnection dc in _rep.DataConnections)
            {
                if (dc.DBConnStr != null && dc.DBConnStr.Length > 0)
                {
                    DataSet tmpDs = GetResultData(id, dc.Set);
                    if (ds == null) ds = tmpDs;
                    else if (tmpDs != null) ds.Merge(tmpDs);
                }
            }

            return ds;
        }

        private DataSet GetResultData(String id, String set)
        {
            DataSet ds = null;

            DataConnection dc = _rep.GetDataConnection(set);

            if (dc != null)
            {
                using (OleDbConnection cnn = new OleDbConnection(dc.DBConnStr))
                {
                    cnn.Open();
                    OleDbCommand cmd = cnn.CreateCommand();

                    String sql = "select ";

                    foreach (FieldMapping fm in dc.Mappings)
                    {
                        if (fm.GetType() == typeof(DatabaseMapping))
                        {
                            String fv = fm.GetValueSQL(dc);
                            if (fv != null) sql += fv + ", ";
                        }
                    }
                    sql = sql.Trim();
                    sql = sql.TrimEnd(',');


                    String fromSql = "";
                    dc.RootTable.GetFullSql(ref fromSql);
                    sql += " from " + fromSql;


                    sql += " where ";
                    if (dc.RootTable.Alias != null && dc.RootTable.Alias.Length > 0) sql += dc.RootTable.Alias;
                    else sql += dc.RootTable.Name;
                    sql += "." + dc.RootTable.PK + " = '" + id + "'";

                    cmd.CommandText = sql;

                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    //root table has name of Set
                    ds.Tables[0].TableName = set;
                    cnn.Close();
                }
            }

            return ds;
        }

        private String GetFieldValue(String set, String dbField)
        {
            String val = "";
            if (_results == null || _results.Tables.Count == 0) return "";

            if (_results.Tables[set] == null || _results.Tables[set].Rows.Count == 0) return "";

            DatabaseMapping fm = (DatabaseMapping)_rep.GetDataConnection(set).GetMapping(dbField);
            if (fm != null)
            {
                DataColumn col = _results.Tables[set].Columns[fm.ColumnOrAlias];
                if (col != null)
                {
                    val = _results.Tables[set].Rows[0][col].ToString();
                }
            }

            return val;
        }

        public XElement GetResultXml(String repository, String id, String metadataPrefix)
        {
            if (metadataPrefix == null) throw new OAIException(OAIError.badArgument);

            WebOperationContext ctx = WebOperationContext.Current;
            
            _rep = OAIServer.GetConfig(repository);

            if (_rep == null) throw new OAIException(OAIError.badArgument);

            _results = GetResultData(id);

            //work out what set this id is in
            String set = "";
            foreach (DataTable dt in _results.Tables)
            {
                if (dt.Rows.Count > 0)
                {
                    set = dt.TableName;
                    break;
                }
            }

           
            string xml = File.ReadAllText(Path.Combine(OAIServer.WebDir, "Responses\\GetRecordResponse.xml"));
            xml = xml.Replace(FieldMapping.GET_DATE_TIME, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));

            string url = System.ServiceModel.OperationContext.Current.IncomingMessageHeaders.To.OriginalString;
            if (url.IndexOf("?") != -1) url = url.Substring(0, url.IndexOf("?"));
            xml = xml.Replace(FieldMapping.BASE_URL, url);

            xml = xml.Replace(FieldMapping.IDENTIFIER, id);
            xml = xml.Replace(FieldMapping.METADATA_PREFIX, metadataPrefix);

            try
            {
                xml = xml.Replace(FieldMapping.SET_SPECS, "<setSpec>" + set + "</setSpec>");

                String val = GetFieldValue(set, FieldMapping.RECORD_STATUS);
                String status = "";
                if (val != null && val.Length > 0)
                {
                    xml = xml.Replace(FieldMapping.RECORD_STATUS, "status=\"" + val + "\"");
                    status = val;
                }
                else
                {
                    xml = xml.Replace(FieldMapping.RECORD_STATUS, "");
                }

                val = GetFieldValue(set, FieldMapping.RECORD_DATE);
                if (val != "")
                {
                    DateTime date = DateTime.Parse(val);
                    Utility.ReplaceXmlField(ref xml, FieldMapping.RECORD_DATE, date.ToString("yyyy-MM-dd"));
                }
                else
                {
                    Utility.ReplaceXmlField(ref xml, FieldMapping.RECORD_DATE, "");
                }

                if (set == null || set == "") throw new OAIException(OAIError.idDoesNotExist);

                if (status == "")
                {
                    //Record Metadata
                    String xVal = GetRecordMetadata(metadataPrefix, id);
                    Utility.ReplaceXmlField(ref xml, FieldMapping.RECORD_METADATA, xVal);
                }
                else
                {
                    Utility.ReplaceXmlField(ref xml, "<Metadata>" + FieldMapping.RECORD_METADATA + "</Metadata>", "");
                }

                xml = xml.Replace(FieldMapping.OAI_ERROR, "");
            }
            catch (OAIException ex)
            {
                xml = xml.Replace(FieldMapping.OAI_ERROR, ex.ToString());
                xml = xml.Replace(FieldMapping.RECORD_METADATA, "");
            }

            return XElement.Parse(xml);
        }

        public String GetRecordMetadata(String metadataPrefix, String id)
        {
            String val = null;

            MetadataFormat mf = _rep.GetMetadataFormat(metadataPrefix);

            if (mf == null) throw new OAIException(OAIError.cannotDisseminateFormat);            

            val = mf.ProcessResults(_results, _rep, id);

            return val;
        }
    }
}
