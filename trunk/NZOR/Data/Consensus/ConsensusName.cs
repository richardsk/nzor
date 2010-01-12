using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace NZOR.Data
{
    public class ConsensusName
    {

        public static bool HasProviderValue(SqlConnection cnn, Guid nameID, String field, object value)
        {
            bool hasVal = false;

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "select count(NameID) from prov.Name pn " +
                    " inner join prov.NameProperty np on np.NameID = pn.NameID " +
                    " inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID and ncp.PropertyName = '" + field + "' " +
                    " where pn.ConsensusNameID = '" + nameID.ToString() + "' and (np.Value is null or np.Value = '" + value.ToString() + "')";

                int cnt = (int)cmd.ExecuteScalar();
                if (cnt > 0) hasVal = true;
            }

            return hasVal;
        }

        public static DsNameMatch GetNamesWithProperty(SqlConnection cnn, String field, object value)
        {
            DsNameMatch ds = new DsNameMatch();

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "declare @ids table(id uniqueidentifier); " +
                    "insert @ids select distinct n.NameID from cons.Name n inner join cons.NameProperty np on np.NameID = n.NameID " +
                    " inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID and ncp.PropertyName = '" + field + "' " +
                    " where np.Value = '" + value.ToString() + "'; " +
                    "select n.* from cons.Name n inner join @ids i on i.id = n.NameID; " +
                    "select np.*, ncp.PropertyName from cons.NameProperty np inner join @ids i on i.id = np.NameID inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID; " +
                    "select c.* from vwConsensusConcepts c inner join @ids i on i.id = c.NameID; ";

                DataSet res = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(res);

                foreach (DataRow row in res.Tables[0].Rows)
                {
                    Guid id = (Guid)row["NameID"];
                    ds.Name.AddNameRow(id,
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Canonical).ToString(),
                        row["FullName"].ToString(),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Rank).ToString(),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Authors).ToString(),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Year).ToString(),
                        (Guid)GetNameConcept(id, res.Tables[2], ConceptProperties.ParentRelationshipType)["NameToID"],
                        100);
                }
            }

            return ds;
        }

        public static DsNameMatch GetNamesWithConcept(SqlConnection cnn, String conceptType, Guid nameToID)
        {
            DsNameMatch ds = new DsNameMatch();

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "declare @ids table(id uniqueidentifier); " +
                    "insert @ids select distinct n.NameID from cons.Name n inner join vwConsensusConcepts cc on cc.NameID = n.NameID where Relationship = '" +
                    conceptType + "' and NameToID = '" + nameToID.ToString() + "'; " +
                    "select n.* from cons.Name n inner join @ids i on i.id = n.NameID; " +
                    "select np.*, ncp.PropertyName from cons.NameProperty np inner join @ids i on i.id = np.NameID inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID; " +
                    "select c.* from vwConsensusConcepts c inner join @ids i on i.id = c.NameID; ";

                DataSet res = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(res);

                foreach (DataRow row in res.Tables[0].Rows)
                {
                    Guid id = (Guid)row["NameID"];
                    ds.Name.AddNameRow(id,
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Canonical).ToString(),
                        row["FullName"].ToString(),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Rank).ToString(),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Authors).ToString(),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Year).ToString(),
                        (Guid)GetNameConcept(id, res.Tables[2], ConceptProperties.ParentRelationshipType)["NameToID"],
                        100);
                }
            }

            return ds;
        }

        public static Object GetNamePropertyValue(Guid nameID, System.Data.DataTable namePropDt, String field)
        {
            Object val = DBNull.Value;

            foreach (System.Data.DataRow dr in namePropDt.Rows)
            {
                if (dr["NameID"].ToString() == nameID.ToString() && dr["PropertyName"].ToString() == field)
                {
                    val = dr["Value"];
                    break;
                }
            }

            return val;
        }

        public static System.Data.DataRow GetNameConcept(Guid nameID, System.Data.DataTable conceptsDt, String conceptType)
        {
            System.Data.DataRow r = null;

            foreach (System.Data.DataRow dr in conceptsDt.Rows)
            {
                if (dr["NameID"].ToString() == nameID.ToString() && dr["Relationship"].ToString() == conceptType)
                {
                    r = dr;
                    break;
                }
            }

            return r;
        }
    }
}
