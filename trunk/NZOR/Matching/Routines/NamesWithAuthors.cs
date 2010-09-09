using System.Data.SqlClient;
using System.Data;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithAuthors : BaseMatcher
    {
        public NamesWithAuthors()
        {
        }

        public override DsNameMatch GetMatchingNames(DataSet pn)
        {
            return null;
        }

        public override void RemoveNonMatches(DataSet pn, ref DsNameMatch names)
        {
            //TODO :
            // - corrected authors / lookup
            // may need another table on pn dataset for Authors??

            object authors = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NameProperties.Authors);

            if (authors != System.DBNull.Value)
            {
                for (int i = names.Name.Count - 1; i >= 0; i--)
                {
                    DsNameMatch.NameRow row = names.Name[i];
                    if (!row.IsAuthorsNull() && row.Authors.Length > 0 && row.Authors.ToLower().Trim() != authors.ToString().ToLower().Trim())
                    {
                        row.Delete();
                    }
                }

                if (names.Name.Rows.Count == 0)
                {
                    //try prov names 
                    names.RejectChanges();

                    using (SqlConnection cnn = new SqlConnection())
                    {
                        for (int i = names.Name.Count - 1; i >= 0; i--)
                        {
                            DsNameMatch.NameRow row = names.Name[i];
                            if (ConsensusName.HasProviderValue(cnn, row.NameID, NameProperties.Authors, authors) == false)
                            {
                                row.Delete();
                            }
                        }

                        if (cnn.State != ConnectionState.Closed) cnn.Close();
                    }
                }

            }

            names.AcceptChanges();
        }

    }
}