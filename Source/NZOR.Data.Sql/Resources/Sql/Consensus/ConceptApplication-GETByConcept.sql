SELECT 
	ca.ConceptApplicationID, 
	
	ca.FromConceptID, 
	ca.ToConceptID,
	ca.ConceptApplicationTypeID, 
	
	ca.Gender,
	ca.PartOfTaxon,
	ca.LifeStage,
	ca.GeoRegionID,
	ca.GeographicSchemaID, 
	gr.Name as GeoRegion,
	gs.Name as GeographicSchema,
		
	ca.AddedDate, 
	ca.ModifiedDate
From consensus.ConceptApplication ca
left join dbo.GeoRegion gr on gr.GeoRegionId = ca.GeoRegionId
left join dbo.GeographicSchema gs on gs.GeographicSchemaId = ca.GeographicSchemaId
where FromConceptId = @conceptId