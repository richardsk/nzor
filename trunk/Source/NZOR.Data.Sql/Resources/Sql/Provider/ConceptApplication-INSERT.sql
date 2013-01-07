INSERT INTO
	provider.ConceptApplication
	(
	ConceptApplicationID, 
	
	FromConceptID, 
	ToConceptID,
	ConceptApplicationTypeID, 
	
	Gender,
	PartOfTaxon,
	LifeStage,
	GeoRegion,
	GeographicSchema, 
	
	InUse, 
	
	AddedDate, 
	ModifiedDate
	)
VALUES
	(
	@ConceptApplicationID, 
	
	@FromConceptID, 
	@ToConceptID, 
	@ConceptApplicationTypeID, 
	
	@Gender, 
	@PartOfTaxon, 
	@LifeStage,
	@GeoRegion,
	@GeographicSchema, 
	
	@InUse, 
	
	@AddedDate, 
	@ModifiedDate
	)
