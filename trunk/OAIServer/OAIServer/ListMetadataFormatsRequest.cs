using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Xml.Linq;

namespace OAIServer
{
    public class ListMetadataFormatsRequest
    {
        private DataSet GetResultData(RepositoryConfig rep, String id)
        {
            DataSet ds = null;

            foreach (DataConnection dc in rep.DataConnections)
            {
                if (dc.DBConnStr != null && dc.DBConnStr.Length > 0)
                {
                    DataSet tmpDs = GetResultData(rep, id, dc.Set);
                    if (ds == null) ds = tmpDs;
                    else if (tmpDs != null) ds.Merge(tmpDs);
                }
            }

            return ds;
        }

        private DataSet GetResultData(RepositoryConfig rep, String id, String set)
        {
            DataSet ds = null;

            DataConnection dc = rep.GetDataConnection(set);

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

        public XElement GetResultXml(String repository, String identifier)
        {
            WebOperationContext ctx = WebOperationContext.Current;

            RepositoryConfig rep = OAIServer.GetConfig(repository);

            if (rep == null) throw new OAIException(OAIError.badArgument);


            string xml = File.ReadAllText(Path.Combine(OAIServer.WebDir, "Responses\\ListMetadataFormatsResponse.xml"));
            xml = xml.Replace(FieldMapping.GET_DATE, DateTime.Now.ToString());

            string url = System.ServiceModel.OperationContext.Current.IncomingMessageHeaders.To.OriginalString;
            if (url.IndexOf("?") != -1) url = url.Substring(0, url.IndexOf("?"));
            xml = xml.Replace(FieldMapping.BASE_URL, url);


            //work out what set this id is in
            //DataSet results = GetResultData(rep, identifier);

            //String set = "";
            //foreach (DataTable dt in results.Tables)
            //{
            //    if (dt.Rows.Count > 0)
            //    {
            //        set = dt.TableName;
            //        break;
            //    }
            //}

            xml = xml.Replace(FieldMapping.IDENTIFIER, identifier);


            return XElement.Parse(xml);
        }

    }
}
