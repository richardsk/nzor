select UserId,
	Name,
	Email,
	[Password],
	Organisation,
	APIKey,
	[Status],
	AddedDate,
	ModifiedDate
from [admin].[User]
where UserId = @userId
