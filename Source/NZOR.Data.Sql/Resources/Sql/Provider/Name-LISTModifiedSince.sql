﻿SELECT n.[NameID]
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
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
left join provider.TaxonProperty tp on tp.NameID = n.NameID
WHERE isnull(n.ModifiedDate, n.AddedDate) > @fromDate
	or isnull(tp.ModifiedDate, tp.AddedDate) > @fromDate 
order by tr.SortOrder