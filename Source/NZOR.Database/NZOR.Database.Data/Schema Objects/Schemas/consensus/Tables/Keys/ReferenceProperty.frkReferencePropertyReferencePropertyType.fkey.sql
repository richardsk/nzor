ALTER TABLE 
	[consensus].[ReferenceProperty]
ADD CONSTRAINT 
	[frkReferencePropertyReferencePropertyType] 
FOREIGN KEY 
	(
	ReferencePropertyTypeID
	)
REFERENCES 
	ReferencePropertyType
	(
	ReferencePropertyTypeID
	)	

