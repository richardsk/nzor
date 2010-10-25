using System.Data.SqlClient;
using System.Data;

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

            ds = NZOR.Data.ConsensusName.GetNamesWithProperty(DBConnection, NZOR.Data.NameProperties.Rank, pn.ProviderName[0].TaxonRank);
            
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
