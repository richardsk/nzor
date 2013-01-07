ALTER TABLE 
	[provider].[NameProperty]
ADD CONSTRAINT 
	[frkNamePropertyNamePropertyType] 
FOREIGN KEY 
	(
	NamePropertyTypeID	
	)
REFERENCES 
	NamePropertyType
	(
	NamePropertyTypeID
	)	

