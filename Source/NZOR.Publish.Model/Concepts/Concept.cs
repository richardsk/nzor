using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Names;
using NZOR.Publish.Model.References;

namespace NZOR.Publish.Model.Concepts
{
    public class Concept
    {
        public Guid ConceptId { get; set; }

        public NameLink Name { get; set; }
        public ReferenceLink Publication { get; set; }

        public string Orthography { get; set; }
        public string Rank { get; set; }
        public string HigherClassification { get; set; }

        public List<Relationship> FromRelationships { get; set; }
        public List<Relationship> ToRelationships { get; set; }

        public List<Application> FromApplications { get; set; }
        public List<Application> ToApplications { get; set; }

        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Concept()
        {
            Orthography = String.Empty;
            Rank = String.Empty;
            HigherClassification = String.Empty;

            FromRelationships = new List<Relationship>();
            ToRelationships = new List<Relationship>();

            FromApplications = new List<Application>();
            ToApplications = new List<Application>();
        }
    }
}
