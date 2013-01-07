CREATE TABLE [admin].[User]
(
	UserId uniqueidentifier not null primary key, 
	Name nvarchar(250) not null,
	Email nvarchar(1000) not null,
	[Password] varbinary(500),
	Organisation nvarchar(250),
	APIKey nvarchar(250),
	[Status] nvarchar(100),
	AddedDate datetime not null,
	ModifiedDate datetime not null
)
