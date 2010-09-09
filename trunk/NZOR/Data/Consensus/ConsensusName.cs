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

        public static DsNameMatch GetNamesWithConcept(SqlConnection cnn, String conceptType, Guid nameToID)
        {
            DsNameMatch ds = new DsNameMatch();

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "sprSelect_NamesWithConcept";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@conceptType", DbType.String).Value = conceptType;
                cmd.Parameters.AddWithValue("@nameToID", DbType.Guid).Value = nameToID;


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

        /// <summary>
        /// Add new consensus name from provider name details
        /// </summary>
        /// <param name="provName"></param>
        /// <returns>new consensus name</returns>
        public static NZOR.Data.Consensus.Name AddConsensusName(DataSet provName)
        {
            NZOR.Data.Consensus.Name nm = new NZOR.Data.Consensus.Name();

            nm.AddedDate = DateTime.Now;
            nm.FullName = provName.Tables["Name"].Rows[0]["FullName"].ToString();
            nm.GoverningCode = provName.Tables["Name"].Rows[0]["GoverningCode"].ToString();
            nm.NameClass.NameClassID = (Guid)provName.Tables["Name"].Rows[0]["NameClassID"];
            nm.NameID = Guid.NewGuid();
            nm.OriginalOrthography = provName.Tables["Name"].Rows[0]["OriginalOrthography"].ToString();
            nm.TaxonRank.TaxonRankID = (Guid)provName.Tables["Name"].Rows[0]["TaxonRankID"];

            NZOR.Data.Provider.NZORProvider provData = new NZOR.Data.Provider.NZORProvider();
            NZOR.Data.Consensus.NZORConsensus consData = new NZOR.Data.Consensus.NZORConsensus();
            consData.AddToName(nm);

            //properties
            foreach (DataRow tpRow in provName.Tables["NameProperty"].Rows)
            {
                NZOR.Data.Consensus.NameProperty np = new NZOR.Data.Consensus.NameProperty();

                np.AddedDate = DateTime.Now;
                np.NameClassProperty.NameClassPropertyID = (Guid)tpRow["NameClassPropertyID"];
                np.Value = tpRow["Value"].ToString();
                if (!tpRow.IsNull("Sequence")) np.Sequence = (int)tpRow["Sequence"];

                if (tpRow["RelatedID"] != DBNull.Value)
                {
                    //connect to related provider name consensus id
                    var res = from nps in provData.Name where nps.NameID.ToString().Equals(tpRow["RelatedID"].ToString()) select nps;
                    if (res.Count() > 0)
                    {
                        NZOR.Data.Provider.Name pn = res.First();
                        np.RelatedID = pn.ConsensusNameID;
                    }
                }

                nm.NameProperty.Add(np);
            }

            consData.SaveChanges();

            return nm;
        }
    }
}
