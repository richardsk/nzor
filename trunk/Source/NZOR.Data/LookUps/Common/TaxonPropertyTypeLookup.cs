using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Data.Entities.Common;

namespace NZOR.Data.LookUps.Common
{
    public class TaxonPropertyTypeLookup
    {        
        private List<TaxonPropertyType> _propertyTypes;

        public const string PropertyTypeGeoRegion = "GeoRegion";
        public const string PropertyTypeGeoSchema = "GeoSchema";
        public const string PropertyTypeBiome = "Biome";
        public const string PropertyTypeEnvironmentalContext = "Environmental Context";
        public const string PropertyTypeOrigin = "Origin";
        public const string PropertyTypeOccurrence = "Occurrence";
        public const string PropertyTypeAction = "Action";
        public const string PropertyTypeOutcome = "Outcome";
        public const string PropertyTypeActionStatus = "ActionStatus";
        public const string PropertyTypeStatus = "Status";

        public static Guid PropertyTypeGeoRegionId = new Guid("7C42B4C0-BC62-4776-9C06-1C78C7215ACF");

        public TaxonPropertyTypeLookup(List<TaxonPropertyType> propertyTypes)
        {
            _propertyTypes = propertyTypes;
        }

        public TaxonPropertyType GetTaxonPropertyType(Guid classId, String propertyName)
        {
            if (_propertyTypes != null)
            {
                foreach (TaxonPropertyType pt in _propertyTypes)
                {
                    if (pt.TaxonPropertyClassId == classId && String.Compare(pt.Name, propertyName, true) == 0)
                    {
                        return pt;
                    }
                }
            }
            return null;
        }
    }
}
