using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Common;

namespace NZOR.Data.LookUps.Common
{
    public class ReferenceTypeLookUp
    {
        public static Guid GenericReferenceType = new Guid("1EF6FCA4-019F-40B6-8F8F-194FF59B25F3");

        List<ReferenceType> _referenceTypes;

        public ReferenceTypeLookUp(List<ReferenceType> referenceTypes)
        {
            _referenceTypes = referenceTypes;
        }

        public ReferenceType GetReferenceType(String type)
        {
            ReferenceType referenceType = null;

            if (_referenceTypes != null)
            {
                referenceType = (from rt in _referenceTypes
                                 where rt.Name.Equals(type, StringComparison.OrdinalIgnoreCase)
                                 select rt).SingleOrDefault();
            }

            return referenceType;
        }
    }
}
