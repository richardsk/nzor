﻿using System;
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
                        
                        select np.*, ncp.PropertyName 
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

            NZOR.Data.Provider.NZORProvider provData = new NZOR.Data.Provider.NZORProvider();
            NZOR.Data.Consensus.NZORConsensus consData = new NZOR.Data.Consensus.NZORConsensus();
            
            nm.AddedDate = DateTime.Now;
            nm.FullName = provName.Tables["Name"].Rows[0]["FullName"].ToString();
            nm.GoverningCode = provName.Tables["Name"].Rows[0]["GoverningCode"].ToString();
            Guid nmCls = (Guid)provName.Tables["Name"].Rows[0]["NameClassID"];
            nm.NameClass = (from c in consData.NameClass where c.NameClassID == nmCls select c).FirstOrDefault();
            nm.NameID = Guid.NewGuid();
            nm.OriginalOrthography = provName.Tables["Name"].Rows[0]["OriginalOrthography"].ToString();
            Guid rnk = (Guid)provName.Tables["Name"].Rows[0]["TaxonRankID"];
            nm.TaxonRank = (from r in consData.TaxonRank where r.TaxonRankID == rnk select r).FirstOrDefault();

            consData.AddToName(nm);

            //properties
            foreach (DataRow tpRow in provName.Tables["NameProperty"].Rows)
            {
                NZOR.Data.Consensus.NameProperty np = new NZOR.Data.Consensus.NameProperty();

                np.NamePropertyID = Guid.NewGuid();
                np.AddedDate = DateTime.Now;
                Guid ncp = (Guid)tpRow["NameClassPropertyID"];
                np.NameClassProperty = (from nc in consData.NameClassProperty where nc.NameClassPropertyID == ncp select nc).FirstOrDefault();
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

            //Update Flat Name data
            using (SqlConnection cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString))
            {
                cnn.Open();
                UpdateFlatNameData(cnn, nm.NameID);
                cnn.Close();
            }

            return nm;
        }
    }
}
