using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;

using NZOR.Matching;

namespace NZOR.Integration
{
    public class Integrator
    {
        public static Data.MatchResult DoMatch(DataSet provName, List<INameMatcher> routines)
        {
            NZOR.Data.DsNameMatch results = null;
            bool done = false;

            Data.MatchResult res = new NZOR.Data.MatchResult();
            res.MatchPath = ""; // Keep audit trail match path

            INameMatcher nm = routines[0];

            while (!done)
            {
                if (res.MatchPath.Length > 0) res.MatchPath += " : ";
                res.MatchPath += nm.GetType().Name;

                string comments = "";
                NZOR.Data.DsNameMatch tmpRes = null;
                if (results != null) tmpRes = (NZOR.Data.DsNameMatch)results.Copy();

                if (results == null)
                {
                    results = nm.GetMatchingNames(provName, ref comments);
                }
                else
                {
                    nm.RemoveNonMatches(provName, ref results, ref comments);
                }

                //get next matcher 
                int nextId = -1;
                if (results == null || results.Tables[0].Rows.Count == 0)
                {
                    res.MatchPath += " (FAIL)";
                    if (tmpRes != null) res.MatchPath += " (Match Count = " + tmpRes.Tables[0].Rows.Count.ToString() + ")";
                    
                    //revert to last set then go to fail id 
                    if (nm.FailId == -1)
                    {
                        //failed 
                        done = true;
                    }
                    else
                    {
                        results = tmpRes;
                        nextId = nm.FailId;
                    }
                }
                else
                {
                    res.MatchPath += " (PASS)";
                    res.MatchPath += " (Match Count = " + results.Tables[0].Rows.Count.ToString() + ")";
                    
                    nextId = nm.SuccessId;
                }

                if (comments != "") res.MatchPath += " [" + comments + "]";

                if (nextId != -1)
                {
                    done = true; //in case the next one doesnt exist
                    foreach (INameMatcher m in routines)
                    {
                        if (m.Id == nextId)
                        {
                            nm = m;
                            done = false;
                            break; 
                        }
                    }
                }
                else
                {
                    done = true;

                }
            }

            res.MatchPath += " : COMPLETE";

            List<Data.NameMatch> names = new List<Data.NameMatch>();
            if (results != null)
            {
                res.MatchPath += " (Match Count = " + results.Name.Count.ToString() + ")";

                foreach (NZOR.Data.DsNameMatch.NameRow row in results.Name)
                {
                    Data.NameMatch match = new Data.NameMatch();
                    match.NameFull = row.FullName;
                    match.NameId = row.NameID;
                    names.Add(match);
                }
            }

            res.Matches = names;

            return res; 
        }

        public static List<INameMatcher> LoadConfig(XmlDocument configDoc, int matchSetId)
        {
            List<INameMatcher> routines = new List<INameMatcher>();

            XmlNode matchSet = configDoc.SelectSingleNode("//MatchSet[@id=" + matchSetId.ToString() + "]");
            if (matchSet != null)
            {
                XmlNodeList rules = matchSet.SelectNodes("//Rule");

                foreach (XmlNode ruleNode in rules)
                {
                    try
                    {
                        String id = ruleNode.Attributes["id"].Value;
                        String asm = ruleNode.Attributes["assembly"].Value;
                        String cls = ruleNode.Attributes["class"].Value;
                        String thr = ruleNode.Attributes["threshold"].Value;
                        String pass = ruleNode.Attributes["pass"].Value;
                        String fail = ruleNode.Attributes["fail"].Value;

                        String fname = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), asm);
                        INameMatcher nm = (INameMatcher)Activator.CreateInstanceFrom(fname, cls).Unwrap();
                        
                        nm.Id = Int32.Parse(id);

                        int p = -1;
                        int f = -1;
                        int t = 100;
                        if (!Int32.TryParse(thr, out t)) t = 100;
                        nm.Threshold = t;
                        if (!Int32.TryParse(pass, out p)) p = -1;
                        nm.SuccessId = p;
                        if (!Int32.TryParse(fail, out f)) f = -1;
                        nm.FailId = f;

                        routines.Add(nm);
                    }
                    catch (Exception ex)
                    {
                        Log.LogError(ex);
                    }

                }
            }

            return routines;
        }
    }
}
