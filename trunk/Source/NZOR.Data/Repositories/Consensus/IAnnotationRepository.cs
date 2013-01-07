using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Consensus;

namespace NZOR.Data.Repositories.Consensus
{
    public interface IAnnotationRepository
    {
        List<Annotation> Annotations { get; }

        Annotation GetAnnotation(Guid annotationId);
        List<Annotation> GetAnnotationsByName(Guid nameId);
        List<Annotation> GetAnnotationsByConcept(Guid conceptId);

        void Save();
    }
}
