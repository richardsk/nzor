ALTER TABLE consensus.Annotation
	ADD CONSTRAINT [frkAnnotationReference] 
	FOREIGN KEY (ReferenceID)
	REFERENCES consensus.Reference (ReferenceID)	

