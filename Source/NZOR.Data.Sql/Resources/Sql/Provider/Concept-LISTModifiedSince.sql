SELECT distinct pc.[ConceptID]
      ,pc.[NameID]
      ,pc.[AccordingToReferenceID]
      ,pc.[DataSourceID]
      ,pc.[ConsensusConceptID]
	  ,pc.IntegrationBatchId
      ,pc.[LinkStatus]
      ,pc.[MatchScore]
	  ,pc.MatchPath
      ,pc.[ConceptSourceType]
      ,pc.[ProviderNameID]
      ,pc.[ProviderReferenceID]
      ,pc.[ProviderRecordID]
      ,pc.[ProviderCreatedDate]
      ,pc.[ProviderModifiedDate]
      ,pc.[Orthography]
      ,pc.[TaxonRank]
      ,pc.[HigherClassification]
      ,pc.[AddedDate]
      ,pc.[ModifiedDate]
	  ,p.Name as ProviderName
	  ,p.ProviderId
from provider.Concept pc
inner join [admin].DataSource d on d.DataSourceID = pc.DataSourceID
inner join [admin].Provider p on p.ProviderID = d.ProviderID
left join provider.ConceptApplication ca on ca.FromConceptID = pc.ConceptID
left join provider.TaxonProperty tp on tp.ConceptID = pc.ConceptID
where isnull(pc.ModifiedDate, pc.AddedDate) > @fromDate 
	or isnull(ca.ModifiedDate, ca.AddedDate) > @fromDate 
	or isnull(tp.ModifiedDate, tp.AddedDate) > @fromDate 
