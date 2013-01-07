INSERT INTO
	consensus.Name
	(
	NameID, 
	
	TaxonRankID, 
	NameClassID, 
		
	FullName, 
	GoverningCode, 
	IsRecombination,

	AddedDate,
	ModifiedDate
	)
VALUES
	(
	@NameID, 
	
	@TaxonRankID, 
	@NameClassID, 
	
	@FullName, 
	@GoverningCode, 
	@IsRecombination,

	@AddedDate,	
	@ModifiedDate
	);	

