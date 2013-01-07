SELECT NamePropertyID
      ,NameID
      ,np.NamePropertyTypeID 
	  ,npt.Name as NamePropertyType
      ,RelatedID
	  ,ProviderRelatedID
      ,Sequence
      ,Value
FROM provider.NameProperty np
inner join dbo.NamePropertyType npt on npt.namepropertytypeid = np.namepropertytypeid
WHERE NameId = @nameId