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

        public override DsNameMatch GetMatchingNames(DsIntegrationName pn, ref string matchComments)
        {
            //This routine makes sure that all names that have been selected for possible match are at the correct rank.
            //  Names that may be selected could be children of the same parent as the matching name, but this does not mean the names will be the 
            //  same rank - eg matching name "Aus bus var. dus" - may select a name in the DB "Aus bus subsp. cus" as "cus" is a child of the naem "Aus bus"            
            
            DsNameMatch ds = new DsNameMatch();
            Guid parentNameID = Guid.Empty;

            if (!pn.ProviderName[0].IsParentConsensusNameIDNull())
            {
                parentNameID = pn.ProviderName[0].ParentConsensusNameID;
            }
            
            if (parentNameID != Guid.Empty)
            {
                String rankId = pn.ProviderName[0].TaxonRankID.ToString();


                    //ds = NZOR.Data.ConsensusName.GetNamesWithConcept(cnn, NZOR.Data.ConceptProperties.ParentRelationshipType, parentId);


                using (SqlCommand cmd = DBConnection.CreateCommand())
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
                            
                            select np.*, ncp.Name 
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
            }

            return ds;
        }

        public override void RemoveNonMatches(DsIntegrationName pn, ref DsNameMatch names, ref string matchComments)
        {
            DsNameMatch ds = new DsNameMatch();

            if (!pn.ProviderName[0].IsParentConsensusNameIDNull())
            {
                System.Guid parentId = pn.ProviderName[0].ParentConsensusNameID;

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
