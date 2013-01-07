using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Data.Helpers;

namespace NZOR.Publish.Data.Repositories
{
    public class VocabularyRepository
    {
        private List<Vocabulary> _vocabularies;

        public VocabularyRepository(string dataSourceFileFullName)
        {
            _vocabularies = DataSourceHelper.DeserializeDataSource<Vocabulary>(dataSourceFileFullName);
        }

        public List<Vocabulary> GetAll()
        {
            return _vocabularies;
        }
    }
}
