ALTER TABLE 
	[dbo].[TaxonPropertyType]
ADD CONSTRAINT 
	[frkTaxonPropertyTypeTaxonPropertyClass] 
FOREIGN KEY 
	(
	TaxonPropertyClassID
	)
REFERENCES 
	TaxonPropertyClass
	(
	TaxonPropertyClassID
	)	
