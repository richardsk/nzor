using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Common
{
    public class Annotation
    {
        public Guid AnnotationId { get; set; }
        public string AnnotationType { get; set; }
        public string AnnotationText { get; set; }

        public Annotation()
        {
            AnnotationType = string.Empty;
            AnnotationText = string.Empty;
        }
    }
}
