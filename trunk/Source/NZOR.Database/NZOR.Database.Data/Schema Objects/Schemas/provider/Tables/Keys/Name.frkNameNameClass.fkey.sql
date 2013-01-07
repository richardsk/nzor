ALTER TABLE 
	[provider].[Name]
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

