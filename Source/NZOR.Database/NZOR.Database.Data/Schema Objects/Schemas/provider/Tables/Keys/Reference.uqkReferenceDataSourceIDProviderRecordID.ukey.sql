ALTER TABLE 
	[provider].[Reference]
ADD CONSTRAINT 
	uqkReferenceDataSourceIDProviderRecordID
UNIQUE 
	(
	DataSourceID,
	ProviderRecordID
	)