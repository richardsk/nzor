using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Common
{
    public class TaxonProperty
    {
        public Guid TaxonPropertyId { get; set; }

        public string Class { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public List<TaxonPropertyLookup> TaxonPropertyLookups { get; set; }

        public TaxonProperty()
        {
            TaxonPropertyLookups = new List<TaxonPropertyLookup>();
        }
    }
}
