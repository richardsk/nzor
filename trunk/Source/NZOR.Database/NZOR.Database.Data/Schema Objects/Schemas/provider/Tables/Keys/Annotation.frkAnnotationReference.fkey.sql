ALTER TABLE provider.Annotation
	ADD CONSTRAINT [frkAnnotationReference] 
	FOREIGN KEY (ReferenceID)
	REFERENCES provider.Reference (ReferenceID)	

