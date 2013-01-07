update [admin].[User]
set Email = @email,
	Name = @name,
	[Password] = convert(varbinary(500), @password),
	Organisation = @organisation,
	ApiKey = @apiKey,
	[Status] = @status,
	AddedDate = @addedDate,
	ModifiedDate = getdate()
where UserId = @userId
