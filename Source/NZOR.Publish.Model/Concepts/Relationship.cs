using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Common;

namespace NZOR.Publish.Model.Concepts
{
    public class Relationship
    {
        public ConceptLink FromConcept { get; set; }
        public ConceptLink ToConcept { get; set; }

        public string Type { get; set; }
        public bool IsActive { get; set; }

        public List<ProviderLink> InUseByProviders { get; set; }

        public Relationship()
        {
            InUseByProviders = new List<ProviderLink>();
        }
    }
}
