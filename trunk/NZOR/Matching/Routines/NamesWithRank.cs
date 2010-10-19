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

        public override DsNameMatch GetMatchingNames(System.Data.DataSet pn, ref string matchComments)
        {
            object val = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Rank);

            DsNameMatch ds = new DsNameMatch();

            if (val != System.DBNull.Value)
            {
                ds = NZOR.Data.ConsensusName.GetNamesWithProperty(DBConnection, NZOR.Data.NameProperties.Rank, val);
            }

            return ds;
        }

        public override void RemoveNonMatches(System.Data.DataSet pn, ref DsNameMatch names, ref string matchComments)
        {
            object pnRank = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Rank);

            for (int i = names.Name.Count - 1; i >= 0; i--) 
            {
                DsNameMatch.NameRow row = names.Name[i];
                if (row["Rank"].ToString().Trim() != pnRank.ToString().Trim())
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }

    }
}
