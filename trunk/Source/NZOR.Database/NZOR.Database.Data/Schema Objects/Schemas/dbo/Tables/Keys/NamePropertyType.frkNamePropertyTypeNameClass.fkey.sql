ALTER TABLE 
	[dbo].NamePropertyType
ADD CONSTRAINT 
	[frkNamePropertyTypeNameClass] 
FOREIGN KEY 
	(
	NameClassID
	)
REFERENCES 
	NameClass
	(
	NameClassID
	)	

