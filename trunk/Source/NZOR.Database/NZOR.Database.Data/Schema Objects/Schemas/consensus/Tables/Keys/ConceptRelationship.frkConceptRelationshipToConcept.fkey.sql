ALTER TABLE 
	[consensus].[ConceptRelationship]
ADD CONSTRAINT 
	[frkConceptRelationshipToConcept] 
FOREIGN KEY 
	(
	ToConceptID
	)
REFERENCES 
	consensus.Concept
	(
	ConceptID
	)	

