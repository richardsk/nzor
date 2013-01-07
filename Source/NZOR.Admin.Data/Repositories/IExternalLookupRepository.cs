using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Admin.Data.Entities;

namespace NZOR.Admin.Data.Repositories
{
    public interface IExternalLookupRepository
    {
        ExternalLookupService GetLookupService(Guid id);
        List<ExternalLookupService> ListLookupServices();
    }
}
