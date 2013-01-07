using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Consensus;
using NZOR.Data.Repositories.Consensus;
using System.Data.SqlClient;
using System.Data;

namespace NZOR.Data.Sql.Repositories.Consensus
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

        public Concept GetConcept(Guid conceptId)
        {
            Concept concept = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@conceptId", conceptId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Concept-GET.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    concept = new Concept();
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    concept.ConceptRelationships.AddRange(GetConceptRelationships(conceptId));
                    concept.ConceptApplications.AddRange(GetConceptApplications(concept.ConceptId));
                }
            }

            return concept;
        }

        private List<ConceptRelationship> GetConceptRelationships(Guid conceptId)
        {
            List<ConceptRelationship> crList = new List<ConceptRelationship>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@conceptId", conceptId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.ConceptRelationship-GETByConcept.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    ConceptRelationship cr = new ConceptRelationship();
                    cr.ConceptRelationshipId = row.Field<Guid>("ConceptRelationshipID");
                    cr.FromConceptId = row.Field<Guid>("FromConceptId");
                    cr.ToConceptId = row.Field<Guid>("ToConceptId");
                    cr.ConceptRelationshipTypeId = row.Field<Guid>("ConceptRelationshipTypeId");
                    cr.ConceptRelationshipType = row.Field<String>("ConceptRelationshipType");
                    cr.NameToId = row.Field<Guid?>("NameToId");
                    cr.NameTo = row.Field<String>("NameTo");
                    cr.IsActive = row.Field<bool>("IsActive");
                    cr.Sequence = row.Field<int?>("Sequence");
                    cr.AddedDate = row.Field<DateTime?>("AddedDate");
                    cr.ModifiedDate = row.Field<DateTime?>("ModifiedDate");

                    crList.Add(cr);
                }
            }

            return crList;
        }


        private List<ConceptApplication> GetConceptApplications(Guid conceptId)
        {
            List<ConceptApplication> apps = new List<ConceptApplication>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@conceptId", conceptId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.ConceptApplication-GETByConcept.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    ConceptApplication ca = new ConceptApplication();

                    ca.ConceptApplicationId = row.Field<Guid>("ConceptApplicationID");
                    ca.ConceptApplicationTypeId = row.Field<Guid>("ConceptApplicationTypeID");
                    ca.FromConceptId = row.Field<Guid>("FromConceptID");
                    ca.ToConceptId = row.Field<Guid>("ToConceptID");
                    ca.Gender = row.Field<String>("Gender");
                    ca.PartOfTaxon = row.Field<String>("PartOfTaxon");
                    ca.LifeStage = row.Field<String>("LifeStage");
                    ca.GeoRegionId = row.Field<Guid?>("GeoRegionID");
                    ca.GeographicSchemaId = row.Field<Guid?>("GeographicSchemaID");
                    ca.GeoRegion = row.Field<String>("GeoRegion");
                    ca.GeographicSchema = row.Field<String>("GeographicSchema");

                    ca.AddedDate = row.Field<DateTime?>("AddedDate");
                    ca.ModifiedDate = row.Field<DateTime?>("ModifiedDate");

                    apps.Add(ca);
                }
            }

            return apps;
        }

        public Concept GetConcept(Guid nameId, Guid? accordingToId)
        {
            Concept concept = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@nameId", nameId));
            
            SqlParameter p = new SqlParameter("@accordingToId", SqlDbType.UniqueIdentifier);
            if (accordingToId.HasValue) p.Value = accordingToId.Value;
            else p.Value = DBNull.Value;
            parameters.Add(p);

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Concept-GETByNameAndRef.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    concept = new Concept();
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    concept.ConceptRelationships.AddRange(GetConceptRelationships(concept.ConceptId));
                    concept.ConceptApplications.AddRange(GetConceptApplications(concept.ConceptId));
                }
            }

            return concept;
        }

        public Concept GetConcensusConcept(Guid providerConceptId)
        {
            Concept concept = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@providerConceptId", providerConceptId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Concept-GETForProviderConcept.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    concept = new Concept();
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    concept.ConceptRelationships.AddRange(GetConceptRelationships(concept.ConceptId));
                    concept.ConceptApplications.AddRange(GetConceptApplications(concept.ConceptId));
                }
            }

            return concept;
        }

        public List<Concept> GetRelatedConcepts(Guid providerNameId, Guid? providerAccordingToId)
        {
            List<Concept> concepts = new List<Concept>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@providerNameId", providerNameId));
            parameters.Add(new SqlParameter("@providerAccordingToId", (object)providerAccordingToId ?? DBNull.Value));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Concept-LISTRelated.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Concept concept = new Concept();

                    concept.NameId = row.Field<Guid>("NameID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");
                    
                    concept.ConceptRelationships.AddRange(GetConceptRelationships(concept.ConceptId));
                    concept.ConceptApplications.AddRange(GetConceptApplications(concept.ConceptId));

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

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Concept-GetByName.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Concept concept = new Concept();
                    concept.ConceptId = row.Field<Guid>("ConceptID");
                    concept.AccordingToReferenceId = row.Field<Guid?>("AccordingToReferenceId");
                    concept.AddedDate = row.Field<DateTime?>("AddedDate");
                    concept.HigherClassification = row.Field<String>("HigherClassification");
                    concept.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    concept.NameId = row.Field<Guid>("NameID");
                    concept.Orthography = row.Field<String>("Orthography");
                    concept.TaxonRank = row.Field<String>("TaxonRank");

                    concept.ConceptRelationships.AddRange(GetConceptRelationships(concept.ConceptId));
                    concept.ConceptApplications.AddRange(GetConceptApplications(concept.ConceptId));

                    concepts.Add(concept);
                }
            }

            return concepts;
        }

        public List<ConceptApplication> GetRelatedConceptApplications(Guid nameId)
        {
            List<ConceptApplication> apps = new List<ConceptApplication>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@nameId", nameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.ConceptApplication-GetRelated.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    ConceptApplication ca = new ConceptApplication();

                    ca.ConceptApplicationId = row.Field<Guid>("ConceptApplicationID");
                    ca.ConceptApplicationTypeId = row.Field<Guid>("ConceptApplicationTypeID");
                    ca.FromConceptId = row.Field<Guid>("FromConceptID");
                    ca.ToConceptId = row.Field<Guid>("ToConceptID");
                    ca.Gender = row.Field<String>("Gender");
                    ca.PartOfTaxon = row.Field<String>("PartOfTaxon");
                    ca.LifeStage = row.Field<String>("LifeStage");
                    ca.GeoRegionId = row.Field<Guid?>("GeoRegionID");
                    ca.GeographicSchemaId = row.Field<Guid?>("GeographicSchemaID");
                    ca.GeoRegion = row.Field<String>("GeoRegion");
                    ca.GeographicSchema = row.Field<String>("GeographicSchema");

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

            if (concept.State == Entities.Entity.EntityState.Added)
            {
                sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Concept-INSERT.sql");
            }
            else if (concept.State == Entities.Entity.EntityState.Modified)
            {
                sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Concept-UPDATE.sql");
            }

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@ConceptID", concept.ConceptId);

                    cmd.Parameters.AddWithValue("@NameID", concept.NameId);
                    cmd.Parameters.AddWithValue("@AccordingToReferenceID", (object)concept.AccordingToReferenceId ?? DBNull.Value);

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


            concept.State = Entities.Entity.EntityState.Unchanged;
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
                Save(concept, false);
            }

            foreach (Concept concept in _concepts.Where(o => o.State == Entities.Entity.EntityState.Added || o.State == Entities.Entity.EntityState.Modified))
            {
                SaveRelationships(concept);

                concept.State = Entities.Entity.EntityState.Unchanged;
            }
        }

        private void InsertConceptRelationships(Concept concept)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.ConceptRelationship-INSERT.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                foreach (ConceptRelationship conceptRelationship in concept.ConceptRelationships)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@ConceptRelationshipID", conceptRelationship.ConceptRelationshipId);

                        cmd.Parameters.AddWithValue("@FromConceptID", (object)conceptRelationship.FromConceptId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToConceptID", (object)conceptRelationship.ToConceptId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ConceptRelationshipTypeID", (object)conceptRelationship.ConceptRelationshipTypeId ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@isActive", conceptRelationship.IsActive);
                        cmd.Parameters.AddWithValue("@Sequence", (object)conceptRelationship.Sequence ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }

                }
            }
        }

        private void DeleteConceptRelationships(Concept concept)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.ConceptRelationships-DELETE.sql");

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
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.ConceptApplication-INSERT.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                foreach (ConceptApplication conceptApplication in concept.ConceptApplications)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@ConceptApplicationID", conceptApplication.ConceptApplicationId);

                        cmd.Parameters.AddWithValue("@FromConceptID", conceptApplication.FromConceptId);
                        cmd.Parameters.AddWithValue("@ToConceptID", conceptApplication.ToConceptId);
                        cmd.Parameters.AddWithValue("@ConceptApplicationTypeID", conceptApplication.ConceptApplicationTypeId);

                        cmd.Parameters.AddWithValue("@Gender", (object)conceptApplication.Gender ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PartOfTaxon", (object)conceptApplication.PartOfTaxon ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LifeStage", (object)conceptApplication.LifeStage ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@GeoRegionID", (object)conceptApplication.GeoRegionId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@GeographicSchemaID", (object)conceptApplication.GeographicSchemaId ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@AddedDate", (object)conceptApplication.AddedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedDate", (object)conceptApplication.ModifiedDate ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void DeleteConceptApplications(Concept concept)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.ConceptApplications-DELETE.sql");

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

        public void SetInUseConcept(Guid nameId, Guid conceptRelationshipTypeId, Guid? accordingToId, Guid? toNameId)
        {
            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Concept-SETInUseConcept.sql");

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@nameId", nameId);
                    cmd.Parameters.AddWithValue("@conceptRelationshipTypeID", conceptRelationshipTypeId);
                    cmd.Parameters.AddWithValue("@toNameId", (object)toNameId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@accordingToId", (object)accordingToId ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteConcept(Concept concept, Guid? replacementId)
        {
            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Concept-DELETE.sql");

                using (SqlCommand cmd = new SqlCommand(sql, cnn))
                {
                    cmd.Parameters.AddWithValue("@conceptId", concept.ConceptId);
                    cmd.Parameters.AddWithValue("@nameId", concept.NameId);
                    cmd.Parameters.AddWithValue("@accordingToReferenceId", (object)concept.AccordingToReferenceId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@replacementId", (object)replacementId ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
