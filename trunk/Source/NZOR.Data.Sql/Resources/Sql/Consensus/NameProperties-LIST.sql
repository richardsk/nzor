SELECT NamePropertyID
      ,NameID
      ,np.NamePropertyTypeID 
	  ,npt.Name as NamePropertyType
      ,RelatedID
      ,Sequence
      ,Value
FROM consensus.NameProperty np
inner join dbo.NamePropertyType npt on npt.namepropertytypeid = np.namepropertytypeid
WHERE NameId = @nameId