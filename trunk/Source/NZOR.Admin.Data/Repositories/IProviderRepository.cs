using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Admin.Data.Entities;

namespace NZOR.Admin.Data.Repositories
{
    public interface IProviderRepository
    {
        Provider GetProvider(Guid providerId);
        Provider GetProviderByCode(string code);
        List<Provider> GetProviders();
        List<DataSource> GetAllDataSources();
        DataSource GetDataSource(Guid dataSourceId);
        DataSource GetDataSourceByCode(string code);
        List<DataSourceEndpoint> GetDatasetEndpoints(Guid dataSourceId);
        List<ProviderStatistics> ListProviderStatistics(string nzorCnnStr, string adminCnnStr);
        List<AttachmentPoint> GetAllAttachmentPoints();
        List<AttachmentPointDataSource> GetAttachmentPointDataSources();
        List<Provider> GetProvidersForName(Guid consensusNameId);

        void Save();
        void SaveDataSourceEndpoint(DataSourceEndpoint dse);
    }
}
