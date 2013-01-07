using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Provider;

namespace NZOR.Data.Repositories.Provider
{
    public interface IAnnotationRepository
    {
        List<Annotation> Annotations { get; }

        Annotation GetAnnotation(Guid annotationId);
        List<Annotation> GetAnnotationsByName(Guid nameId);
        List<Annotation> GetAnnotationsByConcept(Guid conceptId);
        List<Annotation> GetAnnotations(Guid dataSourceId);

        void Save();
    }
}
