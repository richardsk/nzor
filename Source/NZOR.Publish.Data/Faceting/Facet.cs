using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Util;

namespace NZOR.Publish.Data.Faceting
{
    class Facet
    {
        public string Value { get; set; }
        public OpenBitSetDISI BitSet { get; set; }
    }
}
