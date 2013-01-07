using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using NZOR.Data.Entities.Provider;
using NZOR.Data.Repositories.Provider;

namespace NZOR.Data.Sql.Repositories.Provider
{
    public class TaxonPropertyRepository : Repository<TaxonProperty>, ITaxonPropertyRepository
    {
        private List<TaxonProperty> _taxonProperties;

        public TaxonPropertyRepository(String connectionString)
            : base(connectionString)
        {
            _taxonProperties = new List<TaxonProperty>();
        }

        public List<TaxonProperty> TaxonProperties
        {
            get { return _taxonProperties; }
        }

        public List<TaxonProperty> GetTaxonProperties(Guid dataSourceId)
        {
            List<TaxonProperty> tpList = new List<TaxonProperty>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@dataSourceId", dataSourceId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.TaxonProperty-GetByDataSource.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    TaxonProperty tp = new TaxonProperty();
                    tp.TaxonPropertyId = row.Field<Guid>("TaxonPropertyID");
                    tp.TaxonPropertyClassId = row.Field<Guid>("TaxonPropertyClassID");
                    tp.TaxonPropertyClass = row.Field<String>("TaxonPropertyClass");
                    tp.ConsensusTaxonPropertyId = row.Field<Guid?>("ConsensusTaxonPropertyId");
                    tp.DataSourceId = row.Field<Guid>("DataSourceId");
                    tp.LinkStatus = row.Field<String>("LinkStatus");
                    tp.MatchScore = row.Field<int?>("MatchScore");
                    tp.ProviderConceptId = row.Field<String>("ProviderConceptId");
                    tp.ProviderNameId = row.Field<String>("ProviderNameId");
                    tp.ProviderReferenceId = row.Field<String>("ProviderReferenceId");
                    tp.ProviderRecordId = row.Field<String>("ProviderRecordId");
                    tp.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    tp.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");

                    tp.InUse = row.Field<bool?>("InUse");
                    tp.ReferenceId = row.Field<Guid?>("ReferenceId");
                    tp.ReferenceCitation = row.Field<String>("ReferenceCitation");
                    tp.NameId = row.Field<Guid?>("NameID");
                    tp.ConceptId = row.Field<Guid?>("ConceptID");

                    tp.AddedDate = row.Field<DateTime?>("AddedDate");
                    tp.ModifiedDate = row.Field<DateTime?>("ModifiedDate");

                    tp.TaxonPropertyValues.AddRange(GetTaxonPropertyValues(tp.TaxonPropertyId));

                    tpList.Add(tp);
                }
            }

            return tpList;
        }

        public List<TaxonProperty> GetTaxonPropertiesByName(Guid nameId)
        {
            List<TaxonProperty> tpList = new List<TaxonProperty>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@nameId", nameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.TaxonProperty-GetByName.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    TaxonProperty tp = new TaxonProperty();
                    tp.TaxonPropertyId = row.Field<Guid>("TaxonPropertyID");
                    tp.TaxonPropertyClassId = row.Field<Guid>("TaxonPropertyClassID");
                    tp.TaxonPropertyClass = row.Field<String>("TaxonPropertyClass");
                    tp.ConsensusTaxonPropertyId = row.Field<Guid?>("ConsensusTaxonPropertyId");
                    tp.DataSourceId = row.Field<Guid>("DataSourceId");
                    tp.LinkStatus = row.Field<String>("LinkStatus");
                    tp.MatchScore = row.Field<int?>("MatchScore");
                    tp.ProviderConceptId = row.Field<String>("ProviderConceptId");
                    tp.ProviderNameId = row.Field<String>("ProviderNameId");
                    tp.ProviderReferenceId = row.Field<String>("ProviderReferenceId");
                    tp.ProviderRecordId = row.Field<String>("ProviderRecordId");
                    tp.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    tp.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    
                    tp.InUse = row.Field<bool?>("InUse");
                    tp.ReferenceId = row.Field<Guid?>("ReferenceId");
                    tp.ReferenceCitation = row.Field<String>("ReferenceCitation");
                    tp.NameId = row.Field<Guid?>("NameID");
                    tp.ConceptId = row.Field<Guid?>("ConceptID");

                    tp.AddedDate = row.Field<DateTime?>("AddedDate");
                    tp.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    
                    tp.TaxonPropertyValues.AddRange(GetTaxonPropertyValues(tp.TaxonPropertyId));

                    tpList.Add(tp);
                }
            }

            return tpList;
        }

        public List<TaxonPropertyValue> GetTaxonPropertyValues(Guid taxonPropertyId)
        {
            List<TaxonPropertyValue> tpvList = new List<TaxonPropertyValue>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@taxonPropertyId", taxonPropertyId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.TaxonPropertyValue-GetByTaxonProperty.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    TaxonPropertyValue tpv = new TaxonPropertyValue();
                    tpv.TaxonPropertyValueId = row.Field<Guid>("TaxonPropertyValueID");
                    tpv.TaxonPropertyId = row.Field<Guid>("TaxonPropertyID");
                    tpv.TaxonPropertyTypeId = row.Field<Guid>("TaxonPropertyTypeID");
                    tpv.TaxonPropertyType = row.Field<String>("TaxonPropertyType");
                    tpv.Value = row.Field<String>("Value");

                    tpvList.Add(tpv);
                }
            }

            return tpvList;

        }

        public List<TaxonProperty> GetTaxonPropertiesByConsensusName(Guid consensusNameId)
        {
            List<TaxonProperty> tpList = new List<TaxonProperty>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@consensusNameId", consensusNameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.TaxonProperty-GetByConsensusName.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    TaxonProperty tp = new TaxonProperty();
                    tp.TaxonPropertyId = row.Field<Guid>("TaxonPropertyID");
                    tp.TaxonPropertyClassId = row.Field<Guid>("TaxonPropertyClassID");
                    tp.TaxonPropertyClass = row.Field<String>("TaxonPropertyClass");
                    tp.ConsensusTaxonPropertyId = row.Field<Guid?>("ConsensusTaxonPropertyId");
                    tp.DataSourceId = row.Field<Guid>("DataSourceId");
                    tp.LinkStatus = row.Field<String>("LinkStatus");
                    tp.MatchScore = row.Field<int?>("MatchScore");
                    tp.ProviderConceptId = row.Field<String>("ProviderConceptId");
                    tp.ProviderNameId = row.Field<String>("ProviderNameId");
                    tp.ProviderReferenceId = row.Field<String>("ProviderReferenceId");
                    tp.ProviderRecordId = row.Field<String>("ProviderRecordId");
                    tp.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    tp.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");

                    tp.InUse = row.Field<bool?>("InUse");
                    tp.ReferenceId = row.Field<Guid?>("ReferenceId");
                    tp.ReferenceCitation = row.Field<String>("ReferenceCitation");
                    tp.NameId = row.Field<Guid?>("NameID");
                    tp.ConceptId = row.Field<Guid?>("ConceptID");

                    tp.AddedDate = row.Field<DateTime?>("AddedDate");
                    tp.ModifiedDate = row.Field<DateTime?>("ModifiedDate");

                    tp.TaxonPropertyValues.AddRange(GetTaxonPropertyValues(tp.TaxonPropertyId));

                    tpList.Add(tp);
                }
            }

            return tpList;
        }

        public void Save()
        {
            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                foreach (TaxonProperty tp in TaxonProperties)
                {
                    String sql = String.Empty;

                    if (tp.State == Entities.Entity.EntityState.Added)
                    {
                        sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.TaxonProperty-INSERT.sql");
                    }
                    else if (tp.State == Entities.Entity.EntityState.Modified)
                    {
                        sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.TaxonProperty-UPDATE.sql");
                    }

                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@TaxonPropertyID", tp.TaxonPropertyId);

                        cmd.Parameters.AddWithValue("@TaxonPropertyClassID", tp.TaxonPropertyClassId);
                        cmd.Parameters.AddWithValue("@DataSourceID", tp.DataSourceId);
                        cmd.Parameters.AddWithValue("@consensusTaxonPropertyId", (object)tp.ConsensusTaxonPropertyId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@linkStatus", (object)tp.LinkStatus ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@matchScore", (object)tp.MatchScore ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@ProviderRecordID", tp.ProviderRecordId);
                        cmd.Parameters.AddWithValue("@providerConceptId", (object)tp.ProviderConceptId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@conceptId", (object)tp.ConceptId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@providerNameId", (object)tp.ProviderNameId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@nameId", (object)tp.NameId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@providerReferenceId", (object)tp.ProviderReferenceId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@referenceId", (object)tp.ReferenceId ?? DBNull.Value);                        
                        cmd.Parameters.AddWithValue("@ProviderCreatedDate", (object)tp.ProviderCreatedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProviderModifiedDate", (object)tp.ProviderModifiedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@inUse", (object)tp.InUse ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@AddedDate", (object)tp.AddedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedDate", (object)tp.ModifiedDate ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }

                    if (tp.State != Entities.Entity.EntityState.Added)
                    {
                        DeleteTaxonPropertyValues(cnn, tp);
                    }

                    InsertTaxonPropertyValues(cnn, tp);
                }
            }
        }

        private void DeleteTaxonPropertyValues(SqlConnection cnn, TaxonProperty tp)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.TaxonPropertyValue-DELETE.sql");

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("@taxonPropertyId", tp.TaxonPropertyId);

                cmd.ExecuteNonQuery();
            }
        }

        private void InsertTaxonPropertyValues(SqlConnection cnn, TaxonProperty tp)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.TaxonPropertyValue-INSERT.sql");

            foreach (TaxonPropertyValue tpv in tp.TaxonPropertyValues)
            {
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@taxonPropertyValueID", tpv.TaxonPropertyValueId);
                    cmd.Parameters.AddWithValue("@taxonPropertyID", tp.TaxonPropertyId);
                    cmd.Parameters.AddWithValue("@taxonPropertyTypeID", tpv.TaxonPropertyTypeId);
                    cmd.Parameters.AddWithValue("@Value", (object)tpv.Value ?? DBNull.Value);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
