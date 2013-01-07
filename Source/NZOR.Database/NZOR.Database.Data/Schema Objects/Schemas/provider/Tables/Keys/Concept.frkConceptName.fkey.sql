ALTER TABLE 
	[provider].[Concept]
ADD CONSTRAINT 
	[frkConceptName] 
FOREIGN KEY 
	(
	NameID
	)
REFERENCES 
	provider.Name
	(
	NameID
	)	

