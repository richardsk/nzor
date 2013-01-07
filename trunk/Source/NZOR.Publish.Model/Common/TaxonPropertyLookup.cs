using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Common
{
    public class TaxonPropertyLookup
    {
        public Guid TaxonPropertyLookupId { get; set; }

        public Guid? ParentTaxonPropertyLookupId { get; set; }

        public string Value { get; set; }
        public string AlternativeStrings { get; set; }
    }
}
