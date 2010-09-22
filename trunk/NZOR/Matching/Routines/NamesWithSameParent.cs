using System;
using System.Data;
using System.Data.SqlClient;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithSameParent : BaseMatcher
    {

        public NamesWithSameParent()
        {
        }

        public override DsNameMatch GetMatchingNames(System.Data.DataSet pn, ref string matchComments)
        {
            //This routine makes sure that all names that have been selected for possible match are at the correct rank.
            //  Names that may be selected could be children of the same parent as the matching name, but this does not mean the names will be the 
            //  same rank - eg matching name "Aus bus var. dus" - may select a name in the DB "Aus bus subsp. cus" as "cus" is a child of the naem "Aus bus"            

            System.Data.DataRow parRow = NZOR.Data.ProviderName.GetNameConcept(pn.Tables["Concepts"], NZOR.Data.ConceptProperties.ParentRelationshipType);

            string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            DsNameMatch ds = new DsNameMatch();
            Guid parentNameID = Guid.Empty;

            if (parRow != null && !parRow.IsNull("ConsensusNameToID"))
            {
                parentNameID = (Guid)parRow["ConsensusNameToID"];
            }

            //THIS FUZZY matching is now done when the provider name data is first select (to save returning to the DB multiple times)
            // ...
            //else
            //{
            //    //NO Parent CONCEPT - check for higher ranks
            //    System.String pnCanonical = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Canonical).ToString();
            //    Guid rankId = (Guid)pn.Tables["Name"].Rows[0]["TaxonRankID"];
            //    string govCode = pn.Tables["Name"].Rows[0]["GoverningCode"].ToString();
            //    NZOR.Data.SystemData.TaxonRank tr = Data.SystemData.TaxonRankData.GetTaxonRank(rankId);

            //    matchComments = "No parent specified.  Matching on epithet for higher level rank '" + tr.Name + "'.";

            //    //TODO - CHECK THIS !  - do we need to allow for Provider/Dataset preferences - ie provider specifies the location in the taxon hierarchy where names should fit
            //    //ORDER and above - just match canonical and rank 
            //    if (tr.SortOrder <= 1600)
            //    {
            //        using (SqlConnection cnn = new SqlConnection(ConnectionString))
            //        {
            //            cnn.Open();
            //            using (SqlCommand cmd = cnn.CreateCommand())
            //            {
            //                cmd.CommandText = "select distinct fn.ParentNameID from cons.Name n inner join cons.nameproperty np on np.nameid = n.nameid "
            //                    + " inner join dbo.nameclassproperty ncp on ncp.nameclasspropertyid = np.nameclasspropertyid "
            //                    + " inner join cons.FlatName fn on fn.NameID = n.NameID where n.TaxonRankID = '"
            //                    + tr.TaxonRankID.ToString() + "' and np.Value = '" + pnCanonical + "' and ncp.propertyname = '"
            //                    + NZOR.Data.NameProperties.Canonical + "' and n.GoverningCode = '" + govCode + "'";

            //                DataSet pds = new DataSet();
            //                SqlDataAdapter da = new SqlDataAdapter(cmd);
            //                da.Fill(pds);

            //                if (pds.Tables.Count > 0 && pds.Tables[0].Rows.Count == 1)
            //                {
            //                    parentNameID = (Guid)pds.Tables[0].Rows[0]["ParentNameID"];
            //                }
            //            }

            //            if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
            //        }
            //    }
            //}

            if (parentNameID != Guid.Empty)
            {
                String rankId = pn.Tables["Name"].Rows[0]["TaxonRankID"].ToString();

                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    cnn.Open();

                    //ds = NZOR.Data.ConsensusName.GetNamesWithConcept(cnn, NZOR.Data.ConceptProperties.ParentRelationshipType, parentId);


                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            declare @ids table(id uniqueidentifier)
		
                            insert @ids 
                            select distinct SeedNameID 
                            from cons.FlatName fn 
                            inner join cons.name sn on sn.nameid = fn.seednameid
                            where sn.TaxonRankID = '" + rankId + "' and fn.NameId = '" + parentNameID.ToString() + "';" + @"
                            
                            select n.* 
                            from cons.Name n 
                            inner join @ids i on i.id = n.NameID;
                            
                            select np.*, ncp.PropertyName 
                            from cons.NameProperty np 
                            inner join @ids i on i.id = np.NameID 
                            inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID;
                                            
                            select c.* 
                            from vwConsensusConcepts c 
                            inner join @ids i on i.id = c.NameID;";

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
                                ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.Canonical),
                                row["FullName"],
                                ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.Rank),
                                ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.Authors),
                                ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.CombinationAuthors),
                                ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.Year),
                                nameTo,
                                100);
                        }
                    }


                    if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
                }
            }

            return ds;
        }

        public override void RemoveNonMatches(DataSet pn, ref DsNameMatch names, ref string matchComments)
        {
            System.Data.DataRow parRow = NZOR.Data.ProviderName.GetNameConcept(pn.Tables["Concepts"], NZOR.Data.ConceptProperties.ParentRelationshipType);

            DsNameMatch ds = new DsNameMatch();

            if (!parRow.IsNull("ConsensusNameToID"))
            {
                System.Guid parentId = (System.Guid)parRow["ConsensusNameToID"];

                for (int i = names.Name.Count - 1; i >= 0; i--)
                {
                    DsNameMatch.NameRow nmRow = names.Name[i];
                    if (nmRow.ParentID != parentId) nmRow.Delete();
                }
            }

            ds.AcceptChanges();
        }
    }
}
