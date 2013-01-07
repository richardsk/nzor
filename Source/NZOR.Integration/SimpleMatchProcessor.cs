using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using NZOR.Matching;
using NZOR.Data.Entities.Common;

namespace NZOR.Integration
{
    public class SimpleMatchProcessor
    {
        public int Progress = 0;
        public int NameCount = 0;

        /// <summary>
        /// Process names in the list of NameParsedResult - matching StackedNames by parent/rank combinations.
        /// </summary>
        /// <param name="cnnStr"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        //public List<MatchResult> DoNameMatching(String cnnStr, NameParseResultCollection data)
        //{
        //    List<MatchResult> results = new List<MatchResult>();
        //    NameCount = data.Count;

        //    Progress = 0;
        //    int pos = 0;
            
        //    foreach (NameParseResult name in data)
        //    {
        //        MatchResult mr = new MatchResult();
        //        mr.ProviderNameFull = name.OriginalNameText;
        //        mr.ProviderRecordId = name.Id;

        //        if (name.Outcome == NameParseOutcome.Failed)
        //        {
        //            mr.Error = "Error parsing provided name.";
        //            mr.Status = Data.LinkStatus.DataFail;
        //        }
        //        else
        //        {
        //            try
        //            {
        //                //check exact match first
        //                DataTable resDt = NZOR.Data.Sql.Matching.GetFullNameMatches(cnnStr, name.OriginalNameText);
        //                //try standard stacked name match
        //                if (resDt == null || resDt.Rows.Count == 0) resDt = NZOR.Data.Sql.Matching.GetStackedNameMatches(cnnStr, name, false);
        //                //check using levenshtein if none found (not for genera)
        //                if ((resDt == null || resDt.Rows.Count == 0) && name.NameParts.Count > 1) resDt = NZOR.Data.Sql.Matching.GetStackedNameMatches(cnnStr, name, true);

        //                if (resDt != null)
        //                {
        //                    if (resDt.Rows.Count == 0)
        //                    {
        //                        mr.Status = Data.LinkStatus.Inserted;
        //                    }
        //                    else if (resDt.Rows.Count == 1)
        //                    {
        //                        mr.Status = Data.LinkStatus.Matched;
        //                        mr.MatchedId = resDt.Rows[0]["NameID"].ToString();
        //                        mr.MatchedName = resDt.Rows[0]["FullName"].ToString();
        //                        MatchInstance mi = new MatchInstance();
        //                        mi.DisplayText = resDt.Rows[0]["FullName"].ToString();
        //                        mi.Id = resDt.Rows[0].Field<Guid?>("NameID");

        //                        mr.Matches.Add(mi);
        //                    }
        //                    else
        //                    {
        //                        mr.Status = Data.LinkStatus.Multiple;
        //                        foreach (DataRow row in resDt.Rows)
        //                        {
        //                            MatchInstance mi = new MatchInstance();
        //                            mi.DisplayText = row["FullName"].ToString();
        //                            mi.Id = row.Field<Guid?>("NameID");
        //                            mr.Matches.Add(mi);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    mr.Status = Data.LinkStatus.Inserted;                            
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                mr.Status = Data.LinkStatus.DataFail;
        //                mr.Error = "Failed to match name : " + ex.Message;
        //            }
        //        }

        //        results.Add(mr);

        //        pos++;
        //        Progress = pos * 100 / data.Count;
        //    }

        //    Progress = 100;

        //    return results;
        //}

    }
}
