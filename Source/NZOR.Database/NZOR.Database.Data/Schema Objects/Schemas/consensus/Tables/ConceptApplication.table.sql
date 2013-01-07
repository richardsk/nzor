CREATE TABLE 
	[consensus].[ConceptApplication]
	(
	ConceptApplicationID UNIQUEIDENTIFIER NOT NULL, 
	
	FromConceptID UNIQUEIDENTIFIER NOT NULL, 
	ToConceptID UNIQUEIDENTIFIER NOT NULL, 
	ConceptApplicationTypeID UNIQUEIDENTIFIER NOT NULL, 
	
	Gender nvarchar(100) null,
	PartOfTaxon nvarchar(100) null,
	LifeStage nvarchar(100) null,
	GeoRegionID uniqueidentifier null,
	GeographicSchemaID uniqueidentifier null,
	
	AddedDate DATETIME NULL, 
	ModifiedDate DATETIME NULL
	)
