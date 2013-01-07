ALTER TABLE provider.Annotation
	ADD CONSTRAINT [frkAnnotationName] 
	FOREIGN KEY (NameID)
	REFERENCES provider.Name (NameID)	

