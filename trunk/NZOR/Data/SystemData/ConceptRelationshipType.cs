using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data
{
    public class ConceptRelationshipType
    {
        private static Guid _parRelId = Guid.Empty;
        private static Guid _prefRelId = Guid.Empty;

        public static Guid ParentRelationshipTypeID()
        {
            if (_parRelId == Guid.Empty)
            {
                NZOR.Data.SystemData.NZOR_System se = new NZOR.Data.SystemData.NZOR_System();
                var rels = from rt in se.ConceptRelationshipType where rt.Relationship.Equals("is child of") select rt.ConceptRelationshipTypeID;
                _parRelId = rels.First();
            }
            return _parRelId;
        }

        public static Guid PreferredRelationshipTypeID()
        {
            if (_prefRelId == Guid.Empty)
            {
                NZOR.Data.SystemData.NZOR_System se = new NZOR.Data.SystemData.NZOR_System();
                var rels = from rt in se.ConceptRelationshipType where rt.Relationship.Equals("is synonym of") select rt.ConceptRelationshipTypeID;
                _prefRelId = rels.First();
            }
            return _prefRelId;
        }
    }
}
