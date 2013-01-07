using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Provider;
using System.Data.SqlClient;

namespace NZOR.Data.Repositories.Provider
{
    public interface IReferenceRepository
    {
        List<Reference> References { get; }

        Reference GetReference(Guid referenceId);
        List<Reference> GetReferences(Guid dataSourceId);
        List<Reference> GetReferencesForConcensusReference(Guid consensusReferenceId);

        Reference GetDataSourceReference(Guid dataSourceId, DateTime dataSourceDate);
        Reference CreateDataSourceReference(Guid dataSourceId, DateTime dataSourceDate);

        void Save();
        void Save(Reference reference);
    }
}
