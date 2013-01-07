using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

using NZOR.Admin.Data;
using NZOR.Admin.Data.Entities;
using NZOR.Admin.Data.Sql.Helpers;

namespace NZOR.Admin.Data.Sql.Repositories
{
    public class ProviderRepository : Repository<Provider>, NZOR.Admin.Data.Repositories.IProviderRepository
    {
        private List<Provider> _providers = null;
        
        public ProviderRepository(String connectionString)
            : base(connectionString)
        {
        }

        public Provider GetProvider(Guid providerId)
        {
            if (_providers == null)
            {
                _providers = GetProviders();
            }

            foreach (Provider p in _providers)
            {
                if (p.ProviderId == providerId)
                {
                    return p;
                }
            }
            return null;
        }

        public List<Provider> Providers
        {
            get
            {
                if (_providers == null)
                {
                    _providers = GetProviders();
                }
                return _providers;
            }
        }

        public Provider GetProviderByCode(string code)
        {
            Provider provider = null;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@code", code));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Provider-GET.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    provider = new Provider();

                    provider.ProviderId = row.Field<Guid>("ProviderId");
                    provider.Name = row.Field<String>("Name");
                    provider.Code = row.Field<String>("Code");
                    provider.Url = row.IsNull("Url") ? null : row.Field<String>("Url");
                    provider.ContactEmail = row.IsNull("ContactEmail") ? null : row.Field<String>("ContactEmail");
                    provider.Disclaimer = row.IsNull("Disclaimer") ? null : row.Field<String>("Disclaimer");
                    provider.Attribution = row.IsNull("Attribution") ? null : row.Field<String>("Attribution");
                    provider.Licensing = row.IsNull("Licensing") ? null : row.Field<String>("Licensing");
                    provider.PublicStatement = row.IsNull("PublicStatement") ? null : row.Field<String>("PublicStatement");
                    provider.AddedDate = row.IsNull("AddedDate") ? null : row.Field<DateTime?>("AddedDate");
                    provider.AddedBy = row.IsNull("AddedBy") ? null : row.Field<String>("AddedBy");
                    provider.ModifiedDate = row.IsNull("ModifiedDate") ? null : row.Field<DateTime?>("ModifiedDate");
                    provider.ModifiedBy = row.IsNull("ModifiedBy") ? null : row.Field<String>("ModifiedBy");

                    provider.DataSources = GetDataSources(provider.ProviderId);

                    provider.AttachmentPoints = GetAttachmentPoints(provider.ProviderId);
                }
            }

            return provider;
        }

        public List<Provider> GetProviders()
        {
            List<Provider> providers = new List<Provider>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Provider-LIST.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Provider p = new Provider();

                    p.ProviderId = row.Field<Guid>("ProviderId");
                    p.Name = row.Field<String>("Name");
                    p.Code = row.Field<String>("Code");
                    p.Url = row.IsNull("Url") ? null : row.Field<String>("Url");
                    p.ContactEmail = row.IsNull("ContactEmail") ? null : row.Field<String>("ContactEmail");
                    p.Disclaimer = row.IsNull("Disclaimer") ? null : row.Field<String>("Disclaimer");
                    p.Attribution = row.IsNull("Attribution") ? null : row.Field<String>("Attribution");
                    p.Licensing = row.IsNull("Licensing") ? null : row.Field<String>("Licensing");
                    p.PublicStatement = row.IsNull("PublicStatement") ? null : row.Field<String>("PublicStatement");
                    p.AddedDate = row.IsNull("AddedDate") ? null : row.Field<DateTime?>("AddedDate");
                    p.AddedBy = row.IsNull("AddedBy") ? null : row.Field<String>("AddedBy");
                    p.ModifiedDate = row.IsNull("ModifiedDate") ? null : row.Field<DateTime?>("ModifiedDate");
                    p.ModifiedBy = row.IsNull("ModifiedBy") ? null : row.Field<String>("ModifiedBy");

                    p.DataSources = GetDataSources(p.ProviderId);

                    p.AttachmentPoints = GetAttachmentPoints(p.ProviderId);
                    
                    providers.Add(p);
                }
            }

            return providers;
        }

        public DataSource GetDataSourceByCode(string code)
        {
            DataSource ds = null;

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.DataSource-GET.sql"), new List<SqlParameter> { new SqlParameter("@dataSourceCode", code) }))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    ds = new DataSource();
                    ds.DataSourceId = row.Field<Guid>("DataSourceId");
                    ds.ProviderId = row.Field<Guid>("ProviderId");
                    ds.Name = row.Field<String>("Name");
                    ds.Code = row.Field<String>("Code");
                    ds.Description = row.IsNull("Description") ? null : row.Field<String>("Description");
                    ds.AddedDate = row.Field<DateTime>("AddedDate");
                    ds.AddedBy = row.Field<String>("AddedBy");
                    ds.ModifiedDate = row.IsNull("ModifiedDate") ? null : row.Field<DateTime?>("ModifiedDate");
                    ds.ModifiedBy = row.IsNull("ModifiedBy") ? null : row.Field<String>("ModifiedBy");
                }
            }

            return ds;
        }
        
        public DataSource GetDataSource(Guid dataSourceId)
        {
            DataSource ds = null;

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.DataSource-GETById.sql"), new List<SqlParameter> { new SqlParameter("@dataSourceId", dataSourceId) }))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    ds = new DataSource();
                    ds.DataSourceId = row.Field<Guid>("DataSourceId");
                    ds.ProviderId = row.Field<Guid>("ProviderId");
                    ds.Name = row.Field<String>("Name");
                    ds.Code = row.Field<String>("Code");
                    ds.Description = row.IsNull("Description") ? null : row.Field<String>("Description");
                    ds.AddedDate = row.Field<DateTime>("AddedDate");
                    ds.AddedBy = row.Field<String>("AddedBy");
                    ds.ModifiedDate = row.IsNull("ModifiedDate") ? null : row.Field<DateTime?>("ModifiedDate");
                    ds.ModifiedBy = row.IsNull("ModifiedBy") ? null : row.Field<String>("ModifiedBy");
                }
            }

            return ds;
        }

        public List<DataSource> GetAllDataSources()
        {
            List<DataSource> dsList = new List<DataSource>();
            
            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.DataSource-LISTAll.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    DataSource ds = new DataSource();
                    ds.DataSourceId = row.Field<Guid>("DataSourceId");
                    ds.ProviderId = row.Field<Guid>("ProviderId");
                    ds.Name = row.Field<String>("Name");
                    ds.Code = row.Field<String>("Code");
                    ds.Description = row.IsNull("Description") ? null : row.Field<String>("Description");
                    ds.AddedDate = row.Field<DateTime>("AddedDate");
                    ds.AddedBy = row.Field<String>("AddedBy");
                    ds.ModifiedDate = row.IsNull("ModifiedDate") ? null : row.Field<DateTime?>("ModifiedDate");
                    ds.ModifiedBy = row.IsNull("ModifiedBy") ? null : row.Field<String>("ModifiedBy");

                    dsList.Add(ds);
                }
            }

            return dsList;
        }

        public List<Provider> GetProvidersForName(Guid consensusNameId)
        {
            List<Provider> providers = new List<Provider>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@consensusNameId", consensusNameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Provider-LISTForNameId.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Provider p = new Provider();

                    p.ProviderId = row.Field<Guid>("ProviderId");
                    p.Name = row.Field<String>("Name");
                    p.Code = row.Field<String>("Code");
                    p.Url = row.IsNull("Url") ? null : row.Field<String>("Url");
                    p.ContactEmail = row.IsNull("ContactEmail") ? null : row.Field<String>("ContactEmail");
                    p.Disclaimer = row.IsNull("Disclaimer") ? null : row.Field<String>("Disclaimer");
                    p.Attribution = row.IsNull("Attribution") ? null : row.Field<String>("Attribution");
                    p.Licensing = row.IsNull("Licensing") ? null : row.Field<String>("Licensing");
                    p.PublicStatement = row.IsNull("PublicStatement") ? null : row.Field<String>("PublicStatement");
                    p.AddedDate = row.IsNull("AddedDate") ? null : row.Field<DateTime?>("AddedDate");
                    p.AddedBy = row.IsNull("AddedBy") ? null : row.Field<String>("AddedBy");
                    p.ModifiedDate = row.IsNull("ModifiedDate") ? null : row.Field<DateTime?>("ModifiedDate");
                    p.ModifiedBy = row.IsNull("ModifiedBy") ? null : row.Field<String>("ModifiedBy");

                    p.DataSources = GetDataSources(p.ProviderId);

                    p.AttachmentPoints = GetAttachmentPoints(p.ProviderId);

                    providers.Add(p);
                }
            }

            return providers;
        }

        private List<AttachmentPoint> GetAttachmentPoints(Guid providerId)
        {
            List<AttachmentPoint> attPoints = new List<AttachmentPoint>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@providerID", providerId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.AttachmentPoint-LIST.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    AttachmentPoint ap = new AttachmentPoint();
                    ap.AttachmentPointId = row.Field<Guid>("AttachmentPointId");
                    ap.ProviderId = row.Field<Guid>("ProviderId");
                    ap.DataSourceId = row.Field<Guid>("DataSourceId");
                    ap.ProviderRecordId = row.Field<String>("ProviderRecordId");
                    ap.ConsensusNameId = row.Field<Guid>("ConsensusNameId");
                    ap.FullName = row.Field<String>("FullName");
                    ap.AddedDate = row.Field<DateTime>("AddedDate");
                    ap.AddedBy = row.Field<String>("AddedBy");
                    if (!row.IsNull("ModifiedDate")) ap.ModifiedDate = row.Field<DateTime>("ModifiedDate");
                    ap.ModifiedBy = row.Field<String>("ModifiedBy");

                    attPoints.Add(ap);
                }
            }

            return attPoints;
        }

        public List<AttachmentPointDataSource> GetAttachmentPointDataSources()
        {
            List<AttachmentPointDataSource> attPointsDS = new List<AttachmentPointDataSource>();
            
            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.AttachmentPointDataSource-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    AttachmentPointDataSource apds = new AttachmentPointDataSource();
                    apds.AttachmentPointId = row.Field<Guid>("AttachmentPointId");
                    apds.DataSourceId = row.Field<Guid>("DataSourceId");
                    apds.Ranking = row.Field<int>("Ranking");

                    attPointsDS.Add(apds);
                }
            }

            return attPointsDS;
        }

        public List<AttachmentPoint> GetAllAttachmentPoints()
        {
            List<AttachmentPoint> attPoints = new List<AttachmentPoint>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.AttachmentPoint-LIST-All.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    AttachmentPoint ap = new AttachmentPoint();
                    ap.AttachmentPointId = row.Field<Guid>("AttachmentPointId");
                    ap.ProviderId = row.Field<Guid>("ProviderId");
                    ap.DataSourceId = row.Field<Guid>("DataSourceId");
                    ap.ProviderRecordId = row.Field<String>("ProviderRecordId");
                    ap.ConsensusNameId = row.Field<Guid>("ConsensusNameId");
                    ap.FullName = row.Field<String>("FullName");
                    ap.AddedDate = row.Field<DateTime>("AddedDate");
                    ap.AddedBy = row.Field<String>("AddedBy");
                    if (!row.IsNull("ModifiedDate")) ap.ModifiedDate = row.Field<DateTime>("ModifiedDate");
                    ap.ModifiedBy = row.Field<String>("ModifiedBy");

                    attPoints.Add(ap);
                }
            }

            return attPoints;
        }

        private List<DataSource> GetDataSources(Guid providerId)
        {
            List<DataSource> dsList = new List<DataSource>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@providerID", providerId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.DataSource-LIST.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    DataSource ds = new DataSource();
                    ds.DataSourceId = row.Field<Guid>("DataSourceId");
                    ds.ProviderId = row.Field<Guid>("ProviderId");
                    ds.Name = row.Field<String>("Name");
                    ds.Code = row.Field<String>("Code");
                    ds.Description = row.IsNull("Description") ? null : row.Field<String>("Description");
                    ds.AddedDate = row.Field<DateTime>("AddedDate");
                    ds.AddedBy = row.Field<String>("AddedBy");
                    ds.ModifiedDate = row.IsNull("ModifiedDate") ? null : row.Field<DateTime?>("ModifiedDate");
                    ds.ModifiedBy = row.IsNull("ModifiedBy") ? null : row.Field<String>("ModifiedBy");

                    dsList.Add(ds);
                }
            }

            return dsList;
        }

        public List<DataSourceEndpoint> GetDatasetEndpoints(Guid dataSourceId)
        {
            List<DataSourceEndpoint> endpoints = new List<DataSourceEndpoint>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("@datasourceId", dataSourceId));

            NZOR.Admin.Data.Repositories.IScheduledTaskRepository str = new ScheduledTaskRepository(_connectionString);

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.DataSourceEndpoint-LIST.sql"), sqlParameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    DataSourceEndpoint dep = new DataSourceEndpoint();

                    dep.DataSourceEndpointId = row.Field<Guid>("DataSourceEndpointID");
                    dep.DataSourceId = row.Field<Guid>("DataSourceID");
                    dep.DataTypeId = row.Field<Guid>("DataTypeID");
                    dep.DataType = row.Field<String>("DataType");
                    dep.Name = row.Field<String>("Name") ?? null;
                    dep.Description = row.Field<String>("Description") ?? null;
                    dep.Url = row.Field<String>("Url") ?? null;
                    dep.LastHarvestDate = row.Field<DateTime?>("LastHarvestDate");
                    
                    dep.Schedule = str.GetScheduledTask(dep.DataSourceEndpointId);

                    endpoints.Add(dep);
                }
            }

            return endpoints;
        }
        
        public void Save()
        {
            foreach (Provider prov in _providers.Where(o => o.State == Entities.Entity.EntityState.Added || o.State == Entities.Entity.EntityState.Modified))
            {

                using (SqlConnection cnn = new SqlConnection(_connectionString))
                {
                    cnn.Open();
                    using (SqlTransaction trn = cnn.BeginTransaction())
                    {
                        try
                        {
                            Save(trn, prov);

                            trn.Commit();
                        }
                        catch (Exception)
                        {
                            trn.Rollback();

                            throw;
                        }
                    }
                    if (cnn.State != ConnectionState.Closed)
                    {
                        cnn.Close();
                    }
                }

                prov.State = Entities.Entity.EntityState.Unchanged;
            }
        }

        public void Save(SqlTransaction trn, Provider prov)
        {
            String sql = String.Empty;

            if (prov.State == Entities.Entity.EntityState.Added)
            {
                sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Provider-INSERT.sql");
            }
            else if (prov.State == Entities.Entity.EntityState.Modified)
            {
                sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Provider-UPDATE.sql");
            }

            using (SqlCommand cmd = trn.Connection.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("@providerID", prov.ProviderId);
                cmd.Parameters.AddWithValue("@addedBy", prov.AddedBy);
                cmd.Parameters.AddWithValue("@addedDate", (object)prov.AddedDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@attribution", (object)prov.Attribution ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@disclaimer", (object)prov.Disclaimer ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@licensing", (object)prov.Licensing ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@modifiedBy", (object)prov.ModifiedBy ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@modifiedDate", (object)prov.ModifiedDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@name", prov.Name);
                cmd.Parameters.AddWithValue("@code", prov.Code);
                cmd.Parameters.AddWithValue("@publicStatement", (object)prov.PublicStatement ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@url", (object)prov.Url ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@contactEmail", (object)prov.ContactEmail ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }

            SaveDataSources(prov, trn);
        }

        private void SaveDataSources(Provider prov, SqlTransaction trn)
        {
            foreach (DataSource ds in prov.DataSources.Where(o => o.State == Entities.Entity.EntityState.Added || o.State == Entities.Entity.EntityState.Modified))
            {
                String sql = String.Empty;

                if (ds.State == Entities.Entity.EntityState.Added)
                {
                    sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.DataSource-INSERT.sql");
                }
                else if (ds.State == Entities.Entity.EntityState.Modified)
                {
                    sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.DataSource-UPDATE.sql");
                }

                using (SqlCommand cmd = trn.Connection.CreateCommand())
                {
                    cmd.Transaction = trn;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@dataSourceId", ds.DataSourceId);
                    cmd.Parameters.AddWithValue("@addedBy", ds.AddedBy);
                    cmd.Parameters.AddWithValue("@addedDate", ds.AddedDate);
                    cmd.Parameters.AddWithValue("@description", (object)ds.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@modifiedBy", (object)ds.ModifiedBy ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@modifiedDate", (object)ds.ModifiedDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@name", ds.Name);
                    cmd.Parameters.AddWithValue("@code", ds.Code);
                    cmd.Parameters.AddWithValue("@providerId", ds.ProviderId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveDataSourceEndpoint(DataSourceEndpoint dse)
        {
            String sql = String.Empty;

            if (dse.State == Entities.Entity.EntityState.Added)
            {
                sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.DataSourceEndpoint-INSERT.sql");
            }
            else if (dse.State == Entities.Entity.EntityState.Modified)
            {
                sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.DataSourceEndpoint-UPDATE.sql");
            }

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@dataSourceEndpointId", dse.DataSourceEndpointId);
                    cmd.Parameters.AddWithValue("@dataSourceId", dse.DataSourceId);
                    cmd.Parameters.AddWithValue("@dataTypeId", dse.DataTypeId);
                    cmd.Parameters.AddWithValue("@description", (object)dse.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@lastHarvestDate", (object)dse.LastHarvestDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@name", dse.Name);
                    cmd.Parameters.AddWithValue("@url", (object)dse.Url ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<ProviderStatistics> ListProviderStatistics(string nzorCnnStr, string adminCnnStr)
        {
            List<ProviderStatistics> stats = new List<ProviderStatistics>();
            
            DataTable tbl = new DataTable();

            _connectionString = adminCnnStr;

            using (SqlConnection cnn = new SqlConnection(nzorCnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandTimeout = 120;
                    cmd.CommandText = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.ProviderStatistics-LIST.sql");
                    
                    using (SqlDataAdapter dad = new SqlDataAdapter(cmd))
                    {
                        dad.Fill(tbl);
                    }
                }
                cnn.Close();
            }

            foreach (DataRow row in tbl.Rows)
            {
                ProviderStatistics ps = new ProviderStatistics();

                ps.Provider = GetProvider(row.Field<Guid>("ProviderID"));

                if (ps.Provider != null)
                {
                    ps.DataSource = ps.Provider.DataSources.First(o => o.DataSourceId == row.Field<Guid>("DataSourceID"));

                    ps.ProviderNameCount = row.Field<int>("ProviderNameCount");
                    ps.ProviderConceptCount = row.Field<int>("ProviderConceptCount");
                    ps.ProviderReferenceCount = row.Field<int>("ProviderReferenceCount");

                    ps.IntegratedNameCount = row.Field<int>("IntegratedNameCount");
                    ps.IntegratedConceptCount = row.Field<int>("IntegratedConceptCount");
                    ps.IntegratedReferenceCount = row.Field<int>("IntegratedReferenceCount");

                    ps.LastNameUpdatedDate = row.Field<DateTime?>("LastNameUpdatedDate");
                    ps.LastConceptUpdatedDate = row.Field<DateTime?>("LastConceptUpdatedDate");
                    ps.LastReferenceUpdatedDate = row.Field<DateTime?>("LastReferenceUpdatedDate");
                    ps.LastHarvestDate = row.Field<DateTime?>("LastHarvestDate");
                    
                    ps.Issues = GetIssues(ps.DataSource.DataSourceId, nzorCnnStr);

                    stats.Add(ps);
                }
            }

            return stats;
        }

        private List<IntegrationIssue> GetIssues(Guid datasourceId, string cnnStr)
        {
            List<IntegrationIssue> issues = new List<IntegrationIssue>();

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                DataSet ds = new DataSet();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandTimeout = 100;
                    cmd.CommandText = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.IntegrationIssues-GET.sql");
                    cmd.Parameters.AddWithValue("@datasourceId", datasourceId);

                    using (SqlDataAdapter dad = new SqlDataAdapter(cmd))
                    {
                        dad.Fill(ds);

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            IntegrationIssue ii = new IntegrationIssue();
                            ii.RecordType = NZORRecordType.Name;
                            ii.RecordId = row.Field<Guid>("NameId");
                            ii.RecordText = row.Field<String>("FullName");
                            ii.LinkStatus = row.Field<String>("LinkStatus");
                            ii.Message = row.Field<String>("MatchPath");
                            issues.Add(ii);
                        }

                        foreach (DataRow row in ds.Tables[1].Rows)
                        {
                            IntegrationIssue ii = new IntegrationIssue();
                            ii.RecordType = NZORRecordType.Concept;
                            ii.RecordId = row.Field<Guid>("ConceptId");
                            ii.RecordText = row.Field<String>("Text");
                            ii.LinkStatus = row.Field<String>("LinkStatus");                            
                            issues.Add(ii);
                        }

                        foreach (DataRow row in ds.Tables[2].Rows)
                        {
                            IntegrationIssue ii = new IntegrationIssue();
                            ii.RecordType = NZORRecordType.Reference;
                            ii.RecordId = row.Field<Guid>("ReferenceId");
                            ii.RecordText = row.Field<String>("Text");
                            ii.LinkStatus = row.Field<String>("LinkStatus");
                            issues.Add(ii);
                        }
                    }
                }
                cnn.Close();
            }

            return issues;
        }
    }
}
