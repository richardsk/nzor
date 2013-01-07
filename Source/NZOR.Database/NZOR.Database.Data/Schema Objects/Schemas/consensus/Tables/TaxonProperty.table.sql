CREATE TABLE consensus.[TaxonProperty]
(
	[TaxonPropertyID] [uniqueidentifier] NOT NULL,
	[TaxonPropertyClassID] [uniqueidentifier] NOT NULL,
	[GeoRegionID] [uniqueidentifier] NULL,
	[ReferenceID] [uniqueidentifier] NULL,
	[ConceptID] [uniqueidentifier] NULL,
	[NameID] [uniqueidentifier] NULL,
	[InUse] [bit] NULL,
	[AddedDate] [datetime] NULL,
	[ModifiedDate] [datetime] null
)
