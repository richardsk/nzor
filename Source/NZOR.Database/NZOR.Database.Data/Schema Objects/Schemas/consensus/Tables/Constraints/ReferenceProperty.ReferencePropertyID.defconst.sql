
	ALTER TABLE consensus.ReferenceProperty ADD CONSTRAINT
	DF_ReferenceProperty_ReferencePropertyID DEFAULT newid() FOR ReferencePropertyID

