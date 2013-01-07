using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Common
{
    public class ConceptRelationshipType
    {
        public Guid ConceptRelationshipTypeId { get; set; }

        public String Relationship { get; set; }
        public int? MaxOccurrences { get; set; }
        public int? MinOccurrences { get; set; }

        public ConceptRelationshipType()
        {
            ConceptRelationshipTypeId = Guid.Empty;

            Relationship = String.Empty;
            MaxOccurrences = null;
            MinOccurrences = null;
        }
    }
}
