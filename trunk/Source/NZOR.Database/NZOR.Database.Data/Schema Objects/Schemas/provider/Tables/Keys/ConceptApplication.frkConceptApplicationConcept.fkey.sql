ALTER TABLE 
	[provider].[ConceptApplication]
ADD CONSTRAINT 
	[frkConceptApplicationConcept] 
FOREIGN KEY 
	(
	FromConceptID
	)
REFERENCES 
	provider.Concept
	(
	ConceptID
	)	

