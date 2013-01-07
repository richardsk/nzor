INSERT INTO [provider].[TaxonPropertyValue]
           ([TaxonPropertyValueID]
           ,[TaxonPropertyID]
           ,[TaxonPropertyTypeID]
           ,[Value])
     VALUES
           (@taxonPropertyValueId,
		    @taxonPropertyId,
			@taxonPropertyTypeId,
			@value)