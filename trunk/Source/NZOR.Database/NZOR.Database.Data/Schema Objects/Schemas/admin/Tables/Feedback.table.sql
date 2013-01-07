CREATE TABLE [admin].[Feedback]
(
	FeedbackId uniqueidentifier not null,
	NameId uniqueidentifier null,
	[Message] nvarchar(max) not null,
	[Status] nvarchar(250) not null,
	[Resolution] nvarchar(max) null,
	Sender nvarchar(500) not null,
	SenderEmail nvarchar(500) not null,
	SentTo nvarchar(max) null,
	AddedDate datetime not null,
	ModifiedDate datetime null
)
