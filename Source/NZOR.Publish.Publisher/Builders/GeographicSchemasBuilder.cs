using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Common;
using System.Data.SqlClient;
using NZOR.Publish.Publisher.Helpers;

namespace NZOR.Publish.Publisher.Builders
{
    class GeographicSchemasBuilder
    {
        string _connectionString;

        List<GeographicSchema> _geographicSchemas;

        public GeographicSchemasBuilder(string connectionString)
        {
            _connectionString = connectionString;

            _geographicSchemas = new List<GeographicSchema>();
        }

        public void Build()
        {
            LoadGeographicSchemas();
        }

        public List<GeographicSchema> GeographicSchemas
        {
            get { return _geographicSchemas; }
        }

        private void LoadGeographicSchemas()
        {
            string sql = @"

SELECT
	GeographicSchemaId,

	Name,
	Description
FROM
	dbo.GeographicSchema

";

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    var geographicSchema = new GeographicSchema();

                    geographicSchema.GeographicSchemaId = drd.GetGuid("GeographicSchemaId");

                    geographicSchema.Name = drd.GetString("Name");
                    geographicSchema.Description = drd.GetString("Description");

                    _geographicSchemas.Add(geographicSchema);
                }
            }

            sql = @"
;WITH CTE
    (
    GeoRegionId,
    ParentGeoRegionId,
    Level
    )
AS
    (
    -- Anchor member
    SELECT
        GeoRegionId,
        ParentGeoRegionId,
        0
    FROM
        dbo.GeoRegion
    WHERE
        ParentGeoRegionID IS NULL
        AND GeographicSchemaId = @GeographicSchemaId

    UNION ALL

    -- Recursive member
    SELECT
        GeoRegion.GeoRegionId,
        GeoRegion.ParentGeoRegionId,
        Level + 1
    FROM
        CTE
        INNER JOIN dbo.GeoRegion
            ON CTE.GeoRegionId = GeoRegion.ParentGeoRegionId
    )

-- Outer query
SELECT
    GeoRegion.GeoRegionId,
    GeoRegion.ParentGeoRegionId,
	GeoRegion.Name,
	GeoRegion.Language,
    Level
FROM
    CTE
    INNER JOIN dbo.GeoRegion
        ON CTE.GeoRegionId = GeoRegion.GeoRegionId
ORDER BY
	Level,
	Name


";
            foreach (var geographicSchema in _geographicSchemas)
            {
                var geographicSchemaParameter = new SqlParameter("@GeographicSchemaId", geographicSchema.GeographicSchemaId);

                using (var drd = DataAccess.ExecuteReader(_connectionString, sql, new List<SqlParameter> { geographicSchemaParameter }))
                {
                    while (drd.Read())
                    {
                        var georegion = new GeoRegion();

                        georegion.GeoRegionId = drd.GetGuid("GeoregionId");

                        georegion.ParentGeoRegionId = drd.GetNullableGuid("ParentGeoregionId");

                        georegion.Name = drd.GetString("Name");
                        georegion.Language = drd.GetString("Language");

                        geographicSchema.GeoRegions.Add(georegion);
                    }
                }
            }
        }
    }
}
