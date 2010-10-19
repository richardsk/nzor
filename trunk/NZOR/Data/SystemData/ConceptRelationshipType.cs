using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace NZOR.Data
{
    public class ConceptRelationshipType
    {
        private static Guid _parRelId = Guid.Empty;
        private static Guid _prefRelId = Guid.Empty;

        public static Guid ParentRelationshipTypeID(SqlConnection cnn)
        {
            if (_parRelId == Guid.Empty)
            {
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select ConceptRelationshipTypeID from ConceptRelationshipType where Relationship = 'is child of'";
                    object val = cmd.ExecuteScalar();
                    if (val != DBNull.Value) _parRelId = (Guid)val;
                }
            }
            return _parRelId;
        }

        public static Guid PreferredRelationshipTypeID(SqlConnection cnn)
        {
            if (_prefRelId == Guid.Empty)
            {
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select ConceptRelationshipTypeID from ConceptRelationshipType where Relationship = 'is synonym of'";
                    object val = cmd.ExecuteScalar();
                    if (val != DBNull.Value) _prefRelId = (Guid)val;
                }
            }
            return _prefRelId;
        }
    }
}
