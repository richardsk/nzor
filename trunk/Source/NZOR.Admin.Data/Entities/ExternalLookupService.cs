using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public class ExternalLookupService : Entity
    {
        public Guid ExternalLookupServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DataFormat { get; set; }
        public string NameLookupEndpoint {get; set;}
        public string ConceptLookupEndpoint { get; set; }
        public string ReferenceLookupEndpoint { get; set; }
        public string IDLookupEndpoint { get; set; }
        public string SpaceCharacterSubstitute { get; set; }
        public string IconFilename { get; set; }
        public string LookupServiceClassName { get; set; }

        public ExternalLookupService()
        {
            ExternalLookupServiceId = Guid.Empty;

            Name = null;
            Description = null;
            DataFormat = null;
            NameLookupEndpoint = null;
            ConceptLookupEndpoint = null;
            ReferenceLookupEndpoint = null;
            IDLookupEndpoint = null;
            SpaceCharacterSubstitute = null;
            IconFilename = null;
            LookupServiceClassName = null;
        }
    }
}
