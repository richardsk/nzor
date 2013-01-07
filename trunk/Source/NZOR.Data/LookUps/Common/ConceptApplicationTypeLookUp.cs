using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Common;

namespace NZOR.Data.LookUps.Common
{
    public class ConceptApplicationTypeLookUp
    {
        List<ConceptApplicationType> _conceptApplicationTypes;

        public static readonly String IsVernacularFor = @"is vernacular for";
        public static readonly String IsTradeNameFor = @"is trade name for";

        public ConceptApplicationTypeLookUp(List<ConceptApplicationType> conceptApplicationTypes)
        {
            _conceptApplicationTypes = conceptApplicationTypes;
        }

        public ConceptApplicationType GetConceptApplicationType(String name)
        {
            ConceptApplicationType conceptConceptApplicationType = null;

            if (_conceptApplicationTypes != null)
            {
                conceptConceptApplicationType = (from o in _conceptApplicationTypes
                                                 where o.Name == name
                                                 select o).SingleOrDefault();
            }

            return conceptConceptApplicationType;
        }
    }
}
