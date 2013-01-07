
	ALTER TABLE consensus.ConceptApplication ADD CONSTRAINT
	DF_ConceptApplication_ConceptApplicationID DEFAULT newid() FOR ConceptApplicationID

