SELECT NamePropertyID
      ,np.NameID
      ,np.NamePropertyTypeID 
	  ,npt.Name as NamePropertyType
      ,RelatedID
	  ,ProviderRelatedID
      ,Sequence
      ,Value
FROM provider.NameProperty np
inner join provider.Name n on n.NameID = np.NameID
inner join dbo.NamePropertyType npt on npt.namepropertytypeid = np.namepropertytypeid
WHERE n.ConsensusNameId = @consensusNameId
