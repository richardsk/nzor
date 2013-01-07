SELECT pc.[ConceptID]
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
inner join provider.Name pn on pn.NameID = pc.NameID 
left join provider.Reference pr on pr.ReferenceID = pc.AccordingToReferenceID
inner join [admin].DataSource d on d.DataSourceID = pc.DataSourceID
inner join [admin].Provider p on p.ProviderID = d.ProviderID
where pn.ConsensusNameID = @consensusNameId 
