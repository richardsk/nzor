using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using NZOR.Data.Entities.Provider;

namespace NZOR.Data.Repositories.Provider
{
    public interface INameRepository
    {
        List<Name> Names { get; }

        Name GetName(Guid nameId);
        List<Name> GetNames(Guid dataSourceId);
        List<Name> GetNamesForConsensusName(Guid consensusNameId);
        List<NameProperty> GetNamePropertiesForConsensusName(Guid consensusNameId, Guid nameProprtyTypeId);
        Name GetNameByProviderId(string dataSourceCode, String providerRecordId);
        List<Name> GetNamesModifiedSince(DateTime fromDate);
        List<Name> GetNamesForBrokeredName(Guid brokeredNameId);

        void Save();
        void Save(Name name);

        bool CanUnintegrateName(Name name);

        void DeleteName(Guid nameId);
    }
}
