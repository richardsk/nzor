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
    public class ListIdentifiersRequest
    {
        private int _maxResults = 1000;

        DataSet _results = null;
        RepositoryConfig _rep = null;
        String _requestedResumptionToken = "";
        OAIRequestSession _session = null;

        public ListIdentifiersRequest()
        {
        }

        public ListIdentifiersRequest(OAIRequestSession session)
        {
            _session = session;
        }

        private DataSet GetResultData()
        {
            //all sets
            DataSet ds = null;

            foreach (DataConnection dc in _rep.DataConnections)
            {
                if (dc.DBConnStr != null && dc.DBConnStr.Length > 0)
                {
                    DataSet tmpDs = GetResultData(dc.Set, _session.FromDate, _session.ToDate);
                    if (ds == null)
                    {
                        ds = tmpDs;
                    }
                    else if (tmpDs != null)
                    {
                        ds.Merge(tmpDs);
                    }
                }
            }

            return ds;
        }

        private DataSet GetResultData(String set, String fromDate, String toDate)
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
                        if (rcm.OrderBy != null && rcm.OrderBy != "") orderby = " order by " + dc.GetMappedTable(rcm.TableId).AliasOrName + "." + rcm.OrderBy;
                    }

                    String sql = "select top " + (_maxResults + 1).ToString() + " " + idCol;
                    String countSql = "select count(" + idCol + ") ";

                    FieldMapping dtMapping = null;

                    foreach (FieldMapping fm in dc.Mappings)
                    {
                        if (fm.Field == FieldMapping.RECORD_DATE)
                        {
                            dtMapping = fm;
                            sql += ", " + dtMapping.GetValueSQL(dc);
                        }
                        if (fm.Field == FieldMapping.RECORD_STATUS)
                        {
                            sql += ", " + fm.GetValueSQL(dc);
                        }
                        if (rcm != null && fm.Field == FieldMapping.IDENTIFIER)
                        {
                            sql += ", " + fm.GetValueSQL(dc);
                        }
                    }

                    sql = sql.Trim();
                    sql = sql.TrimEnd(',');


                    String fromSql = "";
                    dc.RootTable.GetFullSql(ref fromSql);
                    sql += " from " + fromSql;
                    countSql += " from " + fromSql;

                    if (dtMapping != null && fromDate != null && fromDate != "")
                    {
                        sql += " where ";
                        sql += dtMapping.GetValueSQL(dc) + " >= '" + fromDate.ToString() + "' ";
                        countSql += " where ";
                        countSql += dtMapping.GetValueSQL(dc) + " >= '" + fromDate.ToString() + "' ";
                        if (toDate != null && toDate != "")
                        {
                            sql += "and ";
                            sql += dtMapping.GetValueSQL(dc) + " <= '" + toDate.ToString() + "' ";

                            countSql += "and ";
                            countSql += dtMapping.GetValueSQL(dc) + " <= '" + toDate.ToString() + "' ";
                        }
                    }

                    String pos = "";
                    if (_session.NextRecordPositions[dc.Set] != null) pos = _session.NextRecordPositions[dc.Set].ToString();

                    if (pos != null && pos != "")
                    {
                        sql += " and " + idCol + " > '" + pos + "'";
                        countSql += " and " + idCol + " > '" + pos + "'";
                    }
                    else
                    {
                        if (_requestedResumptionToken != null && _requestedResumptionToken != "") return null; //no records for this set in this iteration
                    }

                    sql += orderby;


                    //get total count of records
                    //only if this is the first call
                    if (pos == null || pos == "")
                    {
                        OleDbCommand cntCmd = cnn.CreateCommand();
                        cntCmd.CommandText = countSql;
                        int total = (int)cntCmd.ExecuteScalar();

                        _session.NumRecords += total;
                    }


                    cmd.CommandText = sql;

                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    int cnt = ds.Tables[0].Rows.Count;

                    if (cnt > _maxResults)
                    {
                        _session.NextRecordPositions[set] = ds.Tables[0].Rows[cnt - 2][idColName].ToString();
                        ds.Tables[0].Rows[cnt - 1].Delete();
                        ds.AcceptChanges();
                    }
                    else
                    {
                        _session.NextRecordPositions[set] = "";
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

            if (_rep == null) throw new OAIException(OAIError.badArgument);

            _requestedResumptionToken = resumptionToken;

            string xml = File.ReadAllText(Path.Combine(OAIServer.WebDir, "Responses\\ListIdentifiersResponse.xml"));

            xml = xml.Replace(FieldMapping.GET_DATE, DateTime.Now.ToString());

            if (fromDate != null && fromDate.Length > 0) xml = xml.Replace(FieldMapping.FROM_DATE, "from=\"" + fromDate + "\"");
            else xml = xml.Replace(FieldMapping.FROM_DATE, "");

            if (toDate != null && toDate.Length > 0) xml = xml.Replace(FieldMapping.TO_DATE, "until=\"" + toDate + "\"");
            else xml = xml.Replace(FieldMapping.TO_DATE, "");

            if (set != null && set.Length > 0) xml = xml.Replace(FieldMapping.SET, "set=\"" + set + "\"");
            else xml = xml.Replace(FieldMapping.SET, "");

            string url = System.ServiceModel.OperationContext.Current.IncomingMessageHeaders.To.OriginalString;
            if (url.IndexOf("?") != -1) url = url.Substring(0, url.IndexOf("?"));
            xml = xml.Replace(FieldMapping.BASE_URL, url);

            xml = xml.Replace(FieldMapping.METADATA_PREFIX, metadataPrefix);

            try
            {
                _results = GetResultData();

                bool moreRecords = false;
                foreach (String nextRec in _session.NextRecordPositions.Values)
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
                    resxml = File.ReadAllText(Path.Combine(OAIServer.WebDir, "Responses\\ResumptionSnippet.xml"));
                    resxml = resxml.Replace(FieldMapping.EXP_DATE, _session.ResumptionExpiry.ToString("s"));
                    resxml = resxml.Replace(FieldMapping.LIST_SIZE, _session.NumRecords.ToString());
                    resxml = resxml.Replace(FieldMapping.CURSOR, _session.Cursor.ToString());
                    resxml = resxml.Replace(FieldMapping.TOKEN, _session.ResumptionToken);
                }

                if (resxml != "")
                {
                    xml = xml.Replace(FieldMapping.RESUMPTION_TOKEN, resxml);
                }
                else
                {
                    xml = xml.Replace(FieldMapping.RESUMPTION_TOKEN, "");
                }

                String records = "";

                if (_results != null)
                {
                    foreach (DataTable resultTable in _results.Tables)
                    {
                        if (resultTable.Rows.Count == 0) continue;

                        _session.Cursor += resultTable.Rows.Count; //cursor incrmented for subsequent calls


                        DataConnection dc = _rep.GetDataConnection(resultTable.TableName);
                        DatabaseMapping idField = (DatabaseMapping)dc.GetMapping(FieldMapping.IDENTIFIER);

                        foreach (DataRow row in resultTable.Rows)
                        {
                            string recordXml = File.ReadAllText(Path.Combine(OAIServer.WebDir, "Responses\\HeaderSnippet.xml"));

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

                            records += recordXml + Environment.NewLine;
                        }
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

    }
}
