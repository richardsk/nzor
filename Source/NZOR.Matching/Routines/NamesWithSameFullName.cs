using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using NZOR.Data;
using NZOR.Data.DataSets;
using NZOR.Data.LookUps.Common;


namespace NZOR.Matching
{
    class NamesWithSameFullName : BaseMatcher
    {
        public NamesWithSameFullName()
        {
        }

        public override DsNameMatch GetMatchingNames(DsIntegrationName.ProviderNameRow pn, ref string matchComments)
        {
            DsNameMatch ds = new DsNameMatch();
            List<DataRow> rows = new List<DataRow>();

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
                            inner join consensus.NameProperty np on np.NameID = n.NameID
                            inner join dbo.NamePropertyType npt on npt.NamePropertyTypeID = np.NamePropertyTypeID and npt.Name = 'NameText_FullName'
                            where n.nameclassid = '" + pn.NameClassID.ToString() + "' and np.Value = '" + pn.FullName.Replace("'", "''") + "';";

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
                MatchData.GetConsensusDataLock();

                foreach (DsConsensusData.ConsensusNameRow cnRow in MatchData.AllData.ConsensusData.ConsensusName)
                {
                    if (cnRow["FullName"].ToString().Equals(pn.FullName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        rows.Add(cnRow);
                    }
                }

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

            return ds;
        }

        public override void RemoveNonMatches(DsIntegrationName.ProviderNameRow pn, ref DsNameMatch names, ref string matchComments)
        {
            for (int i = names.Name.Count - 1; i >= 0; i--)
            {
                DsNameMatch.NameRow row = names.Name[i];
                if (row["FullName"].ToString().Trim() != pn["FullName"].ToString().Trim())
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }

    }
}
