SELECT bn.[BrokeredNameId]
      ,bn.[NameRequestId]
      ,bn.[ExternalLookupServiceId]
      ,bn.[ProviderRecordId]
      ,bn.[NZORProviderNameId]
      ,bn.[DataUrl]
      ,bn.[WebUrl]
      ,bn.[AddedDate]
      ,bn.[AddedBy]
      ,bn.[ModifiedDate]
      ,bn.[ModifiedBy]
FROM [admin].[BrokeredName] bn
left join provider.name pn on pn.nameid = bn.nzorprovidernameid
left join dbo.taxonrank tr on tr.taxonrankid = pn.taxonrankid
where pn.ConsensusNameID is null
order by tr.sortorder

