using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Consensus
{
    public class ReferenceProperty : Entity
    {
        public Guid ReferencePropertyId { get; set; }
        public Guid ReferencePropertyTypeId { get; set; }
        public String ReferencePropertyType { get; set; }        
        public String Value { get; set; }

        public ReferenceProperty()
        {
            ReferencePropertyId = Guid.NewGuid();

            ReferencePropertyTypeId = Guid.Empty;
            ReferencePropertyType = string.Empty;

            Value = null;
        }
    }
}
