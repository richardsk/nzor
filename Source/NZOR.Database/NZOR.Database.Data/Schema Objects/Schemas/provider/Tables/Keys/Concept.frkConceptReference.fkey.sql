ALTER TABLE 
	[provider].[Concept]
ADD CONSTRAINT 
	[frkConceptReference] 
FOREIGN KEY 
	(
	AccordingToReferenceID
	)
REFERENCES 
	provider.Reference
	(
	ReferenceID
	)	

