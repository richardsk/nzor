ALTER TABLE 
	[provider].[ReferenceProperty]
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

