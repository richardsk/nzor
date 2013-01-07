ALTER TABLE 
	[consensus].[Concept]
ADD CONSTRAINT 
	[frkConceptReference] 
FOREIGN KEY 
	(
	AccordingToReferenceID
	)
REFERENCES 
	consensus.Reference
	(
	ReferenceID
	)	

