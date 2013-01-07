select [TaxonPropertyID]
      ,tp.[TaxonPropertyClassID]
	  ,tpc.Name as TaxonPropertyClass
      ,tp.[GeoRegionID]
	  ,gr.Name as GeoRegion
      ,tp.[ReferenceID]
	  ,rp.Value as ReferenceCitation
      ,[ConceptID]
      ,[NameID]
      ,[InUse]
      ,[AddedDate]
      ,[ModifiedDate]
from consensus.TaxonProperty tp
inner join dbo.TaxonPropertyClass tpc on tpc.TaxonPropertyClassID = tp.TaxonPropertyClassID
left join dbo.GeoRegion gr on gr.GeoRegionId = tp.GeoRegionId
left join consensus.ReferenceProperty rp on tp.ReferenceId = rp.ReferenceId and rp.ReferencePropertyTypeId = '7F835876-B459-4023-90E4-6C22646FBE07'
where NameID = @nameId