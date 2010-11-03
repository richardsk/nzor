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

        public override DsNameMatch GetMatchingNames(DsIntegrationName pn, ref string matchComments)
        {
            return null;
        }

        public override void RemoveNonMatches(DsIntegrationName pn, ref DsNameMatch names, ref string matchComments)
        {
            //TODO :
            // - corrected authors / lookup
            // may need another table on pn dataset for Authors??

            object authors = pn.ProviderName[0]["Authors"];
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

                    for (int i = names.Name.Count - 1; i >= 0; i--)
                    {
                        DsNameMatch.NameRow row = names.Name[i];
                        bool hasValue = false;

                        if (UseDBConnection)
                        {
                            hasValue = ConsensusName.HasProviderValue(DBConnection, row.NameID, NameProperties.Authors, authors);
                        }
                        else
                        {
                            lock (MatchData.DataForIntegration)
                            {
                                DataRow[] res = MatchData.DataForIntegration.ProviderName.Select("ConsensusNameID = '" + row.NameID.ToString() + "' and Authors = '" + authors + "'");
                                hasValue = (res.Length > 0);
                            }
                        }

                        if (!hasValue)
                        {
                            row.Delete();
                        }
                    }
                }
            }

            names.AcceptChanges();
        }

    }
}