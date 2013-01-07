CREATE TABLE 
	[consensus].[ConceptRelationship]
	(
	ConceptRelationshipID UNIQUEIDENTIFIER NOT NULL, 
	
	FromConceptID UNIQUEIDENTIFIER NOT NULL, 
	ToConceptID UNIQUEIDENTIFIER NOT NULL, 
	ConceptRelationshipTypeID UNIQUEIDENTIFIER NOT NULL, 
	
	IsActive bit not null,
	Sequence INT NULL, 
	
	AddedDate DATETIME NULL, 
	ModifiedDate DATETIME NULL
	)
