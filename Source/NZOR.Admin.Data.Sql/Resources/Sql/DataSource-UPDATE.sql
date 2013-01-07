update [admin].DataSource
set ProviderId = @providerId,
	Name = @name,
	Code = @code,
	Description = @description,
	AddedDate = @addedDate,
	AddedBy = @addedBy,
	ModifiedDate = @modifiedDate,
	ModifiedBy = @modifiedBy
where DataSourceId = @dataSourceId