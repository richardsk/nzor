ALTER TABLE 
	[consensus].[Concept]
ADD CONSTRAINT 
	[frkConceptName] 
FOREIGN KEY 
	(
	NameID
	)
REFERENCES 
	consensus.Name
	(
	NameID
	)	

