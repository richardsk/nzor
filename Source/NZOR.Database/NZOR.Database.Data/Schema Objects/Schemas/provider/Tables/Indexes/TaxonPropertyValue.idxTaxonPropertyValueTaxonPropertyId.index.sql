
CREATE NONCLUSTERED INDEX [taxonPropertyValue_TaxonPropertyId]
ON [provider].[TaxonPropertyValue] ([TaxonPropertyID])
INCLUDE ([TaxonPropertyTypeID],[Value])
GO

