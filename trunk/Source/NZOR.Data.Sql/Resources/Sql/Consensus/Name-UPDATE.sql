UPDATE
	consensus.Name
SET
	TaxonRankID = @TaxonRankID, 
	NameClassID = @NameClassID, 
	
	FullName = @FullName, 
	GoverningCode = @GoverningCode,
	IsRecombination = @IsRecombination,

	ModifiedDate = @ModifiedDate
WHERE
	NameID = @NameID

