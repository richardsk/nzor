using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Common
{
    public class TaxonPropertyType
    {
        public Guid TaxonPropertyTypeId { get; set; }
        public Guid TaxonPropertyClassId { get; set; }

        public String Name { get; set; }
        public String Description { get; set; }

        public TaxonPropertyType()
        {
            TaxonPropertyTypeId = Guid.Empty;
            TaxonPropertyClassId = Guid.Empty;

            Name = String.Empty;
            Description = String.Empty;
        }
    }
}
