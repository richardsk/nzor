
	ALTER TABLE consensus.ConceptRelationship ADD CONSTRAINT
	DF_ConceptRelationship_ConceptRelationshipID DEFAULT newid() FOR ConceptRelationshipID

