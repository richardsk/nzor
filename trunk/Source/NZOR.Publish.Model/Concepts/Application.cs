using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Concepts
{
    public class Application
    {
        public string Type { get; set; }
        public string Gender { get; set; }
        public string PartOfTaxon { get; set; }
        public string LifeStage { get; set; }

        public ConceptLink FromConcept { get; set; }
        public ConceptLink ToConcept { get; set; }

        public Application()
        {
            Type = String.Empty;
            Gender = String.Empty;
            PartOfTaxon = String.Empty;
            LifeStage = String.Empty;
        }
    }
}
