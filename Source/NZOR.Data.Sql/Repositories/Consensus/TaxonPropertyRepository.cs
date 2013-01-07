using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using NZOR.Data.Entities.Consensus;
using NZOR.Data.Repositories.Consensus;

namespace NZOR.Data.Sql.Repositories.Consensus
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

        public List<TaxonProperty> GetTaxonPropertiesByName(Guid nameId)
        {
            List<TaxonProperty> tpList = new List<TaxonProperty>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@nameId", nameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.TaxonProperty-GetByName.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    TaxonProperty tp = new TaxonProperty();
                    tp.TaxonPropertyId = row.Field<Guid>("TaxonPropertyID");
                    tp.TaxonPropertyClassId = row.Field<Guid>("TaxonPropertyClassID");
                    tp.TaxonPropertyClass = row.Field<String>("TaxonPropertyClass");

                    tp.GeoRegionId = row.Field<Guid?>("GeoRegionId");
                    tp.GeoRegion = row.Field<String>("GeoRegion");
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

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.TaxonPropertyValue-GetByTaxonProperty.sql"), parameters))
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
                        sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.TaxonProperty-INSERT.sql");
                    }
                    else if (tp.State == Entities.Entity.EntityState.Modified)
                    {
                        sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.TaxonProperty-UPDATE.sql");
                    }

                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@TaxonPropertyID", tp.TaxonPropertyId);
                        cmd.Parameters.AddWithValue("@TaxonPropertyClassID", tp.TaxonPropertyClassId);
                        cmd.Parameters.AddWithValue("@conceptId", (object)tp.ConceptId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@nameId", (object)tp.NameId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@referenceId", (object)tp.ReferenceId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@GeoRegionID", (object)tp.GeoRegionId ?? DBNull.Value);                        
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
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.TaxonPropertyValue-DELETE.sql");

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
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.TaxonPropertyValue-INSERT.sql");

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
