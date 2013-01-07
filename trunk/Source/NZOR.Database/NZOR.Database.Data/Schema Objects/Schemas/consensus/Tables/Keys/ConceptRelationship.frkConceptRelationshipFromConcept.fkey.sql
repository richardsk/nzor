ALTER TABLE 
	[consensus].[ConceptRelationship]
ADD CONSTRAINT 
	[frkConceptRelationshipFromConcept] 
FOREIGN KEY 
	(
	FromConceptID
	)
REFERENCES 
	consensus.Concept
	(
	ConceptID
	)	

