using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Data.Helpers;

namespace NZOR.Publish.Data.Repositories
{
    public class DeprecatedRecordRepository
    {
        private List<DeprecatedRecord> _deprecatedRecords;

        public DeprecatedRecordRepository(string dataSourceFileFullName)
        {
            _deprecatedRecords = DataSourceHelper.DeserializeDataSource<DeprecatedRecord>(dataSourceFileFullName);
        }

        public List<DeprecatedRecord> GetAll()
        {
            return _deprecatedRecords;
        }
    }
}
