SELECT 
	DataSourceID,
	ProviderID,
	Name,
	Code,
	Description,
	AddedDate,
	AddedBy,
	ModifiedDate,
	ModifiedBy
FROM 
	[admin].DataSource
WHERE 
	Code = @dataSourceCode
