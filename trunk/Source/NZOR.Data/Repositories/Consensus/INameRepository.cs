using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using NZOR.Data.Entities.Consensus;
using NZOR.Data.Entities.Common;

namespace NZOR.Data.Repositories.Consensus
{
    public interface INameRepository
    {
        List<Name> Names { get; }

        Name GetName(Guid nameId);
        List<Guid> GetAllNames();
        List<Name> GetRelatedNames(Guid nameId);
        List<StackedName> GetStackedNamesForName(Guid nameId);
        List<Name> GetNameChildren(Guid nameId);
        List<Name> GetNameSynonyms(Guid nameId);
        Entities.Consensus.NameProfile GetNameProfile(Guid nameId);
        List<Entities.Consensus.NameProfile> SearchNames(List<SearchField> searchFields, int startIndex, int recordCount, SearchField.OrderByField orderByField, out int totalRecordCount);

        void Save();
        void Save(Name name);
        void SaveNameProperty(Guid nameId, NameProperty nameProperty);
        void SetNamePropertyValue(Guid nameId, Guid namePropertyTypeId, object value, object sequence, object relatedId);
        void UpdateFullName(Name name);

        void DeleteName(Guid nameId, Guid? replacementId);
    }
}
