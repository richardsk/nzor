ALTER TABLE consensus.TaxonPropertyValue
	ADD CONSTRAINT [frkTaxonPropertyValueTaxonPropertyType] 
	FOREIGN KEY (TaxonPropertyTypeID)
	REFERENCES TaxonPropertyType (TaxonPropertyTypeID)	

