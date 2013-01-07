using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Provider;

namespace NZOR.Data.Repositories.Provider
{
    public interface IConceptRepository
    {
        List<Concept> Concepts { get; }

        Concept GetConcept(Guid conceptId);
        Concept GetConceptByProviderId(string providerRecordId, Guid providerId);

        List<Concept> GetProviderConcepts(Guid consensusConceptId);
        List<Concept> GetConcepts(Guid dataSourceId);
        List<Concept> GetConceptsByName(Guid nameId);
        List<Concept> GetProviderConceptsByName(Guid consensusNameId);

        List<Concept> GetConceptsModifiedSince(DateTime fromDate);

        void DeleteConcept(Guid conceptId);

        void Save();
        void Save(Concept pc, bool saveRelationships);
        void SaveRelationships(Concept pc);
    }
}
