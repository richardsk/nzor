ALTER TABLE 
	[consensus].[NameProperty]
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

