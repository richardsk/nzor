ALTER TABLE consensus.[TaxonProperty]
	ADD CONSTRAINT [frkTaxonPropertyTaxonPropertyClass] 
	FOREIGN KEY (TaxonPropertyClassID)
	REFERENCES dbo.TaxonPropertyClass (TaxonPropertyClassID)	

