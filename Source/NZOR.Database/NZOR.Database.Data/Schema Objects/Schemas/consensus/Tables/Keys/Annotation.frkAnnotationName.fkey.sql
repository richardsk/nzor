ALTER TABLE consensus.Annotation
	ADD CONSTRAINT [frkAnnotationName] 
	FOREIGN KEY (NameID)
	REFERENCES consensus.Name (NameID)	

