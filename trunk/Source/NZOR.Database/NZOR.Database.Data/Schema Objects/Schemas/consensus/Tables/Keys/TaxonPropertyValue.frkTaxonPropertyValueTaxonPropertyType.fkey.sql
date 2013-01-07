ALTER TABLE provider.TaxonPropertyValue
	ADD CONSTRAINT [frkTaxonPropertyValueTaxonPropertyType] 
	FOREIGN KEY (TaxonPropertyTypeID)
	REFERENCES TaxonPropertyType (TaxonPropertyTypeID)	

