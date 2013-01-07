using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Data.Helpers;

namespace NZOR.Publish.Data.Repositories
{
    public class GeographicSchemaRepository
    {
        private List<GeographicSchema> _geographicSchemas;

        public GeographicSchemaRepository(string dataSourceFileFullName)
        {
            _geographicSchemas = DataSourceHelper.DeserializeDataSource<GeographicSchema>(dataSourceFileFullName);
        }

        public List<GeographicSchema> GetAll()
        {
            return _geographicSchemas;
        }

        public GeographicSchema SingleOrDefault(Guid id)
        {
            return _geographicSchemas.SingleOrDefault(o => o.GeographicSchemaId == id);
        }
    }
}
