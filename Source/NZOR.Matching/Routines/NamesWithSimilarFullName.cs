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
    public class NamesWithSimilarFullName : BaseMatcher
    {
        public NamesWithSimilarFullName()
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
                        cmd.CommandText = @"select n.NameId, np.Value as FullName from consensus.name n                            
                            inner join consensus.NameProperty np on np.NameID = n.NameID
                            inner join dbo.NamePropertyType npt on npt.NamePropertyTypeID = np.NamePropertyTypeID and npt.Name = 'NameText_FullName'
                            where n.nameclassid = '" + pn.NameClassID.ToString() + "'";

                        DataSet namesDs = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(namesDs);

                        string namesIdList = "(";

                        foreach (DataRow dr in namesDs.Tables[0].Rows)
                        {
                            if (NZOR.Matching.Utility.LevenshteinWordsPercent(dr["FullName"].ToString(), pn.FullName) > Threshold)
                            {
                                namesIdList += "'" + dr["NameId"].ToString() + "',";
                            }
                        }

                        namesIdList = namesIdList.TrimEnd(',');
                        namesIdList += ")";

                        if (namesIdList.Length > 2)
                        {
                            cmd.CommandText = @";                            
                            select n.* 
                            from consensus.Name n 
                            where nameid in " + namesIdList + @";
                            
                            select np.*, ncp.Name 
                            from consensus.NameProperty np  
                            inner join dbo.NamePropertyType ncp on ncp.NamePropertyTypeID = np.NamePropertyTypeID
                            where np.nameid in " + namesIdList + @";
                                            
                            select c.* 
                            from consensus.vwConcepts c 
                            where c.nameid in " + namesIdList + " and c.IsActive = 1;";

                            cmd.CommandTimeout = 500000;

                            DataSet res = new DataSet();
                            da = new SqlDataAdapter(cmd);
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
            }
            else
            {
                MatchData.GetConsensusDataLock();

                foreach (DsConsensusData.ConsensusNameRow cnRow in MatchData.AllData.ConsensusData.ConsensusName)
                {
                    double perc = Utility.LevenshteinPercent(cnRow["FullName"].ToString().Trim(), pn["FullName"].ToString().Trim());

                    if (perc > Threshold)
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
                double perc = Utility.LevenshteinPercent(row["FullName"].ToString().Trim(), pn["FullName"].ToString().Trim());
                if (perc < Threshold)
                {
                    row.Delete();
                }
                else
                {
                    //update match score
                    row.PercentMatch = (int)perc;
                }
            }

            names.AcceptChanges();
        }

    }
}
