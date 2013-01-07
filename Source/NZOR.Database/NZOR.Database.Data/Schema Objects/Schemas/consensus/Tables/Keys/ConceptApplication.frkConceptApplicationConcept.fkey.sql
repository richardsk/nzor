ALTER TABLE 
	[consensus].[ConceptApplication]
ADD CONSTRAINT 
	[frkConceptApplicationConcept] 
FOREIGN KEY 
	(
	ToConceptID
	)
REFERENCES 
	consensus.Concept
	(
	ConceptID
	)	

