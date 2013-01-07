UPDATE
	provider.Reference
SET
	ReferenceTypeID = @ReferenceTypeID, 
	DataSourceID = @DataSourceID,
	ConsensusReferenceID = @consensusReferenceId,
	IntegrationBatchId = @integrationBatchId,
	LinkStatus = @linkStatus,
	MatchScore = @matchScore,
	MatchPath = @matchPath,
	ProviderModifiedDate = @ProviderModifiedDate, 
	
	ModifiedDate = @ModifiedDate
WHERE
	ReferenceID = @ReferenceID;
