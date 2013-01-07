using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Common;
using System.Data.SqlClient;
using NZOR.Publish.Publisher.Helpers;

namespace NZOR.Publish.Publisher.Builders
{
    class TaxonPropertiesBuilder
    {
        string _connectionString;

        List<TaxonProperty> _taxonProperties;

        public TaxonPropertiesBuilder(string connectionString)
        {
            _connectionString = connectionString;

            _taxonProperties = new List<TaxonProperty>();
        }

        public void Build()
        {
            LoadTaxonProperties();
        }

        public List<TaxonProperty> TaxonProperties
        {
            get { return _taxonProperties; }
        }

        private void LoadTaxonProperties()
        {
            string sql = @"

SELECT
	TaxonPropertyType.TaxonPropertyTypeId AS TaxonPropertyId,

	TaxonPropertyClass.Name AS Class,

	TaxonPropertyType.Name AS Name,
	TaxonPropertyType.Description AS Description
FROM 

	dbo.TaxonPropertyType
	INNER JOIN dbo.TaxonPropertyClass
		ON TaxonPropertyType.TaxonPropertyClassId = TaxonPropertyClass.TaxonPropertyClassId
ORDER BY
	Class,
	Name

";

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    var taxonProperty = new TaxonProperty();

                    taxonProperty.TaxonPropertyId = drd.GetGuid("TaxonPropertyId");

                    taxonProperty.Class = drd.GetString("Class");

                    taxonProperty.Name = drd.GetString("Name");
                    taxonProperty.Description = drd.GetString("Description");

                    _taxonProperties.Add(taxonProperty);
                }
            }

            sql = @"
;WITH CTE
    (
    TaxonPropertyLookupId,
    ParentTaxonPropertyLookupId,
    Level
    )
AS
    (
    -- Anchor member
    SELECT
        TaxonPropertyLookupId,
        ParentTaxonPropertyLookupId,
        0
    FROM
        dbo.TaxonPropertyLookup
    WHERE
        ParentTaxonPropertyLookupID IS NULL
        AND TaxonPropertyTypeId = @TaxonPropertyTypeId

    UNION ALL

    -- Recursive member
    SELECT
        TaxonPropertyLookup.TaxonPropertyLookupId,
        TaxonPropertyLookup.ParentTaxonPropertyLookupId,
        Level + 1
    FROM
        CTE
        INNER JOIN dbo.TaxonPropertyLookup
            ON CTE.TaxonPropertyLookupId = TaxonPropertyLookup.ParentTaxonPropertyLookupId
    )

-- Outer query
SELECT
    TaxonPropertyLookup.TaxonPropertyLookupId,
    TaxonPropertyLookup.ParentTaxonPropertyLookupId,
	TaxonPropertyLookup.Value,
	TaxonPropertyLookup.AlternativeStrings,
    Level
FROM
    CTE
    INNER JOIN dbo.TaxonPropertyLookup
        ON CTE.TaxonPropertyLookupId = TaxonPropertyLookup.TaxonPropertyLookupId
ORDER BY
	Level,
	Value

";
            foreach (var taxonProperty in _taxonProperties)
            {
                var taxonPropertyParameter = new SqlParameter("@TaxonPropertyTypeId", taxonProperty.TaxonPropertyId);

                using (var drd = DataAccess.ExecuteReader(_connectionString, sql, new List<SqlParameter> { taxonPropertyParameter }))
                {
                    while (drd.Read())
                    {
                        var taxonPropertyLookup = new TaxonPropertyLookup();

                        taxonPropertyLookup.TaxonPropertyLookupId = drd.GetGuid("TaxonPropertyLookupId");

                        taxonPropertyLookup.ParentTaxonPropertyLookupId = drd.GetNullableGuid("ParentTaxonPropertyLookupId");

                        taxonPropertyLookup.Value = drd.GetString("Value");
                        taxonPropertyLookup.AlternativeStrings = drd.GetString("AlternativeStrings");

                        taxonProperty.TaxonPropertyLookups.Add(taxonPropertyLookup);
                    }
                }
            }
        }
    }
}
