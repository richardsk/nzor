using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Integration
{
    public class ConfigSet
    {
        public List<NZOR.Matching.INameMatcher> Routines = new List<Matching.INameMatcher>();
        public int SetNumber = -1;
    }
}
