ALTER TABLE provider.Annotation
	ADD CONSTRAINT [frkAnnotationConcept] 
	FOREIGN KEY (ConceptID)
	REFERENCES provider.Concept (ConceptID)	

