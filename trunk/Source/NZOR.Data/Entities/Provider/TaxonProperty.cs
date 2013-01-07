using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Provider
{
    public class TaxonProperty : Entity
    {
        public Guid TaxonPropertyId { get; set; }

        public Guid TaxonPropertyClassId { get; set; }
        public string TaxonPropertyClass { get; set; }

        public Guid DataSourceId { get; set; }

        public Guid? ConsensusTaxonPropertyId { get; set; }
        public string LinkStatus { get; set; }
        public int? MatchScore { get; set; }

        public string ProviderRecordId { get; set; }
        
        public string ProviderReferenceId { get; set; }
        public Guid? ReferenceId { get; set; }
        public string ReferenceCitation { get; set; }

        public string ProviderConceptId { get; set; }
        public Guid? ConceptId { get; set; }

        public string ProviderNameId { get; set; }
        public Guid? NameId { get; set; }
        public string FullName { get; set; }

        public bool? InUse { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public DateTime? ProviderCreatedDate { get; set; }
        public DateTime? ProviderModifiedDate { get; set; }

        public List<TaxonPropertyValue> TaxonPropertyValues { get; set; }

        public TaxonProperty() 
        {
            TaxonPropertyValues = new List<TaxonPropertyValue>();

            TaxonPropertyId = Guid.Empty;
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
            TaxonPropertyId = Guid.Empty;
            TaxonPropertyTypeId = Guid.Empty;
            TaxonPropertyType = null; 
            Value = null;

        }
    }
}
