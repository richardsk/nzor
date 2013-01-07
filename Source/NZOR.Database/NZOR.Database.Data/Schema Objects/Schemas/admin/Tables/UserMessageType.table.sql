CREATE TABLE [admin].[UserMessageType]
(
	UserMessageTypeId uniqueidentifier not null primary key,
	MessageTypeId uniqueidentifier not null,
	UserId uniqueidentifier not null
)
