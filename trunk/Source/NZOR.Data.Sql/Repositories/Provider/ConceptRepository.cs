using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Provider;
using NZOR.Data.Repositories.Provider;
using System.Data.SqlClient;
using System.Data;

namespace NZOR.Data.Sql.Repositories.Provider
{
    public class ConceptRepository : Repository<Concept>, IConceptRepository
    {
        private List<Concept> _concepts;

        public ConceptRepository(String connectionString)
            : base(connectionString)
        {
            _concepts = new List<Concept>();
        }

        public List<Concept> Concepts
        {
            get { return _concepts; }
        }

        public List<Concept> GetConcepts(Guid dataSourceId)
        {
            List<Concept> concepts = new List<Concept>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@DataSourceID", dataSourceId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Concept-LIST.sql"), parameters, 200))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Concept concept = new Concept();

                    concept.ProviderRecordId = row.Field<String>("ProviderRecordID");
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.DataSourceId = row.Field<Guid>("DataSourceId");
                    concept.ConsensusConceptId = row.Field<Guid?>("ConsensusConceptID");
                    concept.LinkStatus = row.Field<String>("LinkStatus");
                    concept.MatchScore = row.Field<int?>("MatchScore");
                    concept.MatchPath = row.Field<String>("MatchPath");
                    concept.ProviderNameId = row.Field<String>("ProviderNameID");
                    concept.ProviderReferenceId = row.Field<String>("ProviderReferenceID");
                    concept.ProviderName = row.Field<String>("ProviderName");
                    concept.ProviderId = row.Field<Guid>("ProviderId");
                    concept.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    concept.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    Concept.ConceptType t = Concept.ConceptType.Undefined;
                    Enum.TryParse(row.Field<String>("ConceptSourceType"), out t);
                    concept.Type = t;

                    concept.ConceptRelationships.AddRange(GetRelationships(concept));
                    concept.ConceptApplications.AddRange(GetApplications(concept));

                    concepts.Add(concept);
                }
            }

            return concepts;
        }

        public Concept GetConcept(Guid conceptId)
        {
            Concept concept = new Concept();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@conceptId", conceptId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Concept-GET.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    concept.ProviderRecordId = row.Field<String>("ProviderRecordID");
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.DataSourceId = row.Field<Guid>("DataSourceId");
                    concept.ConsensusConceptId = row.Field<Guid?>("ConsensusConceptID");
                    concept.IntegrationBatchId = row.Field<Guid?>("IntegrationBatchId");
                    concept.LinkStatus = row.Field<String>("LinkStatus");
                    concept.MatchScore = row.Field<int?>("MatchScore");
                    concept.MatchPath = row.Field<String>("MatchPath");
                    concept.ProviderNameId = row.Field<String>("ProviderNameID");
                    concept.ProviderReferenceId = row.Field<String>("ProviderReferenceID");
                    concept.ProviderName = row.Field<String>("ProviderName");
                    concept.ProviderId = row.Field<Guid>("ProviderId");
                    concept.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    concept.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    Concept.ConceptType t = Concept.ConceptType.Undefined;
                    Enum.TryParse(row.Field<String>("ConceptSourceType"), out t);
                    concept.Type = t;

                    concept.ConceptRelationships.AddRange(GetRelationships(concept));
                    concept.ConceptApplications.AddRange(GetApplications(concept));
                }
            }

            return concept;
        }

        public Concept GetConceptByProviderId(string providerRecordId, Guid providerId)
        {
            Concept concept = new Concept();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@providerRecordId", providerRecordId));
            parameters.Add(new SqlParameter("@providerId", providerId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Concept-GETByProviderId.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    concept.ProviderRecordId = row.Field<String>("ProviderRecordID");
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.DataSourceId = row.Field<Guid>("DataSourceId");
                    concept.ConsensusConceptId = row.Field<Guid?>("ConsensusConceptID");
                    concept.IntegrationBatchId = row.Field<Guid?>("IntegrationBatchId");
                    concept.LinkStatus = row.Field<String>("LinkStatus");
                    concept.MatchScore = row.Field<int?>("MatchScore");
                    concept.MatchPath = row.Field<String>("MatchPath");
                    concept.ProviderNameId = row.Field<String>("ProviderNameID");
                    concept.ProviderReferenceId = row.Field<String>("ProviderReferenceID");
                    concept.ProviderName = row.Field<String>("ProviderName");
                    concept.ProviderId = row.Field<Guid>("ProviderId");
                    concept.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    concept.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    Concept.ConceptType t = Concept.ConceptType.Undefined;
                    Enum.TryParse(row.Field<String>("ConceptSourceType"), out t);
                    concept.Type = t;

                    concept.ConceptRelationships.AddRange(GetRelationships(concept));
                    concept.ConceptApplications.AddRange(GetApplications(concept));
                }
            }

            return concept;
        }

        public Concept GetConceptByNameAndReference(Guid nameId, Guid? accordingToId)
        {
            Concept concept = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@nameId", nameId));
            parameters.Add(new SqlParameter("@accordingToId", (object)accordingToId ?? DBNull.Value));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Concept-GETByNameAndRef.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    concept = new Concept();

                    DataRow row = tbl.Rows[0];

                    concept.ProviderRecordId = row.Field<String>("ProviderRecordID");
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.DataSourceId = row.Field<Guid>("DataSourceId");
                    concept.ConsensusConceptId = row.Field<Guid?>("ConsensusConceptID");
                    concept.IntegrationBatchId = row.Field<Guid?>("IntegrationBatchId");
                    concept.LinkStatus = row.Field<String>("LinkStatus");
                    concept.MatchScore = row.Field<int?>("MatchScore");
                    concept.MatchPath = row.Field<String>("MatchPath");
                    concept.ProviderNameId = row.Field<String>("ProviderNameID");
                    concept.ProviderReferenceId = row.Field<String>("ProviderReferenceID");
                    concept.ProviderName = row.Field<String>("ProviderName");
                    concept.ProviderId = row.Field<Guid>("ProviderId");
                    concept.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    concept.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    Concept.ConceptType t = Concept.ConceptType.Undefined;
                    Enum.TryParse(row.Field<String>("ConceptSourceType"), out t);
                    concept.Type = t;

                    concept.ConceptRelationships.AddRange(GetRelationships(concept));
                    concept.ConceptApplications.AddRange(GetApplications(concept));
                }
            }

            return concept;
        }

        public List<Concept> GetProviderConcepts(Guid consensusConceptId)
        {
            List<Concept> concepts = new List<Concept>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@consensusConceptId", consensusConceptId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Concept-LISTProviderConcepts.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Concept concept = new Concept();

                    concept.ProviderRecordId = row.Field<String>("ProviderRecordID");
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.DataSourceId = row.Field<Guid>("DataSourceId");
                    concept.ConsensusConceptId = row.Field<Guid?>("ConsensusConceptID");
                    concept.IntegrationBatchId = row.Field<Guid?>("IntegrationBatchId");
                    concept.LinkStatus = row.Field<String>("LinkStatus");
                    concept.MatchScore = row.Field<int?>("MatchScore");
                    concept.MatchPath = row.Field<String>("MatchPath");
                    concept.ProviderNameId = row.Field<String>("ProviderNameID");
                    concept.ProviderReferenceId = row.Field<String>("ProviderReferenceID");
                    concept.ProviderName = row.Field<String>("ProviderName");
                    concept.ProviderId = row.Field<Guid>("ProviderId");
                    concept.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    concept.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    Concept.ConceptType t = Concept.ConceptType.Undefined;
                    Enum.TryParse(row.Field<String>("ConceptSourceType"), out t);
                    concept.Type = t;

                    concept.ConceptRelationships.AddRange(GetRelationships(concept));
                    concept.ConceptApplications.AddRange(GetApplications(concept));

                    concepts.Add(concept);
                }
            }

            return concepts;
        }

        public List<Concept> GetProviderConceptsByName(Guid consensusNameId)
        {
            List<Concept> concepts = new List<Concept>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@consensusNameId", consensusNameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Concept-LISTConceptsByName.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Concept concept = new Concept();

                    concept.ProviderRecordId = row.Field<String>("ProviderRecordID");
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.DataSourceId = row.Field<Guid>("DataSourceId");
                    concept.ConsensusConceptId = row.Field<Guid?>("ConsensusConceptID");
                    concept.IntegrationBatchId = row.Field<Guid?>("IntegrationBatchId");
                    concept.LinkStatus = row.Field<String>("LinkStatus");
                    concept.MatchScore = row.Field<int?>("MatchScore");
                    concept.MatchPath = row.Field<String>("MatchPath");
                    concept.ProviderNameId = row.Field<String>("ProviderNameID");
                    concept.ProviderReferenceId = row.Field<String>("ProviderReferenceID");
                    concept.ProviderName = row.Field<String>("ProviderName");
                    concept.ProviderId = row.Field<Guid>("ProviderId");
                    concept.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    concept.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    Concept.ConceptType t = Concept.ConceptType.Undefined;
                    Enum.TryParse(row.Field<String>("ConceptSourceType"), out t);
                    concept.Type = t;

                    concept.ConceptRelationships.AddRange(GetRelationships(concept));
                    concept.ConceptApplications.AddRange(GetApplications(concept));

                    concepts.Add(concept);
                }
            }

            return concepts;
        }

        public List<Concept> GetProviderConcepts(Guid consensusNameId, Guid? consensusAccToId)
        {
            List<Concept> concepts = new List<Concept>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@consensusNameId", consensusNameId));
            parameters.Add(new SqlParameter("@consensusAccToId", consensusAccToId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Concept-LISTConceptsByNameAndAccTo.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Concept concept = new Concept();

                    concept.ProviderRecordId = row.Field<String>("ProviderRecordID");
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.DataSourceId = row.Field<Guid>("DataSourceId");
                    concept.ConsensusConceptId = row.Field<Guid?>("ConsensusConceptID");
                    concept.IntegrationBatchId = row.Field<Guid?>("IntegrationBatchId");
                    concept.LinkStatus = row.Field<String>("LinkStatus");
                    concept.MatchScore = row.Field<int?>("MatchScore");
                    concept.MatchPath = row.Field<String>("MatchPath");
                    concept.ProviderNameId = row.Field<String>("ProviderNameID");
                    concept.ProviderReferenceId = row.Field<String>("ProviderReferenceID");
                    concept.ProviderName = row.Field<String>("ProviderName");
                    concept.ProviderId = row.Field<Guid>("ProviderId");
                    concept.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    concept.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    Concept.ConceptType t = Concept.ConceptType.Undefined;
                    Enum.TryParse(row.Field<String>("ConceptSourceType"), out t);
                    concept.Type = t;

                    concept.ConceptRelationships.AddRange(GetRelationships(concept));
                    concept.ConceptApplications.AddRange(GetApplications(concept));

                    concepts.Add(concept);
                }
            }

            return concepts;
        }

        public List<Concept> GetRelatedConcepts(Guid nameId, Guid? accToId)
        {
            List<Concept> concepts = new List<Concept>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@nameId", nameId));
            parameters.Add(new SqlParameter("@accToId", (object)accToId ?? DBNull.Value));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Concept-LISTRelated.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Concept concept = new Concept();

                    concept.ProviderRecordId = row.Field<String>("ProviderRecordID");
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.DataSourceId = row.Field<Guid>("DataSourceId");
                    concept.ConsensusConceptId = row.Field<Guid?>("ConsensusConceptID");
                    concept.IntegrationBatchId = row.Field<Guid?>("IntegrationBatchId");
                    concept.LinkStatus = row.Field<String>("LinkStatus");
                    concept.MatchScore = row.Field<int?>("MatchScore");
                    concept.MatchPath = row.Field<String>("MatchPath");
                    concept.ProviderNameId = row.Field<String>("ProviderNameID");
                    concept.ProviderReferenceId = row.Field<String>("ProviderReferenceID");
                    concept.ProviderName = row.Field<String>("ProviderName");
                    concept.ProviderId = row.Field<Guid>("ProviderId");
                    concept.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    concept.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    Concept.ConceptType t = Concept.ConceptType.Undefined;
                    Enum.TryParse(row.Field<String>("ConceptSourceType"), out t);
                    concept.Type = t;

                    concept.ConceptRelationships.AddRange(GetRelationships(concept));
                    concept.ConceptApplications.AddRange(GetApplications(concept));

                    concepts.Add(concept);
                }
            }

            return concepts;
        }

        public List<Concept> GetConceptsByName(Guid nameId)
        {
            List<Concept> concepts = new List<Concept>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@nameId", nameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Concept-LISTByName.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Concept concept = new Concept();

                    concept.ProviderRecordId = row.Field<String>("ProviderRecordID");
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.DataSourceId = row.Field<Guid>("DataSourceId");
                    concept.ConsensusConceptId = row.Field<Guid?>("ConsensusConceptID");
                    concept.IntegrationBatchId = row.Field<Guid?>("IntegrationBatchId");
                    concept.LinkStatus = row.Field<String>("LinkStatus");
                    concept.MatchScore = row.Field<int?>("MatchScore");
                    concept.MatchPath = row.Field<String>("MatchPath");
                    concept.ProviderNameId = row.Field<String>("ProviderNameID");
                    concept.ProviderReferenceId = row.Field<String>("ProviderReferenceID");
                    concept.ProviderName = row.Field<String>("ProviderName");
                    concept.ProviderId = row.Field<Guid>("ProviderId");
                    concept.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    concept.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    Concept.ConceptType t = Concept.ConceptType.Undefined;
                    Enum.TryParse(row.Field<String>("ConceptSourceType"), out t);
                    concept.Type = t;

                    concept.ConceptRelationships.AddRange(GetRelationships(concept));
                    concept.ConceptApplications.AddRange(GetApplications(concept));

                    concepts.Add(concept);
                }
            }

            return concepts;
        }

        private List<ConceptRelationship> GetRelationships(Concept concept)
        {
            List<ConceptRelationship> rels = new List<ConceptRelationship>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@conceptId", concept.ConceptId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.ConceptRelationship-GETByConcept.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    ConceptRelationship cr = new ConceptRelationship();

                    cr.ConceptRelationshipId = row.Field<Guid>("ConceptRelationshipID");
                    cr.ConceptRelationshipTypeId = row.Field<Guid?>("ConceptRelationshipTypeID");
                    cr.FromConceptId = row.Field<Guid?>("FromConceptID");
                    cr.ToConceptId = row.Field<Guid?>("ToConceptID");
                    cr.Sequence = row.Field<int?>("Sequence");
                    cr.InUse = row.Field<bool?>("InUse");

                    rels.Add(cr);
                }
            }

            return rels;
        }

        private List<ConceptApplication> GetApplications(Concept concept)
        {
            List<ConceptApplication> apps = new List<ConceptApplication>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@conceptId", concept.ConceptId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.ConceptApplication-GETByConcept.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    ConceptApplication ca = new ConceptApplication();

                    ca.ConceptApplicationId = row.Field<Guid>("ConceptApplicationID");
                    ca.ConceptApplicationTypeId = row.Field<Guid>("ConceptApplicationTypeID");
                    ca.FromConceptId = row.Field<Guid>("FromConceptID");
                    ca.ToConceptId = row.Field<Guid>("ToConceptID");
                    ca.Gender = row.Field<String>("Gender");
                    ca.GeoRegion = row.Field<String>("GeoRegion");
                    ca.GeographicSchema = row.Field<String>("GeographicSchema");
                    ca.PartOfTaxon = row.Field<String>("PartOfTaxon");
                    ca.InUse = row.Field<bool?>("InUse");
                    ca.AddedDate = row.Field<DateTime?>("AddedDate");
                    ca.ModifiedDate = row.Field<DateTime?>("ModifiedDate");

                    apps.Add(ca);
                }
            }

            return apps;
        }

        public void Save(Concept concept, bool saveRelationships)
        {
            String sql = String.Empty;

            //Name Based concepts are always according to "Datasource name + year & month"
            if (concept.Type == Concept.ConceptType.TaxonNameUse || concept.Type == Concept.ConceptType.VernacularUse)
            {
                DateTime refDate = DateTime.Now;
                if (concept.ProviderModifiedDate.HasValue || concept.ProviderCreatedDate.HasValue)
                {
                    refDate = concept.ProviderModifiedDate.HasValue ? concept.ProviderModifiedDate.Value : concept.ProviderCreatedDate.Value;
                }

                ReferenceRepository rr = new ReferenceRepository(_connectionString);
                Reference dsRef = rr.GetDataSourceReference(concept.DataSourceId, refDate);

                if (dsRef == null)
                {
                    dsRef = rr.CreateDataSourceReference(concept.DataSourceId, refDate);
                }

                concept.ProviderReferenceId = string.Empty;
                concept.AccordingToReferenceId = dsRef.ReferenceId;

                //only one "name based concept" per name
                if (concept.State == Entities.Entity.EntityState.Added)
                {
                    Concept exConcept = GetConceptByNameAndReference(concept.NameId, concept.AccordingToReferenceId);
                    if (exConcept != null)
                    {
                        Guid oldId = concept.ConceptId;

                        exConcept.ConceptRelationships.Clear();
                        exConcept.ConceptApplications.Clear();

                        exConcept.ConceptRelationships.AddRange(concept.ConceptRelationships);
                        exConcept.ConceptApplications.AddRange(concept.ConceptApplications);

                        concept = exConcept;

                        concept.State = Entities.Entity.EntityState.Modified;
                        concept.ModifiedDate = DateTime.Now;
                        
                        //update relationships
                        foreach (ConceptRelationship cr in concept.ConceptRelationships)
                        {
                            cr.FromConceptId = concept.ConceptId;
                        }

                        foreach (ConceptApplication ca in concept.ConceptApplications)
                        {
                            ca.FromConceptId = concept.ConceptId;
                        }

                        foreach (Concept c in _concepts)
                        {
                            foreach (ConceptRelationship cr in c.ConceptRelationships)
                            {
                                if (cr.ToConceptId == oldId) cr.ToConceptId = concept.ConceptId;
                            }

                            foreach (ConceptApplication ca in c.ConceptApplications)
                            {
                                if (ca.ToConceptId == oldId) ca.ToConceptId = concept.ConceptId;
                            }
                        }
                    }
                }
            }

            if (concept.State == Entities.Entity.EntityState.Added)
            {
                sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Concept-INSERT.sql");
            }
            else if (concept.State == Entities.Entity.EntityState.Modified)
            {
                sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Concept-UPDATE.sql");
            }

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandTimeout = 100;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@ConceptID", concept.ConceptId);

                    cmd.Parameters.AddWithValue("@NameID", concept.NameId);
                    cmd.Parameters.AddWithValue("@AccordingToReferenceID", (object)concept.AccordingToReferenceId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DataSourceID", concept.DataSourceId);

                    cmd.Parameters.AddWithValue("@consensusConceptId", (object)concept.ConsensusConceptId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IntegrationBatchId", (object)concept.IntegrationBatchId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@linkStatus", (object)concept.LinkStatus ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@matchScore", (object)concept.MatchScore ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@matchPath", (object)concept.MatchPath ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@ProviderNameID", (object)concept.ProviderNameId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProviderReferenceID", (object)concept.ProviderReferenceId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProviderRecordID", (object)concept.ProviderRecordId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProviderCreatedDate", (object)concept.ProviderCreatedDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProviderModifiedDate", (object)concept.ProviderModifiedDate ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@ConceptSourceType", concept.Type.ToString());

                    cmd.Parameters.AddWithValue("@Orthography", (object)concept.Orthography ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TaxonRank", (object)concept.TaxonRank ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HigherClassification", (object)concept.HigherClassification ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@AddedDate", (object)concept.AddedDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModifiedDate", (object)concept.ModifiedDate ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }

            if (concept.State != Entities.Entity.EntityState.Added)
            {
                DeleteConceptRelationships(concept);
                DeleteConceptApplications(concept);
            }

            if (saveRelationships) SaveRelationships(concept);
        }

        public void SaveRelationships(Concept concept)
        {
            InsertConceptRelationships(concept);
            InsertConceptApplications(concept);
        }

        public void Save()
        {
                foreach (Concept concept in _concepts.Where(o => o.State == Entities.Entity.EntityState.Added || o.State == Entities.Entity.EntityState.Modified))
                {
                    try
                    {
                        Save(concept, false);
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                    }
                }

                foreach (Concept concept in _concepts.Where(o => o.State == Entities.Entity.EntityState.Added || o.State == Entities.Entity.EntityState.Modified))
                {
                    try
                    {
                        SaveRelationships(concept);

                        concept.State = Entities.Entity.EntityState.Unchanged;
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                    }
                }
        }

        private void InsertConceptRelationships(Concept concept)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.ConceptRelationship-INSERT.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                foreach (ConceptRelationship conceptRelationship in concept.ConceptRelationships)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandTimeout = 100;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@ConceptRelationshipID", conceptRelationship.ConceptRelationshipId);

                        cmd.Parameters.AddWithValue("@FromConceptID", (object)conceptRelationship.FromConceptId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToConceptID", (object)conceptRelationship.ToConceptId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ConceptRelationshipTypeID", (object)conceptRelationship.ConceptRelationshipTypeId ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@Sequence", (object)conceptRelationship.Sequence ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@InUse", (object)conceptRelationship.InUse ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void DeleteConcept(Guid conceptId)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Concept-DELETE.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@ConceptID", conceptId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DeleteConceptRelationships(Concept concept)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.ConceptRelationships-DELETE.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@ConceptID", concept.ConceptId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InsertConceptApplications(Concept concept)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.ConceptApplication-INSERT.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                foreach (ConceptApplication conceptApplication in concept.ConceptApplications)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandTimeout = 100;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@ConceptApplicationID", conceptApplication.ConceptApplicationId);

                        cmd.Parameters.AddWithValue("@FromConceptID", conceptApplication.FromConceptId);
                        cmd.Parameters.AddWithValue("@ToConceptID", conceptApplication.ToConceptId);
                        cmd.Parameters.AddWithValue("@ConceptApplicationTypeID", conceptApplication.ConceptApplicationTypeId);

                        cmd.Parameters.AddWithValue("@Gender", (object)conceptApplication.Gender ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PartOfTaxon", (object)conceptApplication.PartOfTaxon ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LifeStage", (object)conceptApplication.LifeStage ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@GeoRegion", (object)conceptApplication.GeoRegion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@GeographicSchema", (object)conceptApplication.GeographicSchema ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@InUse", (object)conceptApplication.InUse ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@AddedDate", (object)conceptApplication.AddedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedDate", (object)conceptApplication.ModifiedDate ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void DeleteConceptApplications(Concept concept)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.ConceptApplications-DELETE.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@ConceptID", concept.ConceptId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Concept> GetConceptsModifiedSince(DateTime fromDate)
        {
            List<Concept> concepts = new List<Concept>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@fromDate", fromDate));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Concept-LISTModifiedSince.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Concept concept = new Concept();

                    concept.ProviderRecordId = row.Field<String>("ProviderRecordID");
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.DataSourceId = row.Field<Guid>("DataSourceId");
                    concept.ConsensusConceptId = row.Field<Guid?>("ConsensusConceptID");
                    concept.IntegrationBatchId = row.Field<Guid?>("IntegrationBatchId");
                    concept.LinkStatus = row.Field<String>("LinkStatus");
                    concept.MatchScore = row.Field<int?>("MatchScore");
                    concept.MatchPath = row.Field<String>("MatchPath");
                    concept.ProviderNameId = row.Field<String>("ProviderNameID");
                    concept.ProviderReferenceId = row.Field<String>("ProviderReferenceID");
                    concept.ProviderName = row.Field<String>("ProviderName");
                    concept.ProviderId = row.Field<Guid>("ProviderId");
                    concept.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    concept.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    Concept.ConceptType t = Concept.ConceptType.Undefined;
                    Enum.TryParse(row.Field<String>("ConceptSourceType"), out t);
                    concept.Type = t;

                    concept.ConceptRelationships.AddRange(GetRelationships(concept));
                    concept.ConceptApplications.AddRange(GetApplications(concept));

                    concepts.Add(concept);
                }
            }

            return concepts;
        }

    }
}
