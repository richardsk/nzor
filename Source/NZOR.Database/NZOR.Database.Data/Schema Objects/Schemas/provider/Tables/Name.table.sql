CREATE TABLE 
	[provider].[Name]
	(
	NameID UNIQUEIDENTIFIER NOT NULL, 

	TaxonRankID UNIQUEIDENTIFIER NOT NULL,
	NameClassID UNIQUEIDENTIFIER NOT NULL, 
	DataSourceID UNIQUEIDENTIFIER NOT NULL,

	ConsensusNameID UNIQUEIDENTIFIER NULL,
	IntegrationBatchID UNIQUEIDENTIFIER NULL,  
	LinkStatus NVARCHAR(15) NULL,
	MatchScore INT NULL, 
	MatchPath NVARCHAR(MAX), 

	ProviderRecordID NVARCHAR(1000) NOT NULL, 
	ProviderCreatedDate DATETIME NULL, 
	ProviderModifiedDate DATETIME NULL, 

	FullName NVARCHAR(500) NOT NULL,
	GoverningCode NVARCHAR(5) NULL, 
	IsRecombination bit null,

	AddedDate DATETIME NULL,
	ModifiedDate DATETIME NULL
	)
