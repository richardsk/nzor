using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Consensus
{
    public class Annotation : Entity
    {
        public Guid AnnotationId { get; set; }

        public Guid? NameId { get; set; }
        public Guid? ConceptId { get; set; }
        public Guid? ReferenceId { get; set; }
        
        public string AnnotationType { get; set; }
        public string AnnotationText { get; set; }
                        
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        
        public Annotation()
        {
            AnnotationId = Guid.Empty;

            NameId = null;
            ConceptId = null;
            ReferenceId = null;

            AnnotationType = null;
            AnnotationText = null;
            
            AddedDate = null;
            ModifiedDate = null;

        }

    }
}
