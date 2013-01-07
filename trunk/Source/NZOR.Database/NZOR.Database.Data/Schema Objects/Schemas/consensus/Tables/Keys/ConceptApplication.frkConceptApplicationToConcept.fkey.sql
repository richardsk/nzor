ALTER TABLE 
	[consensus].[ConceptApplication]
ADD CONSTRAINT 
	[frkConceptApplicationToConcept] 
FOREIGN KEY 
	(
	FromConceptID
	)
REFERENCES 
	consensus.Concept
	(
	ConceptID
	)	

