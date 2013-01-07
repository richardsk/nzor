using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Consensus
{
    public class TaxonProperty : Entity
    {
        public Guid TaxonPropertyId { get; set; }

        public Guid TaxonPropertyClassId { get; set; }
        public string TaxonPropertyClass { get; set; }
        
        public Guid? GeoRegionId { get; set; }
        public string GeoRegion { get; set; }
        public Guid? ReferenceId { get; set; }
        public string ReferenceCitation { get; set; }

        public Guid? ConceptId { get; set; }
        public Guid? NameId { get; set; }

        public bool? InUse { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public List<TaxonPropertyValue> TaxonPropertyValues { get; set; }

        public TaxonProperty()
        {
            TaxonPropertyId = Guid.NewGuid();

            TaxonPropertyValues = new List<TaxonPropertyValue>();
        }
    }

    public class TaxonPropertyValue
    {
        public Guid TaxonPropertyValueId { get; set; }
        public Guid TaxonPropertyId { get; set; }
        public Guid TaxonPropertyTypeId { get; set; }
        public string TaxonPropertyType { get; set; }

        public string Value { get; set; }

        public TaxonPropertyValue()
        {
            TaxonPropertyValueId = Guid.NewGuid();
        }
    }
}
