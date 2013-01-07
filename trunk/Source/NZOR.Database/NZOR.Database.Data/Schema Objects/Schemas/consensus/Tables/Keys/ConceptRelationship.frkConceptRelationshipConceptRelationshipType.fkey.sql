ALTER TABLE 
	[consensus].[ConceptRelationship]
ADD CONSTRAINT 
	[frkConceptRelationshipConceptRelationshipType] 
FOREIGN KEY 
	(
	ConceptRelationshipTypeID
	)
REFERENCES 
	ConceptRelationshipType
	(
	ConceptRelationshipTypeID
	)	

