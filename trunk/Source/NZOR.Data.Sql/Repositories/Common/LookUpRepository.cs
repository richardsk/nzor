using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Common;
using NZOR.Data.Repositories.Common;
using System.Data;
using System.Data.SqlClient;

namespace NZOR.Data.Sql.Repositories.Common
{
    public class LookUpRepository : ILookUpRepository
    {
        private String _connectionString;

        public LookUpRepository(String connectionString)
        {
            _connectionString = connectionString;
        }

        public List<NamePropertyType> GetNamePropertyTypes()
        {
            List<NamePropertyType> namePropertyTypes = new List<NamePropertyType>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Common.NamePropertyType-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    NamePropertyType namePropertyType = new NamePropertyType();

                    namePropertyType.NamePropertyTypeId = row.Field<Guid>("NamePropertyTypeID");
                    namePropertyType.NameClassId = row.Field<Guid>("NameClassID");
                    namePropertyType.Name = row.Field<String>("Name") ?? String.Empty;
                    namePropertyType.MinOccurrences = row.Field<int?>("MinOccurrences");
                    namePropertyType.MaxOccurrences = row.Field<int?>("MaxOccurrences");
                    namePropertyType.GoverningCode = row.Field<String>("GoverningCode") ?? String.Empty;

                    namePropertyTypes.Add(namePropertyType);
                }
            }

            return namePropertyTypes;
        }

        public List<NamePropertyType> GetNamePropertyTypes(Guid classID)
        {
            List<NamePropertyType> namePropertyTypes = new List<NamePropertyType>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Common.NamePropertyTypeByClass-LIST.sql"), new List<SqlParameter> {new SqlParameter("@nameClassID", classID)}))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    NamePropertyType namePropertyType = new NamePropertyType();

                    namePropertyType.NamePropertyTypeId = row.Field<Guid>("NamePropertyTypeID");
                    namePropertyType.NameClassId = row.Field<Guid>("NameClassID");
                    namePropertyType.Name = row.Field<String>("Name") ?? String.Empty;
                    namePropertyType.MinOccurrences = row.Field<int?>("MinOccurrences");
                    namePropertyType.MaxOccurrences = row.Field<int?>("MaxOccurrences");
                    namePropertyType.GoverningCode = row.Field<String>("GoverningCode") ?? String.Empty;

                    namePropertyTypes.Add(namePropertyType);
                }
            }

            return namePropertyTypes;
        }

        /// <summary>
        /// Taxon Ranks listed by sort order (highest rank in classification first, down to infraspecific ranks).
        /// </summary>
        /// <returns></returns>
        public List<TaxonRank> GetTaxonRanks()
        {
            List<TaxonRank> taxonRanks = new List<TaxonRank>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Common.TaxonRank-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    TaxonRank taxonRank = new TaxonRank();

                    taxonRank.TaxonRankId = row.Field<Guid>("TaxonRankID");

                    taxonRank.Name = row.Field<String>("Name");
                    taxonRank.DisplayName = row.Field<string>("DisplayName");
                    taxonRank.KnownAbbreviations = row.Field<string>("KnownAbbreviations") ?? string.Empty;
                    taxonRank.SortOrder = row.Field<Int32>("SortOrder");
                    if (!row.IsNull("MatchRuleSetId")) taxonRank.MatchRuleSetId = row.Field<Int32>("MatchRuleSetId");
                    if (!row.IsNull("IncludeInFullName")) taxonRank.IncludeInFullName = row.Field<bool?>("IncludeInFullName");
                    if (!row.IsNull("ShowRank")) taxonRank.ShowRank = row.Field<bool?>("ShowRank");
                    if (!row.IsNull("GoverningCode")) taxonRank.GoverningCode = row.Field<string>("GoverningCode");

                    taxonRanks.Add(taxonRank);
                }
            }

            return taxonRanks;
        }

        public List<NameClass> GetNameClasses()
        {
            List<NameClass> nameClasses = new List<NameClass>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Common.NameClass-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    NameClass nameClass = new NameClass();

                    nameClass.NameClassId = row.Field<Guid>("NameClassID");

                    nameClass.Code = row.Field<String>("Name");
                    nameClass.Name = row.Field<String>("Name");
                    nameClass.HasClassification = row.Field<bool>("HasClassification");

                    nameClasses.Add(nameClass);
                }
            }

            return nameClasses;
        }


        public List<ReferenceType> GetReferenceTypes()
        {
            List<ReferenceType> referenceTypes = new List<ReferenceType>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL(@"NZOR.Data.Sql.Resources.Sql.Common.ReferenceType-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    ReferenceType referenceType = new ReferenceType();

                    referenceType.ReferenceTypeId = row.Field<Guid>("ReferenceTypeID");

                    referenceType.Name = row.Field<String>("Name");

                    referenceTypes.Add(referenceType);
                }
            }

            return referenceTypes;
        }

        public List<ReferencePropertyType> GetReferencePropertyTypes()
        {
            List<ReferencePropertyType> referencePropertyTypes = new List<ReferencePropertyType>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL(@"NZOR.Data.Sql.Resources.Sql.Common.ReferencePropertyType-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    ReferencePropertyType referencePropertyType = new ReferencePropertyType();

                    referencePropertyType.ReferencePropertyTypeId = row.Field<Guid>("ReferencePropertyTypeID");

                    referencePropertyType.Name = row.Field<String>("Name");

                    referencePropertyTypes.Add(referencePropertyType);
                }
            }

            return referencePropertyTypes;
        }

        public List<ConceptRelationshipType> GetConceptRelationshipTypes()
        {
            List<ConceptRelationshipType> conceptRelationshipTypes = new List<ConceptRelationshipType>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL(@"NZOR.Data.Sql.Resources.Sql.Common.ConceptRelationshipType-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    ConceptRelationshipType conceptRelationshipType = new ConceptRelationshipType();

                    conceptRelationshipType.ConceptRelationshipTypeId = row.Field<Guid>("ConceptRelationshipTypeID");

                    conceptRelationshipType.Relationship = row.Field<String>("Relationship");
                    conceptRelationshipType.MaxOccurrences = row.Field<int?>("MaxOccurrences");
                    conceptRelationshipType.MinOccurrences = row.Field<int?>("MinOccurrences");

                    conceptRelationshipTypes.Add(conceptRelationshipType);
                }
            }

            return conceptRelationshipTypes;
        }

        public List<ConceptApplicationType> GetConceptApplicationTypes()
        {
            List<ConceptApplicationType> conceptApplicationTypes = new List<ConceptApplicationType>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL(@"NZOR.Data.Sql.Resources.Sql.Common.ConceptApplicationType-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    ConceptApplicationType conceptApplicationType = new ConceptApplicationType();

                    conceptApplicationType.ConceptApplicationTypeId = row.Field<Guid>("ConceptApplicationTypeID");

                    conceptApplicationType.Name = row.Field<String>("Name");

                    conceptApplicationTypes.Add(conceptApplicationType);
                }
            }

            return conceptApplicationTypes;
        }

        public List<TaxonPropertyClass> GetTaxonPropertyClasses()
        {
            List<TaxonPropertyClass> classes = new List<TaxonPropertyClass>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL(@"NZOR.Data.Sql.Resources.Sql.Common.TaxonPropertyClass-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    TaxonPropertyClass cl = new TaxonPropertyClass();

                    cl.TaxonPropertyClassId = row.Field<Guid>("TaxonPropertyClassId");

                    cl.Name = row.Field<String>("Name");
                    cl.Description = row.Field<String>("Description");

                    classes.Add(cl);
                }
            }

            return classes;
        }

        public List<TaxonPropertyType> GetTaxonPropertyTypes()
        {
            List<TaxonPropertyType> propertyTypes = new List<TaxonPropertyType>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL(@"NZOR.Data.Sql.Resources.Sql.Common.TaxonPropertyType-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    TaxonPropertyType pt = new TaxonPropertyType();

                    pt.TaxonPropertyTypeId = row.Field<Guid>("TaxonPropertyTypeId");
                    pt.TaxonPropertyClassId = row.Field<Guid>("TaxonPropertyClassId");

                    pt.Name = row.Field<String>("Name");
                    pt.Description = row.Field<String>("Description");

                    propertyTypes.Add(pt);
                }
            }

            return propertyTypes;
        }

        
        public List<GeoRegion> GetGeoRegions()
        {
            List<GeoRegion> geoRegions = new List<GeoRegion>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL(@"NZOR.Data.Sql.Resources.Sql.Common.GeoRegion-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    GeoRegion region = new GeoRegion();

                    region.GeoRegionId = row.Field<Guid>("GeoRegionID");
                    region.GeographicSchemaId = row.Field<Guid>("GeographicSchemaID");
                    region.CorrectGeoRegionId = row.Field<Guid?>("CorrectGeoRegionID");
                    region.ParentGeoRegionId = row.Field<Guid?>("ParentGeoRegionID");
                    region.Name = row.Field<String>("Name");
                    region.GeographicSchemaName = row.Field<string>("GeographicSchemaName");
                    region.Language = row.Field<string>("Language");
                    
                    geoRegions.Add(region);
                }
            }

            return geoRegions;
        }

        public GeoRegion GetGeoRegionByName(string name)
        {
            GeoRegion region = null;

            if (name != null)
            {
                using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL(@"NZOR.Data.Sql.Resources.Sql.Common.GeoRegion-LIST.sql")))
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        string rName = row.Field<string>("Name");

                        if (rName != null && rName.ToLower() == name.ToLower())
                        {
                            region = new GeoRegion();
                            region.GeoRegionId = row.Field<Guid>("GeoRegionID");
                            region.GeographicSchemaId = row.Field<Guid>("GeographicSchemaID");
                            region.CorrectGeoRegionId = row.Field<Guid?>("CorrectGeoRegionID");
                            region.ParentGeoRegionId = row.Field<Guid?>("ParentGeoRegionID");
                            region.Name = row.Field<String>("Name");
                            region.GeographicSchemaName = row.Field<string>("GeographicSchemaName");
                            region.Language = row.Field<string>("Language");

                            break;
                        }
                    }
                }
            }

            return region;
        }
    }
}
