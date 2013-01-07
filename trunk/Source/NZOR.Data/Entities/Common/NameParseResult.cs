using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Common
{
    public class NamePart
    {
        public string Text = "";
        public string Rank = "";
        public string Authors = "";

        public void AddAuthors(string authors)
        {
            if (Authors.Length > 0) Authors += " ";
            Authors += authors;
        }
    }

    public enum NameParseOutcome
    {
        Unknown,
        Succeeded,
        Failed
    }

    public class NameParseResult
    {
        public String Id = "";
        public String OriginalNameText = "";
        public List<NamePart> NameParts = new List<NamePart>();        
        public string Year = "";
        public bool IsHybrid = false;

        public NameParseOutcome Outcome = NameParseOutcome.Unknown;
        public String Error = "";

        /// <summary>
        /// Add authors.  If for a newPart then create another author entry, otherwise concatenate existing one.
        /// </summary>
        /// <param name="val"></param>
        //public void AddAuthors(string val, bool newPart)
        //{
        //    if (newPart)
        //    {
        //        Authors.Add(val);
        //    }
        //    else
        //    {
        //        if (Authors.Count == 0) Authors.Add("");
        //        else Authors[Authors.Count - 1] += " ";

        //        Authors[Authors.Count - 1] += val;
        //    }
        //}

        /// <summary>
        /// Get the main authors.
        /// </summary>
        /// <returns></returns>
        public string GetAuthors()
        {
            if (NameParts.Count == 0) return "";
            return NameParts[NameParts.Count - 1].Authors;
        }

        public void AddPart(string text, string rank, string authors)
        {
            NamePart np = new NamePart();
            np.Text = text;
            np.Rank = rank;
            np.Authors = authors;
            NameParts.Add(np);
        }

        public NamePart GetPart(string rank)
        {
            foreach (NamePart np in NameParts) if (np.Rank.Equals(rank, StringComparison.InvariantCultureIgnoreCase)) return np;
            return null;
        }

        /// <summary>
        /// Gets the "main" part of the name - ie the last specific/infraspecific part
        /// </summary>
        /// <returns></returns>
        public NamePart GetCanonical()
        {
            return NameParts[NameParts.Count - 1];
        }

        /// <summary>
        /// Gets the parent of the canonical part of the name
        /// </summary>
        /// <returns></returns>
        public NamePart GetParent()
        {
            if (NameParts.Count > 1) return NameParts[NameParts.Count - 2];
            return null;
        }

        public string GetFullName(Data.LookUps.Common.TaxonRankLookUp rankLookup, bool includeAuthors, string governingCode)
        {
            string name = "";
            foreach (NamePart p in NameParts)
            {
                if (p.Rank == "hybrid")
                {
                    if (includeAuthors && p.Authors != "")
                    {
                        name += p.Authors + " ";
                    }
                }

                TaxonRank tr = rankLookup.GetTaxonRank(p.Rank, governingCode);
                if (tr == null)
                {
                    name += p.Text + " ";

                    if (includeAuthors && p.Authors != "")
                    {
                        name += p.Authors + " ";
                    }
                }
                else if (!tr.IncludeInFullName.HasValue || tr.IncludeInFullName.Value)
                {
                    if (tr.ShowRank.HasValue && tr.ShowRank.Value)
                    {
                        name += tr.Name + " ";
                    }
                    name += p.Text + " ";

                    if (includeAuthors && p.Authors != "")
                    {
                        name += p.Authors + " ";
                    }
                }
            }
            
            name = name.Trim();

            return name;
        }

    }

    public class NameParseResultCollection : List<NameParseResult>
    {
        public List<NameParseResult> ResultsWithErrors()
        {
            List<NameParseResult> errRes = new List<NameParseResult>();

            foreach (NameParseResult npr in this)
            {
                if (npr.Outcome == NameParseOutcome.Failed) errRes.Add(npr);
            }

            return errRes;
        }
    }
}
