using System.Data.SqlClient;
using System.Data;
using System;

using NZOR.Data;
using NZOR.Data.DataSets;
using NZOR.Data.LookUps.Common;

namespace NZOR.Matching
{
    public class NamesWithRank : BaseMatcher    
    {

        public NamesWithRank()
        {
        }

        public override DsNameMatch GetMatchingNames(DsIntegrationName.ProviderNameRow pn, ref string matchComments)
        {
            DsNameMatch ds = new DsNameMatch();

            if (UseDBConnection)
            {
                if (!pn.IsTaxonRankIDNull())
                {
                    using (SqlConnection cnn = new SqlConnection(DBConnectionString))
                    {
                        cnn.Open();
                        ds = NZOR.Data.Sql.Integration.GetNamesWithProperty(cnn, NamePropertyTypeLookUp.Rank, pn.TaxonRank);
                    }
                }
            }
            else
            {
                DataRow[] rows = null;

                MatchData.GetConsensusDataLock();
                rows = MatchData.AllData.ConsensusData.ConsensusName.Select("TaxonRank = '" + pn.TaxonRank + "'");
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
            if (!pn.IsTaxonRankIDNull())
            {
                for (int i = names.Name.Count - 1; i >= 0; i--)
                {
                    DsNameMatch.NameRow row = names.Name[i];
                    if (row["Rank"].ToString().Trim() != pn.TaxonRank.ToString().Trim())
                    {
                        row.Delete();
                    }
                }

                names.AcceptChanges();
            }
        }

    }
}
