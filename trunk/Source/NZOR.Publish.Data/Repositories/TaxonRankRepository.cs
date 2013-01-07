using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Data.Helpers;

namespace NZOR.Publish.Data.Repositories
{
    public class TaxonRankRepository
    {
        private List<TaxonRank> _taxonRanks;

        public TaxonRankRepository(string dataSourceFileFullName)
        {
            _taxonRanks = DataSourceHelper.DeserializeDataSource<TaxonRank>(dataSourceFileFullName);
        }

        public List<TaxonRank> GetAll()
        {
            return _taxonRanks;
        }

        public TaxonRank SingleOrDefault(Guid id)
        {
            return _taxonRanks.SingleOrDefault(o => o.TaxonRankId == id);
        }
    }
}
