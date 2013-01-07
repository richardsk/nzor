ALTER TABLE 
	[consensus].[ReferenceProperty]
ADD CONSTRAINT 
	[frkReferencePropertyReference] 
FOREIGN KEY 
	(
	ReferenceID
	)
REFERENCES 
	consensus.Reference
	(
	ReferenceID
	)	

