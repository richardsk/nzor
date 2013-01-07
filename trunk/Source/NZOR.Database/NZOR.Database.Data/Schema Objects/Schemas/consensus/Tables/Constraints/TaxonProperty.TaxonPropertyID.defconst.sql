
	ALTER TABLE consensus.TaxonProperty ADD CONSTRAINT
	DF_TaxonProperty_TaxonPropertyID DEFAULT newid() FOR TaxonPropertyID

