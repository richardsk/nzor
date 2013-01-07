ALTER TABLE 
	[consensus].[Name]
ADD CONSTRAINT 
	[frkNameNameClass] 
FOREIGN KEY 
	(
	NameClassID
	)
REFERENCES 
	NameClass
	(
	NameClassID
	)	

