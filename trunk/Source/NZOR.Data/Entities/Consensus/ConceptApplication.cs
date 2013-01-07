using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Consensus
{
    public class ConceptApplication
    {
        public Guid ConceptApplicationId { get; set; }

        public Guid FromConceptId { get; set; }
        public Guid ToConceptId { get; set; }
        public Guid ConceptApplicationTypeId { get; set; }

        public string Gender { get; set; }
        public string PartOfTaxon { get; set; }
        public string LifeStage { get; set; }
        public Guid? GeoRegionId { get; set; }
        public Guid? GeographicSchemaId { get; set; }
        public string GeoRegion { get; set; }
        public string GeographicSchema { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public ConceptApplication()
        {
            ConceptApplicationId = Guid.NewGuid();

            FromConceptId = Guid.Empty;
            ToConceptId = Guid.Empty;
            ConceptApplicationTypeId = Guid.Empty;

            Gender = null;
            PartOfTaxon = null;
            LifeStage = null;
            GeoRegionId = null;
            GeographicSchemaId = null;
            GeoRegion = null;
            GeographicSchema = null; 

            AddedDate = null;
            ModifiedDate = null;
        }
    }
}
