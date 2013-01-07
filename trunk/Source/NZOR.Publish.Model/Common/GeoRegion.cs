using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Common
{
    public class GeoRegion
    {
        public Guid GeoRegionId { get; set; }

        public Guid? ParentGeoRegionId { get; set; }

        public string Name { get; set; }
        public string Language { get; set; }

        public GeoRegion()
        {
            ParentGeoRegionId = null;

            Name = String.Empty;
            Language = String.Empty;
        }
    }
}
