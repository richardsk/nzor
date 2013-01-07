using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Data.Entities.Provider;

namespace NZOR.Data.Repositories.Provider
{
    public interface ITaxonPropertyRepository
    {
        List<TaxonProperty> TaxonProperties { get; }

        List<TaxonProperty> GetTaxonProperties(Guid dataSourceId);
        List<TaxonProperty> GetTaxonPropertiesByName(Guid nameId);
        List<TaxonPropertyValue> GetTaxonPropertyValues(Guid taxonPropertyId);

        List<TaxonProperty> GetTaxonPropertiesByConsensusName(Guid consensusNameId);

        void Save();
    }
}
