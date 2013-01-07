SELECT  pc.[ConceptID]
      ,[NameID]
      ,[AccordingToReferenceID]
      ,pc.[DataSourceID]
      ,[ConsensusConceptID]
	  ,IntegrationBatchId
      ,[LinkStatus]
      ,[MatchScore]
	  ,MatchPath
      ,[ConceptSourceType]
      ,[ProviderNameID]
      ,[ProviderReferenceID]
      ,[ProviderRecordID]
      ,[ProviderCreatedDate]
      ,[ProviderModifiedDate]
      ,[Orthography]
      ,[TaxonRank]
      ,[HigherClassification]
      ,pc.[AddedDate]
      ,pc.[ModifiedDate]
	  ,p.Name as ProviderName
	  ,p.ProviderId
FROM 
	provider.Concept pc
	inner join [admin].DataSource d on d.DataSourceID = pc.DataSourceID
	inner join [admin].Provider p on p.ProviderID = d.ProviderID
WHERE 
	pc.DataSourceID = @DataSourceID 
ORDER BY 
	ProviderRecordID