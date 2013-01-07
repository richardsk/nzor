use NZOR_Data_Test
 
  --cons
delete consensus.ConceptRelationship
delete consensus.Concept
delete cons.FlatName
delete consensus.NameProperty
--delete consensus.TaxonPropertyValue
--delete consensus.TaxonProperty
delete consensus.ReferenceField
delete consensus.Reference
delete consensus.Name
  
  
  --prov
delete provider.ConceptRelationship
delete provider.Concept
delete provider.NameProperty
delete provider.ReferenceProperty
delete provider.Reference
delete prov.TaxonPropertyValue
delete prov.TaxonProperty
delete provider.Name


 --system
delete dbo.ConceptRelationshipType
delete dbo.GeoRegion
delete dbo.GeoRegionSchema
delete dbo.NamePropertyType
delete dbo.NameClass
delete dbo.NamePropertyLookup
delete dbo.SubDataSet
delete dbo.Provider
delete dbo.ReferenceFieldMap
delete dbo.ReferencePropertyType
delete dbo.ReferenceType
delete dbo.TaxonPropertyLookup
delete dbo.TaxonPropertyType
delete dbo.TaxonPropertyClass
delete dbo.TaxonRank


  

