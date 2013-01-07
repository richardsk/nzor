using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Data.Helpers;

namespace NZOR.Publish.Data.Repositories
{
    public class TaxonPropertyRepository
    {
        private List<TaxonProperty> _taxonProperties;

        public TaxonPropertyRepository(string dataSourceFileFullName)
        {
            _taxonProperties = DataSourceHelper.DeserializeDataSource<TaxonProperty>(dataSourceFileFullName);
        }

        public List<TaxonProperty> GetAll()
        {
            return _taxonProperties;
        }

        public TaxonProperty SingleOrDefault(Guid id)
        {
            return _taxonProperties.SingleOrDefault(o => o.TaxonPropertyId == id);
        }
    }
}
