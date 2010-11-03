using System.Data.SqlClient;
using System.Data;
using System;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithRank : BaseMatcher    
    {

        public NamesWithRank()
        {
        }

        public override DsNameMatch GetMatchingNames(DsIntegrationName pn, ref string matchComments)
        {
            DsNameMatch ds = new DsNameMatch();

            if (UseDBConnection)
            {
                ds = NZOR.Data.ConsensusName.GetNamesWithProperty(DBConnection, NZOR.Data.NameProperties.Rank, pn.ProviderName[0].TaxonRank);
            }
            else
            {
                DataRow[] rows = null;
                lock (MatchData.DataForIntegration)
                {
                    rows = MatchData.DataForIntegration.ConsensusName.Select("TaxonRank = '" + pn.ProviderName[0].TaxonRank + "'");
                }
                foreach (DataRow row in rows)
                {
                    ds.Name.AddNameRow((Guid)row["NameID"],
                                row["Canonical"].ToString(),
                                row["FullName"].ToString(),
                                row["TaxonRank"].ToString(),
                                row["Authors"].ToString(),
                                row["CombinationAuthors"].ToString(),
                                row["YearOnPublication"].ToString(),
                                (Guid)row["ParentID"],
                                100);
                }
                    
            }
            
            return ds;
        }

        public override void RemoveNonMatches(DsIntegrationName pn, ref DsNameMatch names, ref string matchComments)
        {
            for (int i = names.Name.Count - 1; i >= 0; i--) 
            {
                DsNameMatch.NameRow row = names.Name[i];
                if (row["Rank"].ToString().Trim() != pn.ProviderName[0].TaxonRank.ToString().Trim())
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }

    }
}
