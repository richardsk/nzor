using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Provider;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using NZOR.Data.Repositories.Provider;

namespace NZOR.Data.Sql.Repositories.Provider
{
    public class ReferenceRepository : Repository<Reference>, IReferenceRepository
    {
        private List<Reference> _references;

        public ReferenceRepository(String connectionString)
            : base(connectionString)
        {
            _connectionString = connectionString;
            _references = new List<Reference>();
        }

        public List<Reference> References
        {
            get { return _references; }
        }

        public List<Reference> GetReferences(Guid dataSourceId)
        {
            List<Reference> references = new List<Reference>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@DataSourceID", dataSourceId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Reference-LIST.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Reference reference = new Reference();

                    reference.ProviderRecordId = row.Field<String>("ProviderRecordID");
                    reference.ReferenceId = row.Field<Guid>("ReferenceID");
                    reference.AddedDate = row.Field<DateTime?>("AddedDate");
                    reference.ConsensusReferenceId = row.Field<Guid?>("ConsensusReferenceId");
                    reference.IntegrationBatchId = row.Field<Guid?>("IntegrationBatchId");
                    reference.DataSourceId = row.Field<Guid>("DataSourceId");
                    reference.LinkStatus = row.Field<String>("LinkStatus");
                    reference.MatchScore = row.Field<int?>("MatchScore");
                    reference.MatchPath = row.Field<String>("MatchPath");
                    reference.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    reference.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    reference.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    reference.ReferenceTypeId = row.Field<Guid>("ReferenceTypeId");

                    reference.ReferenceProperties.AddRange(GetReferenceProperties(reference.ReferenceId));

                    references.Add(reference);
                }
            }

            return references;
        }

        public List<Reference> GetReferencesForConcensusReference(Guid consensusReferenceId)
        {
            List<Reference> references = new List<Reference>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@consensusReferenceId", consensusReferenceId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Reference-LIST-ByConsensusReference.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Reference reference = new Reference();

                    reference.ProviderRecordId = row.Field<String>("ProviderRecordID");
                    reference.ReferenceId = row.Field<Guid>("ReferenceID");
                    reference.AddedDate = row.Field<DateTime?>("AddedDate");
                    reference.ConsensusReferenceId = row.Field<Guid?>("ConsensusReferenceId");
                    reference.IntegrationBatchId = row.Field<Guid?>("IntegrationBatchId");
                    reference.DataSourceId = row.Field<Guid>("DataSourceId");
                    reference.LinkStatus = row.Field<String>("LinkStatus");
                    reference.MatchScore = row.Field<int?>("MatchScore");
                    reference.MatchPath = row.Field<String>("MatchPath");
                    reference.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    reference.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    reference.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    reference.ReferenceTypeId = row.Field<Guid>("ReferenceTypeId");

                    reference.ReferenceProperties.AddRange(GetReferenceProperties(reference.ReferenceId));

                    references.Add(reference);
                }
            }

            return references;
        }

        public Reference CreateDataSourceReference(Guid dataSourceId, DateTime dataSourceDate)
        {                         
            //date is just year and month
            DateTime dsDate = new DateTime(dataSourceDate.Year, dataSourceDate.Month, 1);

            Guid id = Guid.NewGuid();

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Reference-CreateForDataSource.sql");

                    cmd.Parameters.AddWithValue("@ReferenceID", id);

                    cmd.Parameters.AddWithValue("@ReferenceTypeID", NZOR.Data.LookUps.Common.ReferenceTypeLookUp.GenericReferenceType);
                    cmd.Parameters.AddWithValue("@DataSourceID", dataSourceId);
                    cmd.Parameters.Add(new SqlParameter("@date", dsDate));

                    cmd.ExecuteNonQuery();
                }
            }

            Reference dr = GetReference(id);
            return dr;
        }

        public Reference GetDataSourceReference(Guid dataSourceId, DateTime dataSourceDate)
        {
            //date is just year and month
            Reference reference = null;

            DateTime dsDate = new DateTime(dataSourceDate.Year, dataSourceDate.Month, 1);
            
            string sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Reference-GETForDatasource.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new SqlParameter("@dataSourceId", dataSourceId));
                    cmd.Parameters.Add(new SqlParameter("@date", dsDate));

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables[0].Rows[0];

                        reference = new Reference();

                        reference.ProviderRecordId = row.Field<String>("ProviderRecordID");
                        reference.ReferenceId = row.Field<Guid>("ReferenceID");
                        reference.AddedDate = row.Field<DateTime?>("AddedDate");
                        reference.ConsensusReferenceId = row.Field<Guid?>("ConsensusReferenceId");
                        reference.IntegrationBatchId = row.Field<Guid?>("IntegrationBatchId");
                        reference.DataSourceId = row.Field<Guid>("DataSourceId");
                        reference.LinkStatus = row.Field<String>("LinkStatus");
                        reference.MatchScore = row.Field<int?>("MatchScore");
                        reference.MatchPath = row.Field<String>("MatchPath");
                        reference.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                        reference.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                        reference.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                        reference.ReferenceTypeId = row.Field<Guid>("ReferenceTypeId");
                    }
                }
            }
             
            if (reference != null) reference.ReferenceProperties.AddRange(GetReferenceProperties(reference.ReferenceId));

            return reference;
        }

        public Reference GetReference(Guid referenceId)
        {
            Reference reference = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@referenceId", referenceId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Reference-GET.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    reference = new Reference();

                    reference.ProviderRecordId = row.Field<String>("ProviderRecordID");
                    reference.ReferenceId = row.Field<Guid>("ReferenceID");
                    reference.AddedDate = row.Field<DateTime?>("AddedDate");
                    reference.ConsensusReferenceId = row.Field<Guid?>("ConsensusReferenceId");
                    reference.IntegrationBatchId = row.Field<Guid?>("IntegrationBatchId");
                    reference.DataSourceId = row.Field<Guid>("DataSourceId");
                    reference.LinkStatus = row.Field<String>("LinkStatus");
                    reference.MatchScore = row.Field<int?>("MatchScore");
                    reference.MatchPath = row.Field<String>("MatchPath");
                    reference.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    reference.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    reference.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    reference.ReferenceTypeId = row.Field<Guid>("ReferenceTypeId");

                    reference.ReferenceProperties.AddRange(GetReferenceProperties(reference.ReferenceId));
                    
                }
            }

            return reference;
        }

        public List<ReferenceProperty> GetReferenceProperties(Guid referenceId)
        {
            List<ReferenceProperty> props = new List<ReferenceProperty>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@referenceId", referenceId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.ReferenceProperty-LIST.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    ReferenceProperty prop = new ReferenceProperty();

                    prop.ReferencePropertyId = row.Field<Guid>("ReferencePropertyId");
                    prop.ReferencePropertyTypeId = row.Field<Guid>("ReferencePropertyTypeId");
                    prop.ReferencePropertyType = row.Field<String>("ReferencePropertyType");
                    prop.Level = row.Field<int?>("Level");
                    prop.Sequence = row.Field<int?>("Sequence");
                    prop.SubType = row.Field<String>("SubType");
                    prop.Value = row.Field<String>("Value");

                    props.Add(prop);
                }
            }

            return props;
        }

        public void Save(Reference reference)
        {
            String sql = String.Empty;

            if (reference.State == Entities.Entity.EntityState.Added)
            {
                sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Reference-INSERT.sql");
            }
            else if (reference.State == Entities.Entity.EntityState.Modified)
            {
                sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Reference-UPDATE.sql");
            }

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@ReferenceID", reference.ReferenceId);

                    cmd.Parameters.AddWithValue("@ReferenceTypeID", reference.ReferenceTypeId);
                    cmd.Parameters.AddWithValue("@DataSourceID", reference.DataSourceId);

                    cmd.Parameters.AddWithValue("@consensusReferenceId", (object)reference.ConsensusReferenceId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@integrationBatchId", (object)reference.IntegrationBatchId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@linkStatus", (object)reference.LinkStatus ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@matchScore", (object)reference.MatchScore ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@matchPath", (object)reference.MatchPath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProviderRecordID", (object)reference.ProviderRecordId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProviderCreatedDate", (object)reference.ProviderCreatedDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProviderModifiedDate", (object)reference.ProviderModifiedDate ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@AddedDate", (object)reference.AddedDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModifiedDate", (object)reference.ModifiedDate ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }

            if (reference.State != Entities.Entity.EntityState.Added)
            {
                DeleteReferenceProperties(reference);
            }

            InsertReferenceProperties(reference);
        }

        public void Save()
        {
            foreach (Reference reference in _references.Where(o => o.State == Entities.Entity.EntityState.Added || o.State == Entities.Entity.EntityState.Modified))
            {
                Save(reference);
            }
        }

        private void InsertReferenceProperties(Reference reference)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.ReferenceProperty-INSERT.sql");

            try
            {
                using (SqlConnection cnn = new SqlConnection(_connectionString))
                {
                    cnn.Open();

                    foreach (ReferenceProperty referenceProperty in reference.ReferenceProperties)
                    {
                        using (SqlCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandType = System.Data.CommandType.Text;
                            cmd.CommandText = sql;

                            cmd.Parameters.AddWithValue("@ReferencePropertyID", referenceProperty.ReferencePropertyId);

                            cmd.Parameters.AddWithValue("@ReferenceID", reference.ReferenceId);
                            cmd.Parameters.AddWithValue("@ReferencePropertyTypeID", referenceProperty.ReferencePropertyTypeId);

                            cmd.Parameters.AddWithValue("@SubType", (object)referenceProperty.SubType ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Sequence", (object)referenceProperty.Sequence ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Level", (object)referenceProperty.Level ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Value", (object)referenceProperty.Value ?? DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        private void DeleteReferenceProperties(Reference reference)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.ReferenceProperties-DELETE.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@ReferenceID", reference.ReferenceId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
