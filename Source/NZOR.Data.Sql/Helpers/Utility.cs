using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace NZOR.Data.Sql
{
    public static class Utility
    {
        public static String GetSQL(String sqlName)
        {
            String sql = String.Empty;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using (System.IO.Stream stream = assembly.GetManifestResourceStream(sqlName))
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(stream))
                {
                    sql = streamReader.ReadToEnd();
                }
            }

            return sql;
        }

        public static DataTable GetSourceData(String sourceConnectionString, String commandText)
        {
            return GetSourceData(sourceConnectionString, commandText, null, 30);
        }

        public static DataTable GetSourceData(String sourceConnectionString, String commandText, List<SqlParameter> parameters)
        {
            return GetSourceData(sourceConnectionString, commandText, parameters, 30);
        }

        public static DataTable GetSourceData(String sourceConnectionString, String commandText, List<SqlParameter> parameters, int timeoutSeconds)
        {
            DataTable tbl = new DataTable();

            using (SqlConnection cnn = new SqlConnection(sourceConnectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = commandText;
                    cmd.CommandTimeout = timeoutSeconds;
                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    using (SqlDataAdapter dad = new SqlDataAdapter(cmd))
                    {
                         dad.Fill(tbl);
                    }
                }
                cnn.Close();
            }

            return tbl;
        }

        public static string ApplyXSLT(string xml, string xslt, System.Data.SqlTypes.SqlBoolean addRootNode)
        {
            string result = "";

            try
            {
                XslCompiledTransform xsl = new XslCompiledTransform();

                System.Xml.XmlTextReader xslXr = new System.Xml.XmlTextReader(new System.IO.StringReader(xslt));
                xsl.Load(xslXr);

                //add root node?
                if (addRootNode.IsTrue)
                {
                    xml = "<root>" + xml + "</root>";
                }

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);

                System.IO.StringWriter sw = new System.IO.StringWriter();
                System.Xml.XmlTextWriter resXr = new System.Xml.XmlTextWriter(sw);
                xsl.Transform(doc, resXr);

                result = sw.ToString();
            }
            catch (Exception ex)
            {
                result = ex.Message;
                if (ex.InnerException != null)
                    result += " : " + ex.InnerException.Message;
            }

            return result;
        }        
    }
}
