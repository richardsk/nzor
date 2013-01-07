using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Provider
{
    public class ReferenceProperty : Entity
    {
        public Guid ReferencePropertyId { get; set; }

        public Guid ReferencePropertyTypeId { get; set; }
        public string ReferencePropertyType { get; set; }

        public String SubType { get; set; }
        public Int32? Sequence { get; set; }
        public Int32? Level { get; set; }
        public String Value { get; set; }

        public ReferenceProperty()
        {
            ReferencePropertyId = Guid.NewGuid();

            ReferencePropertyTypeId = Guid.Empty;
            ReferencePropertyType = null;

            SubType = null;
            Sequence = null;
            Level = null;
            Value = null;
        }
    }
}
