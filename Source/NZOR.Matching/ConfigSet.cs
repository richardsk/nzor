using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Matching
{
    public class ConfigSet
    {
        public List<NZOR.Matching.INameMatcher> Routines = new List<Matching.INameMatcher>();
        public int SetNumber = -1;

        public INameMatcher GetRoutine(string matchRuleName)
        {
            INameMatcher nm = null;

            foreach (INameMatcher r in Routines)
            {
                if (r.GetType().ToString().ToLower() == matchRuleName.ToLower())
                {
                    nm = r;
                    break;
                }
            }

            return nm;
        }
    }
}
