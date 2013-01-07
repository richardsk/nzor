ALTER TABLE 
	[provider].[ConceptApplication]
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

