using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Provider
{
    public class Annotation : Entity
    {
        public Guid AnnotationId { get; set; }
        
        public Guid DataSourceId { get; set; }
        public Guid? ConsensusAnnotationId { get; set; }

        public string AnnotationType { get; set; }
        public string AnnotationText { get; set; }

        public string ProviderRecordId { get; set; }
        public DateTime? ProviderCreatedDate { get; set; }
        public DateTime? ProviderModifiedDate { get; set; }

        public string ProviderReferenceId { get; set; }
        public Guid? ReferenceId { get; set; }
        //public string ReferenceCitation { get; set; }

        public string ProviderConceptId { get; set; }
        public Guid? ConceptId { get; set; }

        public string ProviderNameId { get; set; }
        public Guid? NameId { get; set; }
        //public string FullName { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public Annotation()
        {
            AnnotationId = Guid.Empty;

            ConsensusAnnotationId = null;
            DataSourceId = Guid.Empty;

            NameId = null;
            //FullName = null;
            ConceptId = null;
            ReferenceId = null;
            //ReferenceCitation = null;

            AnnotationType = null;
            AnnotationText = null;

            ProviderConceptId = null;
            ProviderNameId = null;
            ProviderRecordId = null;
            ProviderReferenceId = null;

            AddedDate = null;
            ModifiedDate = null;

        }

    }
}
