SELECT n.[NameID]
      ,n.[TaxonRankID]
      ,n.[NameClassID]
      ,n.[DataSourceID]
	  ,ds.Code as DataSourceName
      ,n.[ConsensusNameID]
      ,n.[IntegrationBatchID]
      ,n.[LinkStatus]
      ,n.[MatchScore]
      ,n.[MatchPath]
      ,n.[ProviderRecordID]
      ,n.[ProviderCreatedDate]
      ,n.[ProviderModifiedDate]
	  ,p.Name as ProviderName
      ,n.[FullName]
      ,n.[GoverningCode]
	  ,n.IsRecombination
      ,n.[AddedDate]
      ,n.[ModifiedDate]
	  ,p.ProviderId
FROM provider.Name n
inner join [admin].DataSource ds on ds.DataSourceID = n.DataSourceID
inner join [admin].Provider p on p.ProviderID = ds.ProviderID
WHERE ProviderRecordId = @providerRecordId and ds.Code = @dataSourceCode

