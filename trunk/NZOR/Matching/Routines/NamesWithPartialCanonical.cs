using System.Data.SqlClient;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithPartialCanonical : BaseMatcher
    {

        public NamesWithPartialCanonical()
        {
        }


        public override DsNameMatch GetMatchingNames(System.Data.DataSet pn)
        {
            //todo
            return null;
        }

        public override void RemoveNonMatches(System.Data.DataSet pn, ref DsNameMatch names)
        {
            System.String pnCanonical = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Canonical).ToString();

            for (int i = names.Name.Count - 1; i >= 0; i--)
            {
                DsNameMatch.NameRow row = names.Name[i];
                if (Utility.LevenshteinPercent(row.Canonical.Trim(), pnCanonical.Trim()) < Threshold)
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }

    }
}
