using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Provider
{
    public class ConceptRelationship
    {
        public Guid ConceptRelationshipId { get; set; }

        public Guid? FromConceptId { get; set; }
        public Guid? ToConceptId { get; set; }
        public Guid? ConceptRelationshipTypeId { get; set; }

        public Int32? Sequence { get; set; }
        public Boolean? InUse { get; set; }

        public string ProviderToRecordId { get; set; }

        public ConceptRelationship()
        {
            ConceptRelationshipId = Guid.NewGuid();

            FromConceptId = null;
            ToConceptId = null;
            ConceptRelationshipTypeId = null;

            Sequence = null;
            InUse = null;

            ProviderToRecordId = null;
        }
    }
}
