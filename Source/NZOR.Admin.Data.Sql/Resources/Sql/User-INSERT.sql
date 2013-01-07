insert [admin].[User]
select @userId,
	@name,
	@email,
	convert(varbinary(500), @password),
	@organisation,
	@apiKey,
	@status,
	getdate(),
	getdate()