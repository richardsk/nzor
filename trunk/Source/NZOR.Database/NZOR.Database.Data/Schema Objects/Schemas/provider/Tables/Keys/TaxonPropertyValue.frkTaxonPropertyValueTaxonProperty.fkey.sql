ALTER TABLE consensus.TaxonPropertyValue
	ADD CONSTRAINT [frkTaxonPropertyValueTaxonProperty] 
	FOREIGN KEY (TaxonPropertyID)
	REFERENCES consensus.TaxonProperty (TaxonPropertyID)	

