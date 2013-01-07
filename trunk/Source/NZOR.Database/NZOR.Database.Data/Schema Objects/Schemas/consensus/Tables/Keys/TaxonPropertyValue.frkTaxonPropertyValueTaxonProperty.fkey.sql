ALTER TABLE provider.TaxonPropertyValue
	ADD CONSTRAINT [frkTaxonPropertyValueTaxonProperty] 
	FOREIGN KEY (TaxonPropertyID)
	REFERENCES provider.TaxonProperty (TaxonPropertyID)	

