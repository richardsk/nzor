using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Consensus
{
    public class ConceptRelationship
    {
        public Guid ConceptRelationshipId { get; set; }

        public Guid? FromConceptId { get; set; }
        public Guid? ToConceptId { get; set; }
        public Guid? NameToId { get; set; }
        public string NameTo { get; set; }

        public Guid? ConceptRelationshipTypeId { get; set; }
        public string ConceptRelationshipType { get; set; }

        public bool IsActive { get; set; }
        public Int32? Sequence { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
                
        public ConceptRelationship()
        {
            ConceptRelationshipId = Guid.NewGuid();

            FromConceptId = null;
            ToConceptId = null;
            ConceptRelationshipTypeId = null;

            ConceptRelationshipType = null; 

            IsActive = false;
            Sequence = null;

            AddedDate = null;
            ModifiedDate = null;
        }
    }
}
