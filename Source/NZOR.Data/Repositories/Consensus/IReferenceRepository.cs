using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using NZOR.Data.Entities.Consensus;

namespace NZOR.Data.Repositories.Consensus
{
    public interface IReferenceRepository
    {
        List<Reference> References { get; }

        Reference GetReference(Guid referenceId);

        void Save();
        void Save(Reference reference);
    }
}
