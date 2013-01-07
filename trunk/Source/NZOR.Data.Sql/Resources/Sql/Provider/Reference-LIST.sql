SELECT [ReferenceID]
      ,[ReferenceTypeID]
      ,[DataSourceID]
      ,[ConsensusReferenceID]
	  ,IntegrationBatchId
      ,[LinkStatus]
      ,[MatchScore]
	  ,MatchPath
      ,[ProviderRecordID]
      ,[ProviderCreatedDate]
      ,[ProviderModifiedDate]
      ,[AddedDate]
      ,[ModifiedDate]
FROM 
	provider.Reference
WHERE 
	DataSourceID = @DataSourceID 
ORDER BY 
	ProviderRecordID