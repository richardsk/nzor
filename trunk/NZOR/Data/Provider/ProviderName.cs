using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.EntityClient;

namespace NZOR.Data
{
    public class ProviderName
    {
        public static void GetParentData(SqlConnection cnn, System.Data.DataSet pn)
        {
            //Check we have a parent concept.  If not get fuzzy matches for use in matching process
            //This routine makes sure that all names that have been selected for possible match are at the correct rank.
            //  Names that may be selected could be children of the same parent as the matching name, but this does not mean the names will be the 
            //  same rank - eg matching name "Aus bus var. dus" - may select a name in the DB "Aus bus subsp. cus" as "cus" is a child of the naem "Aus bus"            

            System.Data.DataRow parRow = GetNameConcept(pn.Tables["Concepts"], NZOR.Data.ConceptProperties.ParentRelationshipType);
            string fullName = pn.Tables["Name"].Rows[0]["FullName"].ToString();
            
            DataSet ds = new DataSet();
            Guid parentNameID = Guid.Empty;
            string parFullName = "";
            Guid parRank = Guid.Empty;

            if (parRow == null || parRow.IsNull("ConsensusNameToID"))
            {
                //NO Parent CONCEPT - check for higher ranks
                System.String pnCanonical = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Canonical).ToString();
                Guid rankId = (Guid)pn.Tables["Name"].Rows[0]["TaxonRankID"];
                string govCode = pn.Tables["Name"].Rows[0]["GoverningCode"].ToString();
                NZOR.Data.SystemData.TaxonRank tr = Data.SystemData.TaxonRankData.GetTaxonRank(cnn, rankId);

                //TODO - CHECK THIS !  - do we need to allow for Provider/Dataset preferences - ie provider specifies the location in the taxon hierarchy where names should fit
                //ORDER and above - just match canonical and rank 
                if (tr.SortOrder <= 1600)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = "select distinct fn.ParentNameID, n.FullName, n.TaxonRankID from cons.Name n inner join cons.nameproperty np on np.nameid = n.nameid "
                            + " inner join dbo.nameclassproperty ncp on ncp.nameclasspropertyid = np.nameclasspropertyid "
                            + " inner join cons.FlatName fn on fn.NameID = n.NameID where n.TaxonRankID = '"
                            + tr.TaxonRankId.ToString() + "' and np.Value = '" + pnCanonical + "' and ncp.propertyname = '"
                            + NZOR.Data.NameProperties.Canonical + "' and n.GoverningCode = '" + govCode + "'";

                        DataSet pds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(pds);

                        if (pds.Tables.Count > 0 && pds.Tables[0].Rows.Count == 1)
                        {
                            parentNameID = (Guid)pds.Tables[0].Rows[0]["ParentNameID"];
                            parFullName = pds.Tables[0].Rows[0]["FullName"].ToString();
                            parRank = (Guid)pds.Tables[0].Rows[0]["TaxonRankID"];
                        }
                    }
                }

                //Below GENUS - use the Genus (first word of the full name)
                if (tr.SortOrder > 3000)
                {
                    if (fullName.IndexOf(" ") != -1)
                    {
                        String parent = fullName.Substring(0, fullName.IndexOf(" "));

                        using (SqlCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = "select n.NameID from cons.Name n inner join cons.nameproperty np on np.nameid = n.nameid "
                                + " inner join dbo.nameclassproperty ncp on ncp.nameclasspropertyid = np.nameclasspropertyid where TaxonRankID = '"
                                + NZOR.Data.SystemData.TaxonRankData.GenusRank(cnn).TaxonRankId.ToString() + "' and np.Value = '" + parent + "' and ncp.propertyname = '"
                                + NZOR.Data.NameProperties.Canonical + "' and n.GoverningCode = '" + govCode + "'";

                            DataSet pds = new DataSet();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(pds);

                            if (pds.Tables.Count > 0 && pds.Tables[0].Rows.Count == 1)
                            {
                                parentNameID = (Guid)pds.Tables[0].Rows[0]["NameID"];
                            }
                        }
                    }
                }

                if (parentNameID != Guid.Empty)
                {
                    //add parent concept
                    //copy from name details
                    DataRow cRow = pn.Tables["Concepts"].NewRow();
                    cRow["NameID"] = pn.Tables[0].Rows[0]["NameID"];
                    cRow["ConsensusNameID"] = pn.Tables[0].Rows[0]["ConsensusNameID"];
                    cRow["FullName"] = pn.Tables[0].Rows[0]["FullName"];
                    cRow["TaxonRankID"] = pn.Tables[0].Rows[0]["TaxonRankID"];
                    //cRow["RankName"] = pn.Tables[0].Rows[0]["RankName"];
                    cRow["SortOrder"] = pn.Tables[0].Rows[0]["SortOrder"];
                    cRow["NameClassID"] = pn.Tables[0].Rows[0]["NameClassID"];
                    cRow["LinkStatus"] = pn.Tables[0].Rows[0]["LinkStatus"];
                    cRow["OriginalOrthography"] = pn.Tables[0].Rows[0]["OriginalOrthography"];
                    cRow["GoverningCode"] = pn.Tables[0].Rows[0]["GoverningCode"];
                    cRow["ProviderID"] = pn.Tables[0].Rows[0]["ProviderID"];
                    cRow["ProviderRecordID"] = pn.Tables[0].Rows[0]["ProviderRecordID"];
                    cRow["ProviderUpdatedDate"] = pn.Tables[0].Rows[0]["ProviderUpdatedDate"];
                    cRow["AddedDate"] = DateTime.Now;

                    //add parent concept to parent name id
                    cRow["ConceptID"] = Guid.Empty; //not a real concept
                    cRow["RelationshipTypeID"] = ConceptRelationshipType.ParentRelationshipTypeID(cnn);
                    cRow["Relationship"] = ConceptProperties.ParentRelationshipType;
                    cRow["NameToID"] = Guid.Empty; //dummy
                    cRow["ConsensusNameToID"] = parentNameID;
                    cRow["NameToFull"] = parFullName;
                    cRow["TaxonRankToID"] = parRank;

                    pn.Tables["Concepts"].Rows.Add(cRow);
                }
            }
        }

        public static System.Data.DataSet GetNameMatchData(SqlConnection cnn, Guid provNameId)
        {
            //TODO SQL Transaction to prevent deadlocks!!

            System.Data.DataSet ds = new System.Data.DataSet();

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"
	                        select * 
	                        from prov.Name pn
	                        inner join TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
	                        where NameID = '" + provNameId.ToString() + @"'
                        	
	                        select * 
	                        from prov.NameProperty np
	                        inner join NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID
	                        where NameID = '" + provNameId.ToString() + @"'
                        	
	                        select * 
	                        from vwProviderConcepts
	                        where NameID = '" + provNameId.ToString() + @"'";

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                ds.Tables[0].TableName = "Name";
                ds.Tables[1].TableName = "NameProperty";
                ds.Tables[2].TableName = "Concepts";

                //if no parent concept, get "fuzzy" match parents
                GetParentData(cnn, ds);
            }

            return ds;
        }

        public static Object GetNamePropertyValue(System.Data.DataTable namePropDt, String field)
        {
            Object val = DBNull.Value;

            foreach (System.Data.DataRow dr in namePropDt.Rows)
            {
                if (dr["PropertyName"].ToString() == field)
                {
                    val = dr["Value"];
                    break;
                }
            }

            return val;
        }

        public static System.Data.DataRow GetNameConcept(System.Data.DataTable conceptsDt, String conceptType)
        {
            System.Data.DataRow r = null;

            foreach (System.Data.DataRow dr in conceptsDt.Rows)
            {
                if (dr["Relationship"].ToString() == conceptType)
                {
                    r = dr;
                    break;
                }
            }

            return r;
        }

        public static void UpdateProviderNameLink(SqlConnection cnn, DataSet provName, LinkStatus status, Guid? nameId, int matchScore, string matchPath)
        {
            String id = provName.Tables["Name"].Rows[0]["NameID"].ToString();

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "update prov.Name set LinkStatus = '" + status.ToString() + "', MatchScore = " + matchScore.ToString() + ", MatchPath = '" + matchPath +
                    "', ConsensusNameID = " + (nameId.HasValue ? "'" + nameId.Value.ToString() + "' " : "null ") +
                    "where NameID = '" + id + "'";

                cmd.ExecuteNonQuery();
            }
        }
    }
}
