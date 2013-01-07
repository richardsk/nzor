using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Search.Names
{
    public class NamesSearchResponse : SearchResponse
    {
        public List<NameSearchResult> Results { get; set; }

        public NamesSearchResponse()
        {
            Results = new List<NameSearchResult>();
        }
    }
}
