ALTER TABLE 
	[provider].[NameProperty]
ADD CONSTRAINT 
	frkNamePropertyName 
FOREIGN KEY 
	(
	NameID
	)
REFERENCES 
	provider.Name
	(
	NameID
	)	

