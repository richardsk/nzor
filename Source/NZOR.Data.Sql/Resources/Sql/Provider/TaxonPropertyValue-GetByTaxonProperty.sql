﻿SELECT [TaxonPropertyValueID]
      ,[TaxonPropertyID]
      ,tpv.[TaxonPropertyTypeID]
	  ,tpt.Name as TaxonPropertyType
      ,[Value]
FROM provider.[TaxonPropertyValue] tpv
inner join dbo.TaxonPropertyType tpt on tpt.TaxonPropertyTypeID = tpv.TaxonPropertyTypeID
where TaxonPropertyID = @taxonPropertyId