select [ReferenceID]
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
from provider.Reference 
where referenceId = @referenceId
