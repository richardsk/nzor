using System;
using System.Collections.Generic;
using System.Linq;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Data.Helpers;

namespace NZOR.Publish.Data.Repositories
{
    public class ProviderRepository
    {
        private List<Provider> _providers;

        public ProviderRepository(string dataSourceFileFullName)
        {
            _providers = DataSourceHelper.DeserializeDataSource<Provider>(dataSourceFileFullName);
        }

        public List<Provider> GetAll()
        {
            return _providers;
        }

        public Provider SingleOrDefault(Guid id)
        {
            return _providers.SingleOrDefault(o => o.ProviderId == id);
        }
    }
}
