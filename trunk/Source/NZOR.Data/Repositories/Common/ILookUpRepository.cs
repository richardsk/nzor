using System;
using NZOR.Data.Entities.Common;
using System.Collections.Generic;

namespace NZOR.Data.Repositories.Common
{
    public interface ILookUpRepository
    {
        List<NameClass> GetNameClasses();
        List<NamePropertyType> GetNamePropertyTypes();
        List<NamePropertyType> GetNamePropertyTypes(Guid classID);
        List<ReferencePropertyType> GetReferencePropertyTypes();
        List<ReferenceType> GetReferenceTypes();
        List<TaxonRank> GetTaxonRanks();
        List<ConceptRelationshipType> GetConceptRelationshipTypes();
        List<ConceptApplicationType> GetConceptApplicationTypes();        
        List<GeoRegion> GetGeoRegions();
        GeoRegion GetGeoRegionByName(string name);
        List<TaxonPropertyClass> GetTaxonPropertyClasses();
        List<TaxonPropertyType> GetTaxonPropertyTypes();
    }
}
