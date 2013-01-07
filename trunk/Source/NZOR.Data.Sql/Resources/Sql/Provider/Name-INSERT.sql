INSERT INTO
	provider.Name
	(
	NameID, 
	
	TaxonRankID, 
	NameClassID, 
	DataSourceID, 
	
	ConsensusNameID, 
	IntegrationBatchID,
	LinkStatus, 
	MatchScore, 
	MatchPath, 

	ProviderRecordID, 
	ProviderCreatedDate, 
	ProviderModifiedDate, 
	
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
	@DataSourceID, 
	
	NULL, 
	NULL, 
	@LinkStatus,
	NULL, 
	NULL, 

	@ProviderRecordID, 
	@ProviderCreatedDate, 
	@ProviderModifiedDate, 
	
	@FullName, 
	@GoverningCode, 
	@IsRecombination, 

	@AddedDate,	
	@ModifiedDate
	);	