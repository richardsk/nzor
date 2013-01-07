ALTER TABLE 
	[provider].[ConceptRelationship]
ADD CONSTRAINT 
	[frkConceptRelationshipToConcept] 
FOREIGN KEY 
	(
	ToConceptID
	)
REFERENCES 
	provider.Concept
	(
	ConceptID
	)	