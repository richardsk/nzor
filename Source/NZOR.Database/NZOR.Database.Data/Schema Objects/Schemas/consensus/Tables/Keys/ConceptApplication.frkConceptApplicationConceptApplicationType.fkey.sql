ALTER TABLE 
	[consensus].[ConceptApplication]
ADD CONSTRAINT 
	[frkConceptApplicationConceptApplicationType] 
FOREIGN KEY 
	(
	ConceptApplicationTypeID
	)
REFERENCES 
	ConceptApplicationType
	(
	ConceptApplicationTypeID
	)	

