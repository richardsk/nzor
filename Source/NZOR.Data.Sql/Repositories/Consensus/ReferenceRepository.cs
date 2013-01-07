using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Consensus;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using NZOR.Data.Repositories.Consensus;

namespace NZOR.Data.Sql.Repositories.Consensus
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

        public Reference GetReference(Guid referenceId)
        {
            Reference reference = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@referenceId", referenceId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Reference-GET.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    reference = new Reference();

                    reference.ReferenceId = row.Field<Guid>("ReferenceID");
                    reference.ReferenceTypeId = row.Field<Guid>("ReferenceTypeId");
                    reference.AddedDate = row.Field<DateTime?>("AddedDate");
                    reference.ModifiedDate = row.Field<DateTime?>("ModifiedDate");

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

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.ReferenceProperty-LIST.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    ReferenceProperty prop = new ReferenceProperty();

                    prop.ReferencePropertyId = row.Field<Guid>("ReferencePropertyId");
                    prop.ReferencePropertyTypeId = row.Field<Guid>("ReferencePropertyTypeId");
                    prop.ReferencePropertyType = row.Field<String>("ReferencePropertyType");
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
                sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Reference-INSERT.sql");
            }
            else if (reference.State == Entities.Entity.EntityState.Modified)
            {
                sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Reference-UPDATE.sql");
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
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.ReferenceProperty-INSERT.sql");

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

                        cmd.Parameters.AddWithValue("@Value", (object)referenceProperty.Value ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void DeleteReferenceProperties(Reference reference)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.ReferenceProperties-DELETE.sql");

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
