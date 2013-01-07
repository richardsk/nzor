using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using NZOR.Data;
using NZOR.Data.DataSets;
using NZOR.Data.LookUps.Common;

namespace NZOR.Matching
{
    public class NamesWithSameParent : BaseMatcher
    {

        public NamesWithSameParent()
        {
        }

        public override DsNameMatch GetMatchingNames(DsIntegrationName.ProviderNameRow pn, ref string matchComments)
        {
            //This routine makes sure that all names that have been selected for possible match are at the correct rank.
            //  Names that may be selected could be children of the same parent as the matching name, but this does not mean the names will be the 
            //  same rank - eg matching name "Aus bus var. dus" - may select a name in the DB "Aus bus subsp. cus" as "cus" is a child of the naem "Aus bus"            
            
            DsNameMatch ds = new DsNameMatch();
            List<Guid> parentConsNameIDs = Data.Sql.Integration.GetParentConsensusNameIDs(pn);
            List<String> parents = Data.Sql.Integration.GetParentConsensusNames(pn);
                        
            if (parentConsNameIDs.Count > 0 || (parents.Count > 0 && pn.TaxonRankSort >= 4200))
            {
                String rankSort = pn["TaxonRankSort"].ToString();
                
                if (UseDBConnection)
                {
                    using (SqlConnection cnn = new SqlConnection(DBConnectionString))
                    {
                        cnn.Open();
                        using (SqlCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = @"
                            declare @ids table(id uniqueidentifier)
		
                            insert @ids 
                            select distinct n.NameID from consensus.name n
                            inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID 
                            inner join consensus.StackedName sn on sn.SeedNameId = n.NameID
                            where (";

                            string pids = "";
                            foreach (Guid pnId in parentConsNameIDs)
                            {
                                if (pids.Length > 0) pids += " or ";
                                pids += " sn.NameID = '" + pnId.ToString() + "'";
                            }
                            cmd.CommandText += pids + ") and tr.SortOrder >= " + rankSort + ";";

                            cmd.CommandText += @"                            
                            select n.* 
                            from consensus.Name n 
                            inner join @ids i on i.id = n.NameID;
                            
                            select np.*, ncp.Name 
                            from consensus.NameProperty np 
                            inner join @ids i on i.id = np.NameID 
                            inner join dbo.NamePropertyType ncp on ncp.NamePropertyTypeID = np.NamePropertyTypeID;
                                            
                            select c.* 
                            from consensus.vwConcepts c 
                            inner join @ids i on i.id = c.NameID and c.IsActive = 1;";

                            DataSet res = new DataSet();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(res);

                            foreach (DataRow row in res.Tables[0].Rows)
                            {
                                Guid id = (Guid)row["NameID"];

                                DataRow ntRow = NZOR.Data.Sql.Integration.GetNameConcept(id, res.Tables[2], ConceptRelationshipTypeLookUp.IsChildOf);
                                object nameTo = DBNull.Value;
                                if (ntRow != null && ntRow["NameToID"] != DBNull.Value) nameTo = (Guid)ntRow["NameToID"];

                                ds.Name.Rows.Add(id,
                                    NZOR.Data.Sql.Integration.GetNamePropertyValue(id, res.Tables[1], NamePropertyTypeLookUp.Canonical),
                                    row["FullName"],
                                    NZOR.Data.Sql.Integration.GetNamePropertyValue(id, res.Tables[1], NamePropertyTypeLookUp.Rank),
                                    NZOR.Data.Sql.Integration.GetNamePropertyValue(id, res.Tables[1], NamePropertyTypeLookUp.Authors),
                                    NZOR.Data.Sql.Integration.GetNamePropertyValue(id, res.Tables[1], NamePropertyTypeLookUp.CombinationAuthors),
                                    NZOR.Data.Sql.Integration.GetNamePropertyValue(id, res.Tables[1], NamePropertyTypeLookUp.Year),
                                    nameTo,
                                    row["GoverningCode"],
                                    100);
                            }
                        }
                    }
                }
                else
                {
                    List<DataRow> rows = new List<DataRow>();

                    MatchData.GetConsensusDataLock();

                    //par names [Parent Guid:Rank Guid],[Parent Guid:Rank Guid] ...
                    //get consensus names that have the provider name parent consensus name as part of its parent chain and are at a lower rank than this parent 
                    //

                    if (parentConsNameIDs.Count == 0)
                    {                        
                        String sql = "";
                        foreach (String pname in parents)
                        {
                            if (sql.Length > 0) sql += " or ";
                            sql += "Canonical = '" + pname.Replace("'", "''") + "'";
                        }

                        sql = "(" + sql + ") and TaxonRankSort < " + pn["TaxonRankSort"].ToString();

                        DataRow[] parRows = MatchData.AllData.ConsensusData.ConsensusName.Select(sql);
                        if (parRows.Length == 1)
                        {
                            DataRow[] children = MatchData.AllData.ConsensusData.ConsensusName.Select("ParentID = '" + parRows[0]["NameID"].ToString() + "'");
                            rows.AddRange(children);
                        }
                    }
                    else
                    {
                        foreach (DsConsensusData.ConsensusNameRow cnRow in MatchData.AllData.ConsensusData.ConsensusName)
                        {

                            if (cnRow.TaxonRankSort >= pn.TaxonRankSort)
                            {
                                bool parFound = false;
                                foreach (Guid pid in parentConsNameIDs)
                                {
                                    if (cnRow["ParentIDsToRoot"].ToString().ToLower().IndexOf("[" + pid.ToString().ToLower()) != -1)
                                    {
                                        parFound = true;
                                        break;
                                    }
                                }

                                if (parFound) rows.Add(cnRow);
                            }
                        }
                    }

                    //rows = MatchData.DataForIntegration.ConsensusName.Select("charindex('" + pn.ParentConsensusNameID.ToString() + "', ParentIDsToRoot ) <> 0");

                    MatchData.ReleaseConsensusDataLock();

                    foreach (DataRow row in rows)
                    {
                        ds.Name.Rows.Add((Guid)row["NameID"],
                                    row["Canonical"].ToString(),
                                    row["FullName"].ToString(),
                                    row["TaxonRank"].ToString(),
                                    row["Authors"],
                                    row["CombinationAuthors"],
                                    row["YearOfPublication"],
                                    row["ParentID"],
                                    row["GoverningCode"],
                                    100);                        
                    }
                }
            }

            return ds;
        }

        public override void RemoveNonMatches(DsIntegrationName.ProviderNameRow pn, ref DsNameMatch names, ref string matchComments)
        {
            DsNameMatch ds = new DsNameMatch();

            List<Guid> parentConsNameIDs = Data.Sql.Integration.GetParentConsensusNameIDs(pn);
            
            if (parentConsNameIDs.Count > 0)
            {
                for (int i = names.Name.Count - 1; i >= 0; i--)
                {
                    DsNameMatch.NameRow nmRow = names.Name[i];
                    if (!parentConsNameIDs.Contains(nmRow.ParentID)) nmRow.Delete();
                }
            }

            ds.AcceptChanges();
        }
    }
}
