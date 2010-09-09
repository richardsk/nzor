using System.Data.SqlClient;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithExactCanonical : BaseMatcher
    {

        public NamesWithExactCanonical()
        {
        }

        public override DsNameMatch GetMatchingNames(DataSet pn)
        {
            return null;
        }

        public override void RemoveNonMatches(DataSet pn, ref DsNameMatch names)
        {
            object pnCanonical = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Canonical);

            for (int i = names.Name.Count - 1; i >= 0; i--)
            {
                DsNameMatch.NameRow row = names.Name[i];
                if (row.Canonical.Trim() != pnCanonical.ToString().Trim())
                {
                    row.Delete();
                }
            }

            names.AcceptChanges();
        }


    }
}
