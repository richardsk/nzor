update [admin].DataSourceEndpoint
set DataSourceId = @dataSourceId,
	DataTypeId = @dataTypeId,
	Name = @name,
	Description = @description,
	Url = @url,
	LastHarvestDate = @lastHarvestDate
where DataSourceEndpointId = @dataSourceEndpointId