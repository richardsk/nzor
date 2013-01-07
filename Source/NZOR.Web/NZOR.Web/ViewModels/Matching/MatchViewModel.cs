using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NZOR.Publish.Model.Matching;

namespace NZOR.Web.ViewModels.Matching
{
    public class MatchViewModel
    {
        public List<NameMatchResult> NameMatchResults { get; set; }

        public MatchViewModel()
        {
            NameMatchResults = new List<NameMatchResult>();
        }
    }
}