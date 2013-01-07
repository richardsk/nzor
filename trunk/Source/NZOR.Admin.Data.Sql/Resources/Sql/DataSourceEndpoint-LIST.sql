select de.DataSourceEndpointID,
	de.DataSourceID,
	de.DataTypeID,
	dt.Name as DataType,
	de.Name,
	de.Description,
	de.Url,
	de.LastHarvestDate
from [admin].DataSourceEndpoint de
inner join [admin].DataType dt on dt.DataTypeID = de.DataTypeID
where de.DataSourceID = @datasourceId