ALTER TABLE consensus.Annotation
	ADD CONSTRAINT [frkAnnotationConcept] 
	FOREIGN KEY (ConceptID)
	REFERENCES consensus.Concept (ConceptID)	

