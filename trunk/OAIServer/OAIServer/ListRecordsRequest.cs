using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.ServiceModel.Web;
using System.Xml.Linq;
using System.IO;
using System.Web;

namespace OAIServer
{
    public class ListRecordsRequest
    {
        private int _maxResults = 1000;

        DataSet _results = null;
        RepositoryConfig _rep = null;
        String _requestedResumptionToken = "";

        private DataSet GetResultData(OAIRequestSession req)
        {
            //all sets
            DataSet ds = null;

            foreach (DataConnection dc in _rep.DataConnections)
            {
                req.NumRecords = 0;

                DataSet tmpDs = GetResultData(dc.Set, req.FromDate, req.ToDate, req);
                if (ds == null)
                {
                    ds = tmpDs;
                    if (tmpDs != null) req.NumRecords += tmpDs.Tables[0].Rows.Count;
                }
                else if (tmpDs != null)
                {
                    ds.Merge(tmpDs);
                    req.NumRecords += tmpDs.Tables[0].Rows.Count;
                }
            }

            return ds;
        }
        
        private DataSet GetResultData(String set, String fromDate, String toDate, OAIRequestSession req)
        {
            _maxResults = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxRecordsReturned"]);

            DataSet ds = null;

            DataConnection dc = _rep.GetDataConnection(set);

            if (dc != null)
            {
                using (OleDbConnection cnn = new OleDbConnection(dc.DBConnStr))
                {
                    cnn.Open();
                    OleDbCommand cmd = cnn.CreateCommand();

                    String sql = "select top " + (_maxResults + 1).ToString() + " ";

                    FieldMapping dtMapping = null;

                    foreach (FieldMapping fm in dc.Mappings)
                    {
                        if (fm.GetType() == typeof(DatabaseMapping))
                        {
                            String fv = fm.GetValueSQL(dc);
                            if (fv != null) sql += fv + ", ";
                        }

                        if (fm.Field == FieldMapping.RECORD_DATE)
                        {
                            dtMapping = fm;
                        }
                    }

                    sql = sql.Trim();
                    sql = sql.TrimEnd(',');


                    String fromSql = "";
                    dc.RootTable.GetFullSql(ref fromSql);
                    sql += " from " + fromSql;

                    if (dtMapping != null && fromDate != null && fromDate != "")
                    {
                        sql += " where ";
                        sql += dtMapping.GetValueSQL(dc) + " >= '" + fromDate.ToString() + "' ";
                        if (toDate != null && toDate != "")
                        {
                            sql += "and ";
                            sql += dtMapping.GetValueSQL(dc) + " <= '" + toDate.ToString() + "' ";
                        }
                    }

                    String idCol = "";   
                    String idColName = "";
                    String orderby = "";
                    DatabaseMapping rcm = (DatabaseMapping)dc.GetMapping(FieldMapping.RESUMPTION_COLUMN);
                    if (rcm == null)
                    {
                        DatabaseMapping idm = (DatabaseMapping)dc.GetMapping(FieldMapping.IDENTIFIER);
                        idCol = idm.GetValueSQL(dc);
                        idColName = idm.ColumnOrAlias;
                        if (idm.OrderBy != null && idm.OrderBy != "") orderby = " order by " + dc.GetMappedTable(idm.TableId).AliasOrName + "." + idm.OrderBy;
                    }
                    else
                    {
                        idCol = rcm.GetValueSQL(dc);
                        idColName = rcm.ColumnOrAlias;
                    }

                    String pos = "";
                    if (req.NextRecordPositions[dc.Set] != null) pos = req.NextRecordPositions[dc.Set].ToString();

                    if (pos != null && pos != "")
                    {
                        sql += " and " + idCol + " > '" + pos + "'";
                    }
                    else
                    {
                        if (_requestedResumptionToken != null && _requestedResumptionToken != "") return null; //no records for this set in this iteration
                    }

                    sql += orderby;
                                        
                    cmd.CommandText = sql;

                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    int cnt = ds.Tables[0].Rows.Count;
                    if (cnt > _maxResults)
                    {
                        req.NextRecordPositions[set] = ds.Tables[0].Rows[cnt - 2][idColName].ToString();
                        ds.Tables[0].Rows[cnt - 1].Delete();
                        ds.AcceptChanges();
                    }
                    else
                    {
                        req.NextRecordPositions[set] = "";
                    }

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
            DataColumn col = _results.Tables[set].Columns[fm.ColumnOrAlias];
            if (col != null)
            {
                val = _results.Tables[set].Rows[0][col].ToString();
            }

            return val;
        }

        public XElement GetResultXml(String repository, String metadataPrefix, String set, String fromDate, String toDate, String resumptionToken)
        {
            WebOperationContext ctx = WebOperationContext.Current;
            
            _rep = OAIServer.GetConfig(repository);
            
            _requestedResumptionToken = resumptionToken;


            string xml = File.ReadAllText(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Responses\\ListRecordsResponse.xml"));

            if (fromDate != null && fromDate.Length > 0) xml = xml.Replace(FieldMapping.FROM_DATE, "from=\"" + fromDate + "\"");
            else xml = xml.Replace(FieldMapping.FROM_DATE, "");

            if (toDate != null && toDate.Length > 0) xml = xml.Replace(FieldMapping.TO_DATE, "until=\"" + toDate + "\"");
            else xml = xml.Replace(FieldMapping.TO_DATE, "");

            if (set != null && set.Length > 0) xml = xml.Replace(FieldMapping.SET, "set=\"" + set + "\"");
            else xml = xml.Replace(FieldMapping.SET, "");

            string url = HttpContext.Current.Request.Url.AbsoluteUri;
            if (url.IndexOf("?") != -1) url = url.Substring(0, url.IndexOf("?"));
            xml = xml.Replace(FieldMapping.BASE_URL, url);

            xml = xml.Replace(FieldMapping.METADATA_PREFIX, metadataPrefix);

            try
            {
                OAIRequestSession req = GetResumptionSession(resumptionToken, fromDate, toDate, metadataPrefix, repository, set);

                _results = GetResultData(req);

                bool moreRecords = false;                
                foreach (String nextRec in req.NextRecordPositions.Values)
                {
                    if (nextRec != null && nextRec != "")
                    {
                        moreRecords = true;
                    }
                }

                string resxml = "";
                if (!moreRecords)
                {
                    resxml = "<resumptionToken/>";
                }
                else
                {
                    resxml = File.ReadAllText(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Responses\\ResumptionSnippet.xml"));
                    resxml = resxml.Replace(FieldMapping.EXP_DATE, req.ResumptionExpiry.ToString("s"));
                    resxml = resxml.Replace(FieldMapping.LIST_SIZE, req.NumRecords.ToString());
                    resxml = resxml.Replace(FieldMapping.CURSOR, req.Cursor.ToString());
                    resxml = resxml.Replace(FieldMapping.TOKEN, req.ResumptionToken);
                }
                req.Cursor += req.NumRecords; //ready for next time

                if (resxml != "")
                {
                    xml = xml.Replace(FieldMapping.RESUMPTION_TOKEN, resxml);
                }
                else
                {
                    xml = xml.Replace(FieldMapping.RESUMPTION_TOKEN, "");
                }

                String records = "";

                foreach (DataTable resultTable in _results.Tables)
                {
                    if (resultTable.Rows.Count == 0) continue;

                    DataConnection dc = _rep.GetDataConnection(resultTable.TableName);
                    DatabaseMapping idField = (DatabaseMapping)dc.GetMapping(FieldMapping.IDENTIFIER);

                    foreach (DataRow row in resultTable.Rows)
                    {
                        string recordXml = File.ReadAllText(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Responses\\RecordSnippet.xml"));


                        String status = GetFieldValue(set, FieldMapping.RECORD_STATUS);
                        if (status != null && status.Length > 0)
                        {
                            recordXml = recordXml.Replace(FieldMapping.RECORD_STATUS, "status=\"" + status + "\"");
                        }
                        else
                        {
                            recordXml = recordXml.Replace(FieldMapping.RECORD_STATUS, "");
                        }

                        recordXml = recordXml.Replace(FieldMapping.IDENTIFIER, row[idField.ColumnOrAlias].ToString());

                        String val = GetFieldValue(set, FieldMapping.RECORD_DATE);
                        if (val != "")
                        {
                            DateTime date = DateTime.Parse(val);
                            Utility.ReplaceXmlField(ref recordXml, FieldMapping.RECORD_DATE, date.ToString("s"));
                        }
                        else
                        {
                            Utility.ReplaceXmlField(ref recordXml, FieldMapping.RECORD_DATE, "");
                        }

                        Utility.ReplaceXmlField(ref recordXml, FieldMapping.SET_SPECS, resultTable.TableName);

                        //Record Metadata
                        if (status == null || status == "")
                        {
                            String xVal = GetRecordMetadata(metadataPrefix, row[idField.ColumnOrAlias].ToString());
                            Utility.ReplaceXmlField(ref recordXml, FieldMapping.RECORD_METADATA, xVal);
                        }

                        records += recordXml + Environment.NewLine;
                    }
                }

                xml = xml.Replace(FieldMapping.RECORDS, records);
            }
            catch (OAIException oaiex)
            {
                xml = xml.Replace(FieldMapping.RESUMPTION_TOKEN, "");
                xml = xml.Replace(FieldMapping.RECORDS, oaiex.ToString());
            }

            return XElement.Parse(xml);
        }

        public OAIRequestSession GetResumptionSession(String resumptionToken, String fromDate, String toDate, String metadataPrefix, String repository, String set)
        {
            OAIRequestSession req = null;

            if (resumptionToken != null && resumptionToken.Length > 0)
            {
                req = (OAIRequestSession)HttpContext.Current.Session[resumptionToken];

                if (req == null) throw new OAIException(OAIError.badResumptionToken);
            }
            else
            {
                req = new OAIRequestSession();
                req.CallerIP = HttpContext.Current.Request.UserHostName;
                req.CallDate = DateTime.Now;
                req.FromDate = fromDate;
                req.ToDate = toDate;
                req.MetadataPrefix = metadataPrefix;
                req.Repository = repository;
                req.Set = set;
                req.ResumptionToken = Guid.NewGuid().ToString();

                HttpContext.Current.Session[req.ResumptionToken] = req;
            }

            int hours = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TokenExpirationHours"]);
            req.ResumptionExpiry = DateTime.Now.AddHours(hours);
            HttpContext.Current.Session.Timeout = hours * 60;

            return req;
        }

        public String GetRecordMetadata(String metadataPrefix, String id)
        {
            String val = null;

            MetadataFormat mf = _rep.GetMetadataFormat(metadataPrefix);
            val = mf.ProcessResults(_results, _rep, id);

            return val;
        }
    }
}
