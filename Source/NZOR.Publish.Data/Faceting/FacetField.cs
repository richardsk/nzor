using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Data.Faceting
{
    class FacetField
    {
        public string FieldName { get; set; }
        public string DisplayName { get; set; }

        public List<Facet> Facets { get; private set; }

        public FacetField()
        {
            FieldName = String.Empty;
            DisplayName = String.Empty;

            Facets = new List<Facet>();
        }
    }
}
