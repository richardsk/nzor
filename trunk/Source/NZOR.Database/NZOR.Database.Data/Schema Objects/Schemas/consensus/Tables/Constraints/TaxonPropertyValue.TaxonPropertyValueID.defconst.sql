
	ALTER TABLE consensus.TaxonPropertyValue ADD CONSTRAINT
	DF_TaxonPropertyValue_TaxonPropertyValueID DEFAULT newid() FOR TaxonPropertyValueID

