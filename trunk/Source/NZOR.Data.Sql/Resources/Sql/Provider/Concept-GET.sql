select  pc.[ConceptID]
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
from provider.Concept pc
inner join [admin].DataSource d on d.DataSourceID = pc.DataSourceID
inner join [admin].Provider p on p.ProviderID = d.ProviderID
where pc.ConceptId = @conceptId