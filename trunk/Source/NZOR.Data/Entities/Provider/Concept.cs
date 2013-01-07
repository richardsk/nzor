using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NZOR.Data.Entities.Provider
{
    [DebuggerDisplay("Id: {ConceptId}; ProviderNameId: {ProviderNameId}")]
    public class Concept : Entity
    {
        public enum ConceptType
        {
            Undefined,

            TaxonConcept,
            VernacularConcept,
            TaxonNameUse,
            VernacularUse
        }

        public Guid ConceptId { get; set; }

        public Guid NameId { get; set; }
        public Guid? AccordingToReferenceId { get; set; }
        public Guid DataSourceId { get; set; }

        public Guid? ConsensusConceptId { get; set; }
        public Guid? IntegrationBatchId { get; set; }
        public String LinkStatus { get; set; }
        public Int32? MatchScore { get; set; }
        public String MatchPath { get; set; }

        public String ProviderRecordId { get; set; }
        public DateTime? ProviderCreatedDate { get; set; }
        public DateTime? ProviderModifiedDate { get; set; }
        public String ProviderNameId { get; set; }
        public String ProviderReferenceId { get; set; }
        public String ProviderName { get; set; }
        public Guid ProviderId { get; set; }

        public String Orthography { get; set; }
        public String TaxonRank { get; set; }
        public String HigherClassification { get; set; }
        public String NameQualifier { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public List<ConceptApplication> ConceptApplications { get; private set; }
        public List<ConceptRelationship> ConceptRelationships { get; private set; }

        public ConceptType Type { get; set; }

        public Concept()
        {
            ConceptId = Guid.Empty;

            NameId = Guid.Empty;
            AccordingToReferenceId = null;
            DataSourceId = Guid.Empty;

            ConsensusConceptId = null;
            IntegrationBatchId = null;
            LinkStatus = null;
            MatchScore = null;
            MatchPath = null;

            ProviderRecordId = null;
            ProviderCreatedDate = null;
            ProviderModifiedDate = null;
            ProviderNameId = null;
            ProviderReferenceId = null;

            Orthography = null;
            TaxonRank = null;
            HigherClassification = null;
            NameQualifier = null;

            AddedDate = null;
            ModifiedDate = null;

            ConceptApplications = new List<ConceptApplication>();
            ConceptRelationships = new List<ConceptRelationship>();

            Type = ConceptType.Undefined;
        }

        public List<ConceptRelationship> GetRelationships(Guid relationshipTypeId)
        {
            List<ConceptRelationship> crList = new List<ConceptRelationship>();

            foreach (ConceptRelationship rel in ConceptRelationships)
            {
                if (rel.ConceptRelationshipTypeId == relationshipTypeId) crList.Add(rel);
            }

            return crList;
        }

        public List<ConceptApplication> GetApplications(Guid applicationTypeId)
        {
            List<ConceptApplication> caList = new List<ConceptApplication>();

            foreach (ConceptApplication app in ConceptApplications)
            {
                if (app.ConceptApplicationTypeId == applicationTypeId) caList.Add(app);
            }

            return caList;
        }
    }
}
