ALTER TABLE 
	[provider].[Reference]
ADD CONSTRAINT 
	[frkReferenceReferenceType] 
FOREIGN KEY 
	(
	ReferenceTypeID
	)
REFERENCES 
	ReferenceType
	(
	ReferenceTypeID
	)	

