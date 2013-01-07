using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesAtRankWithAncestor : BaseMatcher
    {

        public NamesAtRankWithAncestor()
        {
        }


        public override DsNameMatch GetMatchingNames(DsIntegrationName.ProviderNameRow pn, ref string matchComments)
        {
            return null;
        }

        public override void RemoveNonMatches(DsIntegrationName.ProviderNameRow pn, ref DsNameMatch names, ref string matchComments)
        {
            //PROBABLY NOT REQUIRED NOW - just add the correct names in NamesWithSameParent

            //adds matches, not removes them 
            //This routine makes sure that all names that have been selected for possible match are at the correct rank.
            //  Names may have been selected at this point because they are a child of the same parent as the matching name, but this does not mean the names will be the 
            //  correct rank - eg matching name "Aus bus var. dus" - may select a name in the DB "Aus bus subsp. cus" as "cus" is a child of the naem "Aus bus"
            //gets the descendent names with correct rank 

//            DsNameMatch ds = new DsNameMatch();

//            string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
//            using (SqlConnection cnn = new SqlConnection(ConnectionString))
//            {
//                cnn.Open();

//                String rankId = pn.Tables["Name"].Rows[0]["TaxonRankID"].ToString();
//                foreach (DsNameMatch.NameRow row in names.Name)
//                {
//                    using (SqlCommand cmd = cnn.CreateCommand())
//                    {
//                        cmd.CommandText = @"
//                            declare @ids table(id uniqueidentifier)
//		
//                            insert @ids 
//                            select distinct SeedNameID 
//                            from cons.FlatName	
//                            where TaxonRankID = '" + rankId + "' and NameId = '" + row.NameID.ToString() + "';" +
//                           @"                            
//                            select n.* 
//                            from consensus.Name n 
//                            inner join @ids i on i.id = n.NameID;
//                            
//                            select np.*, ncp.PropertyName 
//                            from consensus.NameProperty np 
//                            inner join @ids i on i.id = np.NameID 
//                            inner join dbo.NamePropertyType ncp on ncp.NamePropertyTypeID = np.NamePropertyTypeID;
//                                            
//	                        select c.* 
//	                        from vwConsensusConcepts c 
//	                        inner join @ids i on i.id = c.NameID;";

//                        DsNameMatch res = new DsNameMatch();
//                        SqlDataAdapter da = new SqlDataAdapter(cmd);
//                        da.Fill(res);

//                        foreach (DataRow resRow in res.Tables[0].Rows)
//                        {
//                            Guid id = (Guid)row["NameID"];
//                            ds.Name.AddNameRow(id,
//                                GetNamePropertyValue(id, res.Tables[1], NameProperties.Canonical).ToString(),
//                                row["FullName"].ToString(),
//                                GetNamePropertyValue(id, res.Tables[1], NameProperties.Rank).ToString(),
//                                GetNamePropertyValue(id, res.Tables[1], NameProperties.Authors).ToString(),
//                                GetNamePropertyValue(id, res.Tables[1], NameProperties.CombinationAuthors).ToString(),
//                                GetNamePropertyValue(id, res.Tables[1], NameProperties.Year).ToString(),
//                                (Guid)GetNameConcept(id, res.Tables[2], ConceptProperties.ParentRelationshipType)["NameToID"],
//                                100);
//                        }
//                    }
//                }

            //    names = ds;
                
            //    if (cnn.State != ConnectionState.Closed) cnn.Close();

            //}
        }


    }
}
