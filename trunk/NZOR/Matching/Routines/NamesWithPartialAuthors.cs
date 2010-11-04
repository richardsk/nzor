using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithPartialAuthors : BaseMatcher
    {

        public NamesWithPartialAuthors()
        {
        }

        public override DsNameMatch GetMatchingNames(DsIntegrationName.ProviderNameRow pn, ref string matchComments)
        {
            //todo
            return null;
        }

        public override void RemoveNonMatches(DsIntegrationName.ProviderNameRow pn, ref DsNameMatch names, ref string matchComments)
        {
            //try :
            // - matching by levenshtein
            // - matching removing common author bits, eg "ex"
            // - matching on combination authors

            object auth = pn["Authors"];
            object combAuth = pn["CombinationAuthors"];

            if (auth == System.DBNull.Value || auth.ToString().Length == 0) return;
            //succeed 

            String authors = auth.ToString().Trim();
            authors = authors.Replace("  ", " ");
            authors = authors.Replace(" et ", " & ");
            if (authors.IndexOf(" ex ") != -1) authors = authors.Substring(authors.IndexOf(" ex ") + 4);

            
            //2 different match types Levenshtien Words and plain Levenshtein
            //if there are 0 matches for Levenshtien then use the names that matched Levenshtien Words
            List<Guid> wordMatches = new List<Guid>();
            List<Guid> combMatches = new List<Guid>();

            for (int i = names.Name.Count - 1; i >= 0; i--)
            {
                DsNameMatch.NameRow row = names.Name[i];
                String nameAuth = row["Authors"].ToString().Trim();
                nameAuth = nameAuth.Replace("  ", " ");
                nameAuth = nameAuth.Replace(" et ", " & ");
                if (nameAuth.IndexOf(" ex ") != -1) nameAuth = nameAuth.Substring(nameAuth.IndexOf(" ex ") + 4);

                if (Utility.LevenshteinPercent(authors, nameAuth) < Threshold)
                {
                    if (Utility.LevenshteinWordsPercent(authors, nameAuth) >= Threshold)
                    {
                        wordMatches.Add(row.NameID);
                    }
                    else
                    {
                        String provCombAuth = row["CombinationAuthors"].ToString().Trim();
                        provCombAuth = provCombAuth.Replace("  ", " ");
                        provCombAuth = provCombAuth.Replace(" et ", " & ");
                        if (provCombAuth.IndexOf(" ex ") != -1) provCombAuth = provCombAuth.Substring(provCombAuth.IndexOf(" ex ") + 4);

                        if (Utility.LevenshteinPercent(combAuth.ToString(), provCombAuth) >= Threshold)
                        {
                            combMatches.Add(row.NameID);
                        }
                        else if (Utility.LevenshteinWordsPercent(combAuth.ToString(), provCombAuth) >= Threshold)
                        {
                            combMatches.Add(row.NameID);
                        }
                    }
                    row.Delete();
                }
            }

            if (names.Name.Select().Length == 0 && wordMatches.Count > 0)
            {
                //use Levenshtien Word matches
                foreach (DsNameMatch.NameRow r in names.Name)
                {
                    if (wordMatches.Contains((Guid)r["NameID", DataRowVersion.Original]))
                    {
                        r.RejectChanges();
                    }                    
                }
            }
            else if (names.Name.Select().Length == 0 && combMatches.Count > 0)
            {
                //use Levenshtien Word matches
                foreach (DsNameMatch.NameRow r in names.Name)
                {
                    if (combMatches.Contains((Guid)r["NameID", DataRowVersion.Original]))
                    {
                        r.RejectChanges();
                    }
                }
            }
            
            names.AcceptChanges();
        }

    }
}
