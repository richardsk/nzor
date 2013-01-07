select tp.[TaxonPropertyID]
      ,tp.[TaxonPropertyClassID]
	  ,tpc.Name as TaxonPropertyClass
      ,tp.[ConsensusTaxonPropertyID]
      ,tp.[LinkStatus]
      ,tp.[MatchScore]
      ,tp.[ProviderRecordID]
      ,tp.[ProviderReferenceID]
      ,tp.[ProviderNameID]
      ,tp.[ProviderConceptID]
      ,tp.[DataSourceID]
      ,tp.[InUse]
      ,tp.[ProviderCreatedDate]
      ,tp.[ProviderModifiedDate]      
      ,tp.[ReferenceID]
	  ,rp.Value as ReferenceCitation
      ,tp.[ConceptID]
      ,tp.[NameID]
      ,tp.[AddedDate]
      ,tp.[ModifiedDate]
from provider.TaxonProperty tp
inner join provider.Name n on n.NameId = tp.NameID
inner join dbo.TaxonPropertyClass tpc on tpc.TaxonPropertyClassID = tp.TaxonPropertyClassID
left join provider.ReferenceProperty rp on tp.ReferenceId = rp.ReferenceId and rp.ReferencePropertyTypeId = '7F835876-B459-4023-90E4-6C22646FBE07'
where n.ConsensusNameID = @consensusNameId