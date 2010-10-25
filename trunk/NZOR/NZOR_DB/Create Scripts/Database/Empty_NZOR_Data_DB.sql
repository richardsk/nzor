use NZOR_Data_Test
 
  --cons
delete cons.ConceptRelationship
delete cons.Concept
delete cons.FlatName
delete cons.NameProperty
delete cons.TaxonPropertyValue
delete cons.TaxonProperty
delete cons.ReferenceField
delete cons.Reference
delete cons.Name
  
  
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
delete dbo.NameClassProperty
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


  

