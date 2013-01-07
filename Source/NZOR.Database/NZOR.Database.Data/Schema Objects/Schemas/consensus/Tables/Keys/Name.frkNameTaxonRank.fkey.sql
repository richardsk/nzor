ALTER TABLE 
	[consensus].[Name]
ADD CONSTRAINT 
	[frkNameTaxonRank] 
FOREIGN KEY 
	(
	TaxonRankID
	)
REFERENCES 
	TaxonRank
	(
	TaxonRankID
	)	

