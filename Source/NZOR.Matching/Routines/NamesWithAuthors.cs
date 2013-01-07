using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

using NZOR.Data;
using NZOR.Data.DataSets;
using NZOR.Data.LookUps.Common;

namespace NZOR.Matching
{
    public class NamesWithAuthors : BaseMatcher
    {
        public NamesWithAuthors()
        {
        }

        public override DsNameMatch GetMatchingNames(DsIntegrationName.ProviderNameRow pn, ref string matchComments)
        {
            return null;
        }

        public override void RemoveNonMatches(DsIntegrationName.ProviderNameRow pn, ref DsNameMatch names, ref string matchComments)
        {
            //TODO :
            // - corrected authors / lookup
            // may need another table on pn dataset for Authors??

            object a = pn["Authors"];
            if (a != System.DBNull.Value)
            {
                string authors = a.ToString();
                authors = authors.Replace("  ", " ");
                authors = authors.Replace(" et ", " & ");
                authors = authors.Replace(" and ", " & ");
                authors = authors.Replace(".", "");
                authors = authors.Replace(",", "");
                if (authors.IndexOf(" ex ") != -1) authors = authors.Substring(authors.IndexOf(" ex ") + 4);

                for (int i = names.Name.Count - 1; i >= 0; i--)
                {
                    DsNameMatch.NameRow row = names.Name[i];

                    if (!row.IsAuthorsNull() && row.Authors.Length > 0)
                    {
                        string authMatch = row["Authors"].ToString();
                        authMatch = authMatch.Replace("  ", " ");
                        authMatch = authMatch.Replace(" et ", " & ");
                        authMatch = authMatch.Replace(" and ", " & ");
                        authMatch = authMatch.Replace(".", "");
                        authMatch = authMatch.Replace(",", "");
                        if (authMatch.IndexOf(" ex ") != -1) authMatch = authMatch.Substring(authMatch.IndexOf(" ex ") + 4);

                        if (authMatch.ToLower().Trim() != authors.ToString().ToLower().Trim())
                        {
                            row.Delete();
                        }
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
                            using (SqlConnection cnn = new SqlConnection(DBConnectionString))
                            {
                                cnn.Open();
                                hasValue = NZOR.Data.Sql.Integration.HasProviderValue(cnn, row.NameID, NamePropertyTypeLookUp.Authors, authors);
                            }
                        }
                        else
                        {
                            MatchData.GetAllDataLock();
                            List<DsIntegrationName.ProviderNameRow> pnList = MatchData.AllData.GetProviderNames(row.NameID);

                            foreach (DsIntegrationName.ProviderNameRow pnr in pnList)
                            {
                                string pnrMatch = row["Authors"].ToString();
                                pnrMatch = pnrMatch.Replace("  ", " ");
                                pnrMatch = pnrMatch.Replace(" et ", " & ");
                                pnrMatch = pnrMatch.Replace(" and ", " & ");
                                pnrMatch = pnrMatch.Replace(".", "");
                                pnrMatch = pnrMatch.Replace(",", "");
                                if (pnrMatch.IndexOf(" ex ") != -1) pnrMatch = pnrMatch.Substring(pnrMatch.IndexOf(" ex ") + 4);

                                if (pnrMatch.ToLower() == authors.ToString().ToLower()) hasValue = true;                            
                            }

                            MatchData.ReleaseAllDataLock();
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