ALTER TABLE 
	dbo.[GeoRegion]
ADD CONSTRAINT 
	[frkGeoRegionGeographicSchema] 
FOREIGN KEY 
	(
	GeographicSchemaID
	)
REFERENCES 
	dbo.GeographicSchema 
	(
	GeographicSchemaID
	)	

