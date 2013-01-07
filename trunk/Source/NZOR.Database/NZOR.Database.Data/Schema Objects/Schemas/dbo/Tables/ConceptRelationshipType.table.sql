CREATE TABLE 
	[dbo].[ConceptRelationshipType]
	(
	ConceptRelationshipTypeID UNIQUEIDENTIFIER NOT NULL, 
	
	Relationship NVARCHAR(50) NOT NULL, 
	MinOccurrences INT NULL, 
	MaxOccurrences INT NULL
	)