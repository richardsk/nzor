CREATE TABLE 
	[provider].[Concept]
	(
	ConceptID UNIQUEIDENTIFIER NOT NULL, 

	NameID UNIQUEIDENTIFIER NOT NULL, 
	AccordingToReferenceID UNIQUEIDENTIFIER NULL, 
	DataSourceID UNIQUEIDENTIFIER NOT NULL, 

	ConsensusConceptID UNIQUEIDENTIFIER NULL, 
	IntegrationBatchID UNIQUEIDENTIFIER NULL,  
	LinkStatus NVARCHAR(15) NULL,
	MatchScore INT NULL, 
	MatchPath nvarchar(max),
	ConceptSourceType NVARCHAR(50) NOT NULL DEFAULT N'',

	ProviderNameID NVARCHAR(1000) NULL, 
	ProviderReferenceID NVARCHAR(1000) NULL, 
	ProviderRecordID NVARCHAR(1000) NULL, 
	ProviderCreatedDate DATETIME NULL, 
	ProviderModifiedDate DATETIME NULL, 

	Orthography NVARCHAR(1000) NULL, 
	TaxonRank NVARCHAR(100) NULL, 
	HigherClassification NVARCHAR(MAX) NULL, 
	NameQualifier nvarchar(250) null,

	AddedDate DATETIME NULL,
	ModifiedDate DATETIME NULL
	)
