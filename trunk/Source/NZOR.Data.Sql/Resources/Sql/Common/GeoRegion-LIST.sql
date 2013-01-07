SELECT g.GeoRegionID,
	g.GeographicSchemaID,
	g.CorrectGeoRegionID,
	g.ParentGeoRegionID,
	g.Name,
	g.Language,
	grs.Name as GeographicSchemaName
FROM dbo.GeoRegion g
inner join dbo.GeographicSchema grs on grs.GeographicSchemaID = g.GeographicSchemaID