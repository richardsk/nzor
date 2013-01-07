ALTER TABLE 
	[provider].[ConceptApplication]
ADD CONSTRAINT 
	[frkConceptApplicationToConcept] 
FOREIGN KEY 
	(
	ToConceptID
	)
REFERENCES 
	provider.Concept
	(
	ConceptID
	)	

