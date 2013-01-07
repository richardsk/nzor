INSERT INTO
	provider.ConceptRelationship
	(
	ConceptRelationshipID, 
	
	FromConceptID, 
	ToConceptID, 
	ConceptRelationshipTypeID, 
	
	Sequence, 
	InUse	
	)
VALUES
	(
	@ConceptRelationshipID, 
	
	@FromConceptID, 
	@ToConceptID, 
	@ConceptRelationshipTypeID, 
	
	@Sequence, 
	@InUse	
	)
