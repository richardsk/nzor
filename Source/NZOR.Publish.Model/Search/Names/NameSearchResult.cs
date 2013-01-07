using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Names;

namespace NZOR.Publish.Model.Search.Names
{
    public class NameSearchResult
    {
        public decimal Score { get; set; }

        public Name Name { get; set; }
    }
}
