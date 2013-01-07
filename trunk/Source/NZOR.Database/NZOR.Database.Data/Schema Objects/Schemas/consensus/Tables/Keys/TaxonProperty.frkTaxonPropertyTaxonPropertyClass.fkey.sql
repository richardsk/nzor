ALTER TABLE provider.[TaxonProperty]
	ADD CONSTRAINT [frkTaxonPropertyTaxonPropertyClass] 
	FOREIGN KEY (TaxonPropertyClassID)
	REFERENCES dbo.TaxonPropertyClass (TaxonPropertyClassID)	

