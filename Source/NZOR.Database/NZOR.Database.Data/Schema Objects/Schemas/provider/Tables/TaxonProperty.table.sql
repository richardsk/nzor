CREATE TABLE provider.[TaxonProperty]
(
	[TaxonPropertyID] [uniqueidentifier] NOT NULL,
	[TaxonPropertyClassID] [uniqueidentifier] NOT NULL,
	[ConsensusTaxonPropertyID] [uniqueidentifier] NULL,
	[LinkStatus] [varchar](15) NULL,
	[MatchScore] [int] NULL,
	[ProviderRecordID] [nvarchar](1000) NOT NULL,
	[ProviderReferenceID] [nvarchar](1000) NULL,
	[ReferenceID] [uniqueidentifier] NULL,
	[ConceptID] [uniqueidentifier] NULL,
	[ProviderNameID] [nvarchar](1000) NULL,
	[ProviderConceptID] [nvarchar](1000) NULL,
	[NameID] [uniqueidentifier] NULL,
	[DataSourceID] [uniqueidentifier] NOT NULL,
	[InUse] [bit] NULL,
	[ProviderCreatedDate] [datetime] NULL,
	[ProviderModifiedDate] [datetime] NULL,
	[AddedDate] [datetime] NULL,
	[ModifiedDate] [datetime] null
)
