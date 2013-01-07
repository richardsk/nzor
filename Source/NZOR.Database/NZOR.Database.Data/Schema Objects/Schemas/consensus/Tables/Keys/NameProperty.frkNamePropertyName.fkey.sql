ALTER TABLE 
	[consensus].[NameProperty]
ADD CONSTRAINT 
	[frkNamePropertyName] 
FOREIGN KEY 
	(
	NameID
	)
REFERENCES 
	consensus.Name
	(
	NameID
	)	

