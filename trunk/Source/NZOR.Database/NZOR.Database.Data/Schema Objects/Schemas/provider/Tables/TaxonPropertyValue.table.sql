CREATE TABLE [provider].[TaxonPropertyValue]
(
	[TaxonPropertyValueID] [uniqueidentifier] NOT NULL,
	[TaxonPropertyID] [uniqueidentifier] NOT NULL,
	[TaxonPropertyTypeID] [uniqueidentifier] NOT NULL,
	[Value] [nvarchar](500) NULL 
)
