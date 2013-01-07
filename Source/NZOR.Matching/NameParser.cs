using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Data.Entities.Common;

namespace NZOR.Matching
{
    public class NameParser
    {

        private static bool IsAuthor(string val)
        {
            if (val.Length == 0) return false;

            bool isA = false;

            //upper case letter, or "ex" or "&" or "and" or "et" or "("
            if ((val[0].ToString().ToUpper().Equals(val[0].ToString(), StringComparison.InvariantCulture) ||
                val.ToUpper() == "EX" ||
                val == "&" ||
                val.ToUpper() == "AND" ||
                val.ToUpper() == "ET" ||
                val[0].ToString().ToUpper() == "(")
                && val[0] != '×')
            {
                isA = true;
            }

            return isA;
        }

        private static bool IsHybrid(List<string> parts)
        {
            bool isH = false;

            foreach (string p in parts)
            {
                isH |= IsHybrid(p);
            }

            return isH;
        }

        private static bool IsHybrid(string val)
        {
            if (val.Trim() == "X" || val.Trim() == "x" || val.Trim() == "×") return true;

            return false;
        }

        private static bool IsYear(string val)
        {
            if (val.Length == 0) return false;

            bool isY = true;
            bool hasDigit = false;
            foreach (char c in val)
            {
                if (char.IsDigit(c))
                {
                    hasDigit = true;
                }
                else if (c != '(' && c != ')')
                {
                    isY = false;
                    break;
                }
            }
            if (!hasDigit) isY = false;

            return isY;
        }

        /// <summary>
        /// Splits up the name into parts.  Assumes the names are at genus or species level or below, with infraspecific ranks specified, and with or without authors.
        /// </summary>
        /// <param name="nameString"></param>
        /// <returns></returns>
        public static NameParseResult ParseName(string nameString, NZOR.Data.LookUps.Common.TaxonRankLookUp trLookup)
        {
            NameParseResult res = new NameParseResult(); 
            res.OriginalNameText = nameString;

            try
            {
                List<string> parts = new List<string>();

                nameString = nameString.Replace("'", "");
                nameString = nameString.Replace("  ", " ");
                nameString = nameString.Replace(" x ", " ×"); //hybrid

                int startPos = -1;
                int lastPos = -1;
                if (nameString.Contains("\"")) //break up by quotes instead??
                {
                    for (int i = 0; i < nameString.Length; i++)
                    {
                        char c = nameString[i];
                        if (c == '"')
                        {
                            if (startPos != -1)
                            {
                                parts.Add(nameString.Substring(startPos + 1, i - startPos - 1));
                                lastPos = i + 1;
                                startPos = -1;
                            }
                            else
                            {
                                startPos = i;
                                if (parts.Count == 0 && startPos > 0) parts.Add(nameString.Substring(0, startPos));
                            }
                        }

                    }
                    if (lastPos != -1 && lastPos < nameString.Length) parts.Add(nameString.Substring(lastPos).Trim());
                }
                else
                {
                    parts.AddRange(nameString.Split(' '));
                }

                for (int i = 0; i < parts.Count; i++)
                {
                    parts[i] = parts[i].Trim(',', ' ');
                }

                if (parts.Count == 1)
                {
                    //just a genus
                    res.AddPart(parts[0], "genus", "");
                }
                else if (parts.Count == 2)
                {
                    //could be genus+species, or genus+authors, or genus+year
                    if (IsYear(parts[1]))
                    {
                        res.AddPart(parts[0], "genus", "");
                        res.Year = parts[1].Trim('(', ')');
                    }
                    else if (IsAuthor(parts[1]))
                    {
                        //genus + author as second word has upper case letter
                        res.AddPart(parts[0], "genus", parts[1]);                        
                    }
                    else
                    {
                        res.AddPart(parts[0], "genus", "");
                        res.AddPart(parts[1], "species", "");
                    }
                }
                else if (parts.Count >= 3)
                {
                    //could be genus+species+authors or genus+authors+more authors+more authors ...
                    //assume first part is genus, then the next part with uppercase letter will be start of authors
                    //could also be a hybrid formula - check for X 

                    if (IsHybrid(parts))
                    {
                        ParseHybridFormula(parts, res);
                    }
                    else
                    {
                        res.AddPart(parts[0], "genus", "");
                        string infraspRank = "";
                        string infraspRank2 = "";

                        for (int remPos = 1; remPos < parts.Count; remPos++)
                        {
                            string p = parts[remPos];

                            if (IsYear(p))
                            {
                                res.Year = p.Trim('(', ')');
                            }
                            else if (IsAuthor(p))
                            {
                                res.NameParts[res.NameParts.Count - 1].AddAuthors(p);
                            }
                            else
                            {
                                string rnk = "species";
                                if (res.NameParts.Count == 1) //up to species
                                {
                                    res.AddPart(p, rnk, "");
                                }
                                else if (res.NameParts.Count == 2) //next is the infrasp. rank 
                                {
                                    if (res.GetAuthors() != "" && (p == "de" || p == "van" || p == "la" || p == "der")) //a middle word of the author string - add to last part authors
                                    {
                                        res.NameParts[1].AddAuthors(p);
                                        remPos++;
                                        if (remPos < parts.Count) res.NameParts[1].AddAuthors(parts[remPos]);
                                    }
                                    else if (infraspRank == "")
                                    {
                                        if (p == "sens" || p == "sensu" || p == "sens." || p == "x" || p == "×" || p == "aff." || p == "aff") //hybrid or sensu, add to last name part
                                        {
                                            //rest is sensu
                                            res.NameParts[1].Text += " " + p;
                                            remPos++;
                                            while (remPos < parts.Count)
                                            {
                                                res.NameParts[1].Text += " " + parts[remPos];
                                                remPos++;
                                            }
                                        }
                                        else if (trLookup.GetTaxonRank(p, null) == null)
                                        {
                                            //not a rank, must be the infrasp (ICZN)
                                            res.AddPart(p, "", "");
                                        }
                                        else infraspRank = p;
                                    }
                                    else
                                    {
                                        //already got infrasp rank
                                        res.AddPart(p, infraspRank, "");
                                    }
                                }
                                else if (res.NameParts.Count == 3)
                                {
                                    if (infraspRank2 == "")
                                    {
                                        if (p == "sens" || p == "sensu" || p == "sens." || p == "x" || p == "×" || p == "aff." || p == "aff") //hybrid or sensu, add to last name part
                                        {
                                            res.NameParts[2].Text += " " + p;
                                            remPos++;
                                            while (remPos < parts.Count)
                                            {
                                                res.NameParts[2].Text += " " + parts[remPos];
                                                remPos++;
                                            }
                                        }
                                        else if (trLookup.GetTaxonRank(p, null) == null)
                                        {
                                            //not a rank, must be the infrasp (ICZN)
                                            res.AddPart(p, "", "");
                                        }
                                        else infraspRank2 = p;
                                    }
                                    else
                                    {
                                        //already got infrasp rank 2
                                        res.AddPart(p, infraspRank2, "");
                                    }
                                }
                                else
                                {
                                    //cant be any more???
                                    res.Outcome = NameParseOutcome.Failed;
                                    res.Error = "Failed to parse name string";
                                }
                            }
                        }
                    }
                }

                //fix author brackets
                if (res.GetAuthors().StartsWith("(") && !res.GetAuthors().EndsWith(")"))
                {
                    res.NameParts[res.NameParts.Count - 1].Authors += ")";
                }

                res.Outcome = NameParseOutcome.Succeeded;
            }
            catch (Exception ex)
            {
                res.Outcome = NameParseOutcome.Failed;
                res.Error = ex.Message;
            }

            return res;
        }

        private static void ParseHybridFormula(List<string> parts, NameParseResult res)
        {
            res.IsHybrid = true;

            bool upToSpecies = false;
            string infraSpRank = "";
            
            foreach (string p in parts)
            {
                if (res.NameParts.Count == 0)
                {
                    res.AddPart(p, "genus", "");
                    upToSpecies = true;
                }
                else if (IsHybrid(p))
                {
                    res.AddPart(p, "hybrid", "");
                    upToSpecies = true; //next part has to be genus or species                    
                }
                else if (p == res.NameParts[0].Text.Replace("(","")[0] + "." || p == res.NameParts[0].Text.Replace("(","")) //abbrev of genus or genus again
                {
                    res.AddPart(p, "genus", "");
                    upToSpecies = true;
                }
                else if (IsAuthor(p))
                {
                    res.NameParts[res.NameParts.Count - 1].AddAuthors(p);
                }
                else if (upToSpecies)
                {
                    //could be genus or species
                    if (p[0] == p.ToUpper()[0])
                    {
                        res.AddPart(p, "genus", "");
                    }
                    else
                    {
                        res.AddPart(p, "species", "");
                        upToSpecies = false;
                    }
                }
                else
                {
                    //infra specific
                    if (infraSpRank == "")
                    {
                        if (p == "sens" || p == "sens." || p == "x" || p == "×" || p == "aff." || p == "aff") //hybrid or sensu, add to last name part
                        {
                            res.NameParts[2].Text += " " + p;
                        }
                        else infraSpRank = p;
                    }
                    else
                    {
                        //already got infrasp rank 2
                        res.AddPart(p, infraSpRank, "");
                    }
                }
            }
        }
    }
}
