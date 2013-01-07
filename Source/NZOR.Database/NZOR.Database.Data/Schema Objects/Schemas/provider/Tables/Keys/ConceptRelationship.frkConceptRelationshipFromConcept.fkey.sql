ALTER TABLE 
	[provider].[ConceptRelationship]
ADD CONSTRAINT 
	[frkConceptRelationshipFromConcept] 
FOREIGN KEY 
	(
	FromConceptID
	)
REFERENCES 
	provider.Concept
	(
	ConceptID
	)	

