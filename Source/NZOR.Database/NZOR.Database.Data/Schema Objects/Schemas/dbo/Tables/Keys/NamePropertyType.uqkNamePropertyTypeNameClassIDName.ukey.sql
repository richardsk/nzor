ALTER TABLE 
	[dbo].NamePropertyType
ADD CONSTRAINT 
	[uqkNamePropertyTypeNameClassIDName]
UNIQUE 
	(
	NameClassID,
	Name
	)