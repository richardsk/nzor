CREATE TABLE 
	[provider].[Reference]
	(
	ReferenceID UNIQUEIDENTIFIER NOT NULL, 
	
	ReferenceTypeID UNIQUEIDENTIFIER NOT NULL, 
	DataSourceID UNIQUEIDENTIFIER NOT NULL,
	
	ConsensusReferenceID UNIQUEIDENTIFIER NULL,
	IntegrationBatchID UNIQUEIDENTIFIER NULL,  
	LinkStatus NVARCHAR(15) NULL, 
	MatchScore INT NULL, 
	MatchPath nvarchar(max),
	
	ProviderRecordID NVARCHAR(1000) NOT NULL, 
	ProviderCreatedDate DATETIME NULL, 
	ProviderModifiedDate DATETIME NULL, 
	
	AddedDate DATETIME NULL,
	ModifiedDate DATETIME NULL
	)
