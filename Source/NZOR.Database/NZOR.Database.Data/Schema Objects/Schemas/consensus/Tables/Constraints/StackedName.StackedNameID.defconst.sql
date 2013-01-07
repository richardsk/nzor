
	ALTER TABLE consensus.StackedName ADD CONSTRAINT
	DF_StackedName_StackedNameID DEFAULT newid() FOR StackedNameID

