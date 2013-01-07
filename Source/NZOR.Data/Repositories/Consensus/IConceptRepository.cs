using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Consensus;

namespace NZOR.Data.Repositories.Consensus
{
    public interface IConceptRepository
    {
        List<Concept> Concepts { get; }

        Concept GetConcept(Guid conceptId);
        Concept GetConcept(Guid nameId, Guid? accordingToId);
        List<Concept> GetConceptsByName(Guid nameId);

        Concept GetConcensusConcept(Guid providerConceptId);

        void Save();
        void Save(Concept cc, bool saveRelationships);
        void SaveRelationships(Concept concept);

        void SetInUseConcept(Guid conceptId, Guid conceptRelationshipTypeId, Guid? accordingToId, Guid? toNameId);

        void DeleteConcept(Concept concept, Guid? replacementId);
    }
}
