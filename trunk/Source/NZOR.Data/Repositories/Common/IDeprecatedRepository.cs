using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Repositories.Common
{
    public interface IDeprecatedRepository
    {
        void InsertDeprecatedRecord(string table, Guid oldId, Guid? newId, DateTime deprecationDate);
    }
}
