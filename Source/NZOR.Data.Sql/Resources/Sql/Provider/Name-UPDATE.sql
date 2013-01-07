UPDATE
	provider.Name
SET
	TaxonRankID = @TaxonRankID, 
	NameClassID = @NameClassID, 
	DataSourceID = @DataSourceID,
	ConsensusNameID = @consensusNameId,
	LinkStatus = @linkStatus,
	MatchScore = @matchScore,
	MatchPath = @matchPath,
	ProviderRecordId = @providerRecordId,
	ProviderCreatedDate = @providerCreatedDate,
	ProviderModifiedDate = @ProviderModifiedDate, 
	
	FullName = @FullName, 
	GoverningCode = @GoverningCode,
	IsRecombination = @IsRecombination,

	ModifiedDate = @ModifiedDate
WHERE
	NameID = @NameID
