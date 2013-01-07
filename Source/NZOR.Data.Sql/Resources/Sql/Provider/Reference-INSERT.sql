INSERT INTO
	provider.Reference
	(
	ReferenceID, 
	
	ReferenceTypeID, 
	DataSourceID, 

	ConsensusReferenceID, 
	IntegrationBatchId,
	LinkStatus, 
	MatchScore, 
	MatchPath,

	ProviderRecordID, 
	ProviderCreatedDate, 
	ProviderModifiedDate, 
	
	AddedDate, 
	ModifiedDate
	)
VALUES
	(
	@ReferenceID, 
	
	@ReferenceTypeID, 
	@DataSourceID, 
	
	NULL, 
	NULL,
	NULL, 
	NULL, 
	NULL,

	@ProviderRecordID, 
	@ProviderCreatedDate, 
	@ProviderModifiedDate, 

	@AddedDate,	
	@ModifiedDate
	);	