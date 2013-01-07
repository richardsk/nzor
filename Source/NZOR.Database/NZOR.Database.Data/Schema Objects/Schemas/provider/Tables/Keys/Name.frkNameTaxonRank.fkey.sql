ALTER TABLE 
	[provider].[Name]
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

