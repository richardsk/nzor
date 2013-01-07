using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Common
{
    public class GeoRegion
    {
        public Guid GeoRegionId { get; set; }
        public Guid GeographicSchemaId { get; set; }
        public string Name { get; set; }
        public string GeographicSchemaName { get; set; }
        public Guid? CorrectGeoRegionId { get; set; }
        public Guid? ParentGeoRegionId { get; set; }
        public string Language { get; set; }

        public GeoRegion()
        {
            GeoRegionId = Guid.Empty;
            GeographicSchemaId = Guid.Empty;
            Name = null;
            GeographicSchemaName = null;
            CorrectGeoRegionId = null;
            ParentGeoRegionId = null;
            Language = null;
        }
    }
}
