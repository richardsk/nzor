CREATE TABLE 
	[provider].[ConceptRelationship]
	(
	ConceptRelationshipID UNIQUEIDENTIFIER NOT NULL, 
	
	FromConceptID UNIQUEIDENTIFIER NULL, 
	ToConceptID UNIQUEIDENTIFIER NULL, 
	ConceptRelationshipTypeID UNIQUEIDENTIFIER NULL, 
	
	Sequence INT NULL, 
	InUse BIT NULL
	)
