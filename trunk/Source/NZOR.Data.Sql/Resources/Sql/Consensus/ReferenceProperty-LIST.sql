SELECT rp.[ReferencePropertyID]
      ,rp.[ReferenceID]
      ,rp.[ReferencePropertyTypeID]
      ,rp.[Value]
	  ,rpt.Name as ReferencePropertyType
FROM consensus.ReferenceProperty rp
inner join dbo.ReferencePropertyType rpt on rpt.ReferencePropertyTypeId = rp.ReferencePropertyTypeId 
where ReferenceID = @referenceID
