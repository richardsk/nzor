using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Data.Entities.Common;

namespace NZOR.Data.LookUps.Common
{
    public class TaxonPropertyClassLookup
    {
        private List<TaxonPropertyClass> _propertyClasses;

        public const string PropertyClassBiostatus = "Biostatus";
        public const string PropertyClassManagementStatus = "Management Status";

        public TaxonPropertyClassLookup(List<TaxonPropertyClass> propertyClasses)
        {
            _propertyClasses = propertyClasses;
        }

        public TaxonPropertyClass GetTaxonPropertyClass(String className)
        {
            if (_propertyClasses != null)
            {
                foreach (TaxonPropertyClass pc in _propertyClasses)
                {
                    if (String.Compare(pc.Name, className, true) == 0)
                    {
                        return pc;
                    }
                }
            }
            return null;
        }
    }
}
