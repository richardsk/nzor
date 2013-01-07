using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Names;
using NZOR.Publish.Model.References;

namespace NZOR.Publish.Model.Concepts
{
    public class ConceptLink
    {
        public Guid ConceptId { get; set; }

        public NameLink Name { get; set; }
        public ReferenceLink Publication { get; set; }
    }
}
