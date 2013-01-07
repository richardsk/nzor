ALTER TABLE 
	[provider].[ConceptRelationship]
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

