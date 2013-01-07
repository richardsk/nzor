INSERT INTO
	consensus.ConceptApplication
	(
	ConceptApplicationID, 
	
	FromConceptID, 
	ToConceptID,
	ConceptApplicationTypeID, 
	
	Gender,
	PartOfTaxon,
	LifeStage,
	GeoRegionID,
	GeographicSchemaID, 
	
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
	@GeoRegionID,
	@GeographicSchemaID,
	
	@AddedDate, 
	@ModifiedDate
	)
