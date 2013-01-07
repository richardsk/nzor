UPDATE consensus.[TaxonProperty]
   SET [TaxonPropertyClassID] = @TaxonPropertyClassID
      ,[ReferenceID] = @ReferenceID
      ,[ConceptID] = @ConceptID
      ,[NameID] = @NameID
      ,[InUse] = @InUse
	  ,[GeoRegionID] = @GeoRegionId
      ,[AddedDate] = @AddedDate
      ,[ModifiedDate] = @ModifiedDate
 WHERE TaxonPropertyID = @TaxonPropertyID
