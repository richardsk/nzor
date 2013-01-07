CREATE TABLE [admin].[Message]
(
	MessageId uniqueidentifier not null primary key,
	MessageTypeId uniqueidentifier not null,
	TaskId uniqueidentifier null,
	AddedDate datetime not null,
	[Message] nvarchar(max) null,
	Data xml null,
	[Level] int null
)
