INSERT INTO consensus.[TaxonPropertyValue]
           ([TaxonPropertyValueID]
           ,[TaxonPropertyID]
           ,[TaxonPropertyTypeID]
           ,[Value])
     VALUES
           (@taxonPropertyValueId,
		    @taxonPropertyId,
			@taxonPropertyTypeId,
			@value)