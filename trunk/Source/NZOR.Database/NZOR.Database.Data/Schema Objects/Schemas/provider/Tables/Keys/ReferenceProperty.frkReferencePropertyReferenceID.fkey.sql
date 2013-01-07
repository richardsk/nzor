ALTER TABLE 
	[provider].[ReferenceProperty]
ADD CONSTRAINT 
	[frkReferencePropertyReferenceID] 
FOREIGN KEY 
	(
	ReferenceID
	)
REFERENCES
	provider.Reference
	(
	ReferenceID
	)	

