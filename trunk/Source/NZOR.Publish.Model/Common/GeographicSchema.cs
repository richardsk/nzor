using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Common
{
    public class GeographicSchema
    {
        public Guid GeographicSchemaId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public List<GeoRegion> GeoRegions { get; set; }

        public GeographicSchema()
        {
            Name = String.Empty;
            Description = String.Empty;

            GeoRegions = new List<GeoRegion>();
        }
    }
}
