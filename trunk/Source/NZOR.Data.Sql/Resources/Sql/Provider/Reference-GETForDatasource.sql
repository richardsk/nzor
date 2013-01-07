select pr.[ReferenceID]
      ,[ReferenceTypeID]
      ,pr.[DataSourceID]
      ,[ConsensusReferenceID]
	  ,IntegrationBatchId
      ,[LinkStatus]
      ,[MatchScore]
	  ,MatchPath
      ,[ProviderRecordID]
      ,[ProviderCreatedDate]
      ,[ProviderModifiedDate]
      ,pr.[AddedDate]
      ,pr.[ModifiedDate]
from provider.Reference pr
inner join provider.ReferenceProperty crp on crp.ReferenceID = pr.ReferenceID and crp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07'
inner join [admin].DataSource d on d.DataSourceID = pr.DataSourceID
where crp.Value = d.Code + ' ' + datename(month, @date) + ' ' + datename(year, @date)
	and ProviderCreatedDate = @date and d.datasourceid = @dataSourceId
