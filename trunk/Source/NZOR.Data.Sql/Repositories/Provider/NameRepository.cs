using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Provider;
using NZOR.Data.Repositories.Provider;

namespace NZOR.Data.Sql.Repositories.Provider
{
    public class NameRepository : Repository<Name>, INameRepository
    {
        private List<Name> _names;

        public NameRepository(String connectionString)
            : base(connectionString)
        {
            _names = new List<Name>();
        }

        public List<Name> Names
        {
            get { return _names; }
        }

        public Name GetName(Guid nameId)
        {
            Name n = null;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@nameId", nameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-GET.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    n = new Name();
                    n.NameId = row.Field<Guid>("NameID");
                    n.AddedDate = row.Field<DateTime?>("AddedDate");
                    n.ConsensusNameId = row.Field<Guid?>("ConsensusNameId");
                    n.DataSourceId = row.Field<Guid>("DataSourceID");
                    n.DataSourceName = row.Field<String>("DataSourceName");
                    n.FullName = row.Field<String>("FullName");
                    n.GoverningCode = row.Field<String>("GoverningCode");
                    n.IsRecombination = row.Field<bool?>("IsRecombination");
                    n.LinkStatus = row.Field<String>("LinkStatus");
                    n.MatchPath = row.Field<String>("MatchPath");
                    n.MatchScore = row.Field<int?>("MatchScore");
                    n.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    n.NameClassId = row.Field<Guid>("NameClassId");
                    n.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    n.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    n.ProviderRecordId = row.Field<String>("ProviderRecordId");
                    n.ProviderName = row.Field<String>("ProviderName");
                    n.ProviderId = row.Field<Guid>("ProviderId");
                    n.TaxonRankId = row.Field<Guid>("TaxonRankId");

                    n.NameProperties.AddRange(GetNameProperties(n.NameId));                    
                }
            }

            return n;
        }

        public Name GetNameByProviderId(string dataSourceCode, String providerRecordId)
        {
            Name n = null;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@providerRecordId", providerRecordId));
            parameters.Add(new SqlParameter("@dataSourceCode", dataSourceCode));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-GETByProviderId.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    n = new Name();
                    n.NameId = row.Field<Guid>("NameID");
                    n.AddedDate = row.Field<DateTime?>("AddedDate");
                    n.ConsensusNameId = row.Field<Guid?>("ConsensusNameId");
                    n.DataSourceId = row.Field<Guid>("DataSourceID");
                    n.DataSourceName = row.Field<String>("DataSourceName");
                    n.FullName = row.Field<String>("FullName");
                    n.GoverningCode = row.Field<String>("GoverningCode");
                    n.IsRecombination = row.Field<bool?>("IsRecombination");
                    n.LinkStatus = row.Field<String>("LinkStatus");
                    n.MatchPath = row.Field<String>("MatchPath");
                    n.MatchScore = row.Field<int?>("MatchScore");
                    n.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    n.NameClassId = row.Field<Guid>("NameClassId");
                    n.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    n.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    n.ProviderRecordId = row.Field<String>("ProviderRecordId");
                    n.ProviderName = row.Field<String>("ProviderName");
                    n.ProviderId = row.Field<Guid>("ProviderId");
                    n.TaxonRankId = row.Field<Guid>("TaxonRankId");

                    n.NameProperties.AddRange(GetNameProperties(n.NameId));
                }
            }

            return n;
        }

        private List<NameProperty> GetNameProperties(Guid nameId)
        {
            List<NameProperty> props = new List<NameProperty>();
            
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@nameId", nameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.NameProperties-LIST.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    NameProperty np = new NameProperty();

                    np.NamePropertyId = row.Field<Guid>("NamePropertyID");
                    np.NamePropertyTypeId = row.Field<Guid>("NamePropertyTypeID");
                    np.NamePropertyType = row.Field<String>("NamePropertyType");
                    np.ProviderRelatedId = row.Field<String>("ProviderRelatedID");
                    np.RelatedId = row.Field<Guid?>("RelatedID");
                    np.Sequence = row.Field<int?>("Sequence");
                    np.Value = row.Field<String>("Value");

                    props.Add(np);
                }
            }

            return props;
        }

        public List<Name> GetNamesModifiedSince(DateTime fromDate)
        {
            List<Name> names = new List<Name>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@fromDate", fromDate));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-LISTModifiedSince.sql"), parameters, 5000))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Name n = new Name();
                    n.NameId = row.Field<Guid>("NameID");
                    n.AddedDate = row.Field<DateTime?>("AddedDate");
                    n.ConsensusNameId = row.Field<Guid?>("ConsensusNameId");
                    n.DataSourceId = row.Field<Guid>("DataSourceID");
                    n.DataSourceName = row.Field<String>("DataSourceName");
                    n.FullName = row.Field<String>("FullName");
                    n.GoverningCode = row.Field<String>("GoverningCode");
                    n.IsRecombination = row.Field<bool?>("IsRecombination");
                    n.LinkStatus = row.Field<String>("LinkStatus");
                    n.MatchPath = row.Field<String>("MatchPath");
                    n.MatchScore = row.Field<int?>("MatchScore");
                    n.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    n.NameClassId = row.Field<Guid>("NameClassId");
                    n.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    n.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    n.ProviderRecordId = row.Field<String>("ProviderRecordId");
                    n.ProviderName = row.Field<String>("ProviderName");
                    n.ProviderId = row.Field<Guid>("ProviderId");
                    n.TaxonRankId = row.Field<Guid>("TaxonRankId");

                    n.NameProperties.AddRange(GetNameProperties(n.NameId));

                    names.Add(n);
                }
            }

            return names;
        }
    
        public List<Name> GetNames(Guid dataSourceId)
        {
            List<Name> names = new List<Name>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@DataSourceID", dataSourceId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-LIST.sql"), parameters, 5000))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Name n = new Name();
                    n.NameId = row.Field<Guid>("NameID");
                    n.AddedDate = row.Field<DateTime?>("AddedDate");
                    n.ConsensusNameId = row.Field<Guid?>("ConsensusNameId");
                    n.DataSourceId = row.Field<Guid>("DataSourceID");
                    n.DataSourceName = row.Field<String>("DataSourceName");
                    n.FullName = row.Field<String>("FullName");
                    n.GoverningCode = row.Field<String>("GoverningCode");
                    n.IsRecombination = row.Field<bool?>("IsRecombination");
                    n.LinkStatus = row.Field<String>("LinkStatus");
                    n.MatchPath = row.Field<String>("MatchPath");
                    n.MatchScore = row.Field<int?>("MatchScore");
                    n.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    n.NameClassId = row.Field<Guid>("NameClassId");
                    n.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    n.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    n.ProviderRecordId = row.Field<String>("ProviderRecordId");
                    n.ProviderName = row.Field<String>("ProviderName");
                    n.ProviderId = row.Field<Guid>("ProviderId");
                    n.TaxonRankId = row.Field<Guid>("TaxonRankId");

                    n.NameProperties.AddRange(GetNameProperties(n.NameId));

                    names.Add(n);
                }
            }

            return names;
        }

        public List<Name> GetNamesForConsensusName(Guid consensusNameId)
        {
            List<Name> names = new List<Name>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@consensusNameId", consensusNameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-LISTByConsensusName.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Name n = new Name();
                    n.NameId = row.Field<Guid>("NameID");
                    n.AddedDate = row.Field<DateTime?>("AddedDate");
                    n.ConsensusNameId = row.Field<Guid?>("ConsensusNameId");
                    n.DataSourceId = row.Field<Guid>("DataSourceID");
                    n.DataSourceName = row.Field<String>("DataSourceName");
                    n.FullName = row.Field<String>("FullName");
                    n.GoverningCode = row.Field<String>("GoverningCode");
                    n.IsRecombination = row.Field<bool?>("IsRecombination");
                    n.LinkStatus = row.Field<String>("LinkStatus");
                    n.MatchPath = row.Field<String>("MatchPath");
                    n.MatchScore = row.Field<int?>("MatchScore");
                    n.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    n.NameClassId = row.Field<Guid>("NameClassId");
                    n.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    n.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    n.ProviderRecordId = row.Field<String>("ProviderRecordId");
                    n.ProviderName = row.Field<String>("ProviderName");
                    n.ProviderId = row.Field<Guid>("ProviderId");
                    n.TaxonRankId = row.Field<Guid>("TaxonRankId");

                    n.NameProperties.AddRange(GetNameProperties(n.NameId));

                    names.Add(n);
                }
            }

            return names;
        }

        public List<NameProperty> GetNamePropertiesForConsensusName(Guid consensusNameId, Guid nameProprtyTypeId)
        {
            List<NameProperty> props = new List<NameProperty>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@consensusNameId", consensusNameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.NameProperties-LISTByConsensusName.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    NameProperty np = new NameProperty();

                    np.NamePropertyId = row.Field<Guid>("NamePropertyID");
                    np.NamePropertyTypeId = row.Field<Guid>("NamePropertyTypeID");
                    np.NamePropertyType = row.Field<String>("NamePropertyType");
                    np.ProviderRelatedId = row.Field<String>("ProviderRelatedID");
                    np.RelatedId = row.Field<Guid?>("RelatedID");
                    np.Sequence = row.Field<int?>("Sequence");
                    np.Value = row.Field<String>("Value");

                    props.Add(np);
                }
            }

            return props;
        }

        public void Save(Name name)
        {
            String sql = String.Empty;

            if (name.State == Entities.Entity.EntityState.Added)
            {
                sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-INSERT.sql");
            }
            else if (name.State == Entities.Entity.EntityState.Modified)
            {
                sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-UPDATE.sql");
            }

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@NameID", name.NameId);

                    cmd.Parameters.AddWithValue("@TaxonRankID", name.TaxonRankId);
                    cmd.Parameters.AddWithValue("@NameClassID", name.NameClassId);
                    cmd.Parameters.AddWithValue("@DataSourceID", name.DataSourceId);
                    cmd.Parameters.AddWithValue("@consensusNameId", (object)name.ConsensusNameId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@linkStatus", (object)name.LinkStatus ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@matchScore", (object)name.MatchScore ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@matchPath", (object)name.MatchPath ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@ProviderRecordID", name.ProviderRecordId);
                    cmd.Parameters.AddWithValue("@ProviderCreatedDate", (object)name.ProviderCreatedDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProviderModifiedDate", (object)name.ProviderModifiedDate ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@FullName", name.FullName);
                    cmd.Parameters.AddWithValue("@GoverningCode", (object)name.GoverningCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsRecombination", (object)name.IsRecombination ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@AddedDate", (object)name.AddedDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModifiedDate", (object)name.ModifiedDate ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }

            if (name.State != Entities.Entity.EntityState.Added)
            {
                DeleteNameProperties(name);
            }

            InsertNameProperties(name);
        }

        public void Save()
        {
            foreach (Name name in _names.Where(o => o.State == Entities.Entity.EntityState.Added || o.State == Entities.Entity.EntityState.Modified))
            {
                Save(name);
            }
        }

        private void InsertNameProperties(Name name)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.NameProperty-INSERT.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                foreach (NameProperty nameProperty in name.NameProperties)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@NamePropertyID", nameProperty.NamePropertyId);

                        cmd.Parameters.AddWithValue("@NameID", name.NameId);
                        cmd.Parameters.AddWithValue("@NamePropertyTypeID", nameProperty.NamePropertyTypeId);
                        cmd.Parameters.AddWithValue("@RelatedID", (object)nameProperty.RelatedId ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@ProviderRelatedID", (object)nameProperty.ProviderRelatedId ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@Sequence", (object)nameProperty.Sequence ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Value", (object)nameProperty.Value ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void DeleteNameProperties(Name name)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.NameProperties-DELETE.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@NameID", name.NameId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool CanUnintegrateName(Name name)
        {
            bool canUnintegrate = true;
            
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-CanUnintegrate.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@NameID", name.NameId);

                    canUnintegrate = (bool)cmd.ExecuteScalar();
                }

                cnn.Close();
            }

            return canUnintegrate;
        }

        public void DeleteName(Guid nameId)
        {
            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-DELETE.sql");

                using (SqlCommand cmd = new SqlCommand(sql, cnn))
                {
                    cmd.Parameters.AddWithValue("@nameId", nameId);
                    cmd.ExecuteNonQuery();
                }
            }

        }

        public List<Name> GetNamesForBrokeredName(Guid brokeredNameId)
        {
            List<Name> names = new List<Name>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@brokeredNameId", brokeredNameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-LISTByBrokeredName.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Name n = new Name();
                    n.NameId = row.Field<Guid>("NameID");
                    n.AddedDate = row.Field<DateTime?>("AddedDate");
                    n.ConsensusNameId = row.Field<Guid?>("ConsensusNameId");
                    n.DataSourceId = row.Field<Guid>("DataSourceID");
                    n.DataSourceName = row.Field<String>("DataSourceName");
                    n.FullName = row.Field<String>("FullName");
                    n.GoverningCode = row.Field<String>("GoverningCode");
                    n.IsRecombination = row.Field<bool?>("IsRecombination");
                    n.LinkStatus = row.Field<String>("LinkStatus");
                    n.MatchPath = row.Field<String>("MatchPath");
                    n.MatchScore = row.Field<int?>("MatchScore");
                    n.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    n.NameClassId = row.Field<Guid>("NameClassId");
                    n.ProviderCreatedDate = row.Field<DateTime?>("ProviderCreatedDate");
                    n.ProviderModifiedDate = row.Field<DateTime?>("ProviderModifiedDate");
                    n.ProviderRecordId = row.Field<String>("ProviderRecordId");
                    n.ProviderName = row.Field<String>("ProviderName");
                    n.ProviderId = row.Field<Guid>("ProviderId");
                    n.TaxonRankId = row.Field<Guid>("TaxonRankId");

                    n.NameProperties.AddRange(GetNameProperties(n.NameId));

                    names.Add(n);
                }
            }

            return names;
        }

    }
}
