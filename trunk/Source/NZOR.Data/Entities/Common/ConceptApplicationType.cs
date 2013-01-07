using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Common
{
    public class ConceptApplicationType
    {
        public Guid ConceptApplicationTypeId { get; set; }

        public String Name { get; set; }

        public ConceptApplicationType()
        {
            ConceptApplicationTypeId = Guid.Empty;

            Name = String.Empty;
        }
    }
}
