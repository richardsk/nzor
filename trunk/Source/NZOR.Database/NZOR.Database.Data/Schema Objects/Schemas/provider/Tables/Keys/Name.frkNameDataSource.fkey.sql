ALTER TABLE 
	[provider].[Name]
ADD CONSTRAINT 
	frkNameDataSource 
FOREIGN KEY 
	(
	DataSourceID
	)
REFERENCES 
	[admin].DataSource
	(
	DataSourceID
	)	

