using System.Collections.Generic;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Publisher.Helpers;

namespace NZOR.Publish.Publisher.Builders
{
    class VocabulariesBuilder
    {
        private readonly string _connectionString;

        private List<Vocabulary> _vocabularies;

        public VocabulariesBuilder(string connectionString)
        {
            _connectionString = connectionString;

            _vocabularies = new List<Vocabulary>();
        }

        public void Build()
        {
            LoadVocabularies();
        }

        public List<Vocabulary> Vocabularies
        {
            get { return _vocabularies; }
        }

        private void LoadVocabularies()
        {
            _vocabularies.Clear();

            string sql = @"

SELECT 
    *
FROM
    dbo.ConceptRelationshipType

";

            var vocabulary = new Vocabulary();

            _vocabularies.Add(vocabulary);

            vocabulary.Title = "Concept Relationship";
            vocabulary.Uses.Add("Concept Relationships");

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    var value = new VocabularyValue();

                    value.Name = drd.GetString("Relationship");

                    vocabulary.Values.Add(value);
                }
            }
        }
    }
}
