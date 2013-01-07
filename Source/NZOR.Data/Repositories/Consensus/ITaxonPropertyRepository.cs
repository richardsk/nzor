using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Data.Entities.Consensus;

namespace NZOR.Data.Repositories.Consensus
{
    public interface ITaxonPropertyRepository
    {
        List<TaxonProperty> TaxonProperties { get; }

        List<TaxonProperty> GetTaxonPropertiesByName(Guid nameId);
        List<TaxonPropertyValue> GetTaxonPropertyValues(Guid taxonPropertyId);

        void Save();
    }
}
