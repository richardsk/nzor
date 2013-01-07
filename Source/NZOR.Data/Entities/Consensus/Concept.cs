using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NZOR.Data.Entities.Consensus
{
    [DebuggerDisplay("Id: {ConceptId};")]
    public class Concept : Entity
    {
        public Guid ConceptId { get; set; }

        public Guid NameId { get; set; }
        public Guid? AccordingToReferenceId { get; set; }
        public String AccordingToReference { get; set; }
        
        public String Orthography { get; set; }
        public String TaxonRank { get; set; }
        public String HigherClassification { get; set; }
        public String NameQualifier { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public List<ConceptApplication> ConceptApplications { get; private set; }
        public List<ConceptRelationship> ConceptRelationships { get; private set; }
        
        public Concept()
        {
            ConceptId = Guid.Empty;

            NameId = Guid.Empty;
            AccordingToReferenceId = null;
            
            Orthography = null;
            TaxonRank = null;
            HigherClassification = null;
            NameQualifier = null;

            AddedDate = null;
            ModifiedDate = null;

            ConceptApplications = new List<ConceptApplication>();
            ConceptRelationships = new List<ConceptRelationship>();

        }
    }
}
