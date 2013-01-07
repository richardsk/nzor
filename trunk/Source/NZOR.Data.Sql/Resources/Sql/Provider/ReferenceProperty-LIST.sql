SELECT [ReferencePropertyID]
      ,[ReferenceID]
      ,rp.[ReferencePropertyTypeID]
	  ,rpt.Name as ReferencePropertyType
      ,[SubType]
      ,[Sequence]
      ,[Level]
      ,[Value]
FROM provider.ReferenceProperty rp
inner join dbo.ReferencePropertyType rpt on rpt.ReferencePropertyTypeId = rp.ReferencePropertyTypeId
where ReferenceID = @referenceID
