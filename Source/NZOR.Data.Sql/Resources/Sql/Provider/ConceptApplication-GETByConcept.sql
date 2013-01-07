SELECT 
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
From provider.ConceptApplication
where FromConceptId = @conceptId