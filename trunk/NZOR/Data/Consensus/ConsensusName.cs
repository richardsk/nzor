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
                cmd.CommandText = "select count(NameID) from provider.Name pn " +
                    " inner join provider.NameProperty np on np.NameID = pn.NameID " +
                    " inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID and ncp.Name = '" + field + "' " +
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
                    " inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID and ncp.Name = '" + field + "' " +
                    " where np.Value = '" + value.ToString() + "'; " +
                    "select n.* from cons.Name n inner join @ids i on i.id = n.NameID; " +
                    "select np.*, ncp.Name from cons.NameProperty np inner join @ids i on i.id = np.NameID inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID; " +
                    "select c.* from vwConsensusConcepts c inner join @ids i on i.id = c.NameID; ";

                DataSet res = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(res);

                foreach (DataRow row in res.Tables[0].Rows)
                {
                    Guid id = (Guid)row["NameID"];

                    DataRow ntRow = ConsensusName.GetNameConcept(id, res.Tables[2], ConceptProperties.ParentRelationshipType);
                    object nameTo = DBNull.Value;
                    if (ntRow != null && ntRow["NameToID"] != DBNull.Value) nameTo = (Guid)ntRow["NameToID"];

                    ds.Name.Rows.Add(id,
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Canonical),
                        row["FullName"],
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Rank),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Authors),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.CombinationAuthors),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Year),
                        nameTo,
                        100);
                }
            }

            return ds;
        }

        public static void UpdateFlatNameData(SqlConnection cnn, Guid nameID)
        {
            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "delete cons.FlatName where SeedNameID = '" + nameID.ToString() + "'";
                cmd.ExecuteNonQuery();
            }

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "INSERT cons.FlatName EXEC sprSelect_FlatNameToRoot '" + nameID.ToString() + "'";
                cmd.ExecuteNonQuery();
            }
        }

        public static DsNameMatch GetNamesWithConcept(SqlConnection cnn, String conceptType, Guid nameToID)
        {
            DsNameMatch ds = new DsNameMatch();

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"
	                    declare @ids table(id uniqueidentifier)
                    		
                        insert @ids 
                        select distinct n.NameID 
                        from cons.Name n 
                        inner join vwConsensusConcepts cc on cc.NameID = n.NameID 
                        where Relationship = '" + conceptType + "' and NameToID = '" + nameToID.ToString() + @"'
                        
                        select n.* 
                        from cons.Name n 
                        inner join @ids i on i.id = n.NameID
                        
                        select np.*, ncp.Name 
                        from cons.NameProperty np 
                        inner join @ids i on i.id = np.NameID 
                        inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID
                                        
	                    select c.* 
	                    from vwConsensusConcepts c 
	                    inner join @ids i on i.id = c.NameID";


                DataSet res = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(res);

                foreach (DataRow row in res.Tables[0].Rows)
                {
                    Guid id = (Guid)row["NameID"];

                    DataRow ntRow = ConsensusName.GetNameConcept(id, res.Tables[2], ConceptProperties.ParentRelationshipType);
                    object nameTo = DBNull.Value;
                    if (ntRow != null && ntRow["NameToID"] != DBNull.Value) nameTo = (Guid)ntRow["NameToID"];

                    ds.Name.Rows.Add(id,
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Canonical),
                        row["FullName"],
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Rank),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Authors),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.CombinationAuthors),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Year),
                        nameTo,
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
                if (dr["NameID"].ToString() == nameID.ToString() && dr["Name"].ToString() == field)
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

        /// <summary>
        /// Add new consensus name from provider name details
        /// </summary>
        /// <param name="provName"></param>
        /// <returns>new consensus name</returns>
        public static DataSet AddConsensusName(SqlConnection cnn, DsIntegrationName.ProviderNameRow provName)
        {
            Guid nameId = Guid.NewGuid();

            string sql = "insert cons.Name(NameID, AddedDate, FullName, GoverningCode, NameClassID, TaxonRankID) select '" +
                nameId.ToString() + "', '" +
                DateTime.Now.ToString("s") + "', '" +
                provName.FullName.Replace("'","''") + "', '" +
                provName.GoverningCode + "', '" +
                provName.NameClassID.ToString() + "', '" +
                provName.TaxonRankID.ToString() + "'";

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            DataSet provDs = ProviderName.GetName(cnn, provName.NameID);

            //properties
            foreach (DataRow tpRow in provDs.Tables["NameProperty"].Rows)
            {
                int seq = -1;
                if (!tpRow.IsNull("Sequence")) seq = (int)tpRow["Sequence"];
                Guid relId = Guid.Empty;
                if (tpRow["RelatedID"] != DBNull.Value) relId = (Guid)tpRow["RelatedID"];

                string val = tpRow["Value"].ToString().Replace("'", "''");

                sql = "insert cons.NameProperty(NamePropertyID, NameID, AddedDate, NameClassPropertyID, Value, Sequence, RelatedID) select '" +
                    Guid.NewGuid().ToString() + "', '" +
                    nameId.ToString() + "', '" +
                    DateTime.Now.ToString("s") + "', '" +
                    tpRow["NameClassPropertyID"].ToString() + "', '" +
                    val + "', " +
                    (seq == -1 ? "null, " : seq.ToString() + ", ") +
                    (relId == Guid.Empty ? "null" : "'" + relId.ToString() + "'");

                using (SqlCommand npCmd = cnn.CreateCommand())
                {
                    npCmd.CommandText = sql;
                    npCmd.ExecuteNonQuery();
                }
            }

            //Update Flat Name data
            UpdateFlatNameData(cnn, nameId);

            return GetName(cnn, nameId);
        }

        public static DataSet GetName(SqlConnection cnn, Guid nameId)
        {
            DataSet ds = null;

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "select * from cons.Name where NameID = '" + nameId.ToString() + "'; select * from cons.NameProperty where NameID = '" + nameId.ToString() + "'";

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
            }
            return ds;
        }
    }
}
