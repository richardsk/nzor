ALTER TABLE 
	[consensus].[Reference]
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
