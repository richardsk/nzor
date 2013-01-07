CREATE TABLE 
	[provider].[ConceptApplication]
	(
	ConceptApplicationID UNIQUEIDENTIFIER NOT NULL, 
	
	FromConceptID UNIQUEIDENTIFIER NOT NULL, 
	ToConceptID UNIQUEIDENTIFIER NOT NULL, 
	ConceptApplicationTypeID UNIQUEIDENTIFIER NOT NULL, 
	
	Gender nvarchar(100) null,
	PartOfTaxon nvarchar(100) null,
	LifeStage nvarchar(100) null,
	GeoRegion nvarchar(500) null,
	GeographicSchema nvarchar(500) null,
		
	InUse BIT NULL, 
	
	AddedDate DATETIME NULL,
	ModifiedDate DATETIME NULL
	)
