using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Common;

namespace NZOR.Data.LookUps.Common
{
    /// <summary>
    /// Utility class for looking up concept relationship type ids.
    /// </summary>
    public class ConceptRelationshipTypeLookUp
    {
        List<ConceptRelationshipType> _conceptRelationshipTypes;

        public static readonly String IsParentOf = @"is parent of";
        public static readonly String IsTeleomorphOf = @"is teleomorph of";
        public static readonly String IsHybridChildOf = @"is hybrid child of";
        public static readonly String IsSynonymOf = @"is synonym of";
        public static readonly String IsAnamorphOf = @"is anamorph of";
        public static readonly String HasVernacularOf = @"has vernacular of";
        public static readonly String IsChildOf = @"is child of";
        public static readonly String IsHybridParentFor = @"is hybrid parent for";

        public ConceptRelationshipTypeLookUp(List<ConceptRelationshipType> conceptRelationshipTypes)
        {
            _conceptRelationshipTypes = conceptRelationshipTypes;
        }

        public ConceptRelationshipType GetConceptRelationshipType(String relationship)
        {
            ConceptRelationshipType conceptRelationshipType = null;

            if (_conceptRelationshipTypes != null)
            {
                conceptRelationshipType = (from o in _conceptRelationshipTypes
                                           where o.Relationship.Equals(relationship, StringComparison.OrdinalIgnoreCase)
                                           select o).SingleOrDefault();
            }

            return conceptRelationshipType;
        }
    }
}
