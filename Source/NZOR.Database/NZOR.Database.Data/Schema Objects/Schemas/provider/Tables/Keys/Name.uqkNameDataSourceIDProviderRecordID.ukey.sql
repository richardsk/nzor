ALTER TABLE 
	[provider].[Name]
ADD CONSTRAINT 
	uqkNameDataSourceIDProviderRecordID
UNIQUE 
	(
	DataSourceID,
	ProviderRecordID
	)