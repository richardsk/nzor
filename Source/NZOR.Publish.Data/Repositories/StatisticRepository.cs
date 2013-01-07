using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Administration;
using NZOR.Publish.Data.Helpers;

namespace NZOR.Publish.Data.Repositories
{
    public class StatisticRepository
    {
        private List<Statistic> _statistics;

        public StatisticRepository(string dataSourceFileFullName)
        {
            _statistics = DataSourceHelper.DeserializeDataSource<Statistic>(dataSourceFileFullName);
        }

        public List<Statistic> GetAll()
        {
            return _statistics;
        }
    }
}
