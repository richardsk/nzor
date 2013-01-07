CREATE TABLE [dbo].[Deprecated]
(
	DeprecatedId uniqueidentifier not null,
	[Table] nvarchar(250) not null,
	OldId uniqueidentifier not null,
	[NewId] uniqueidentifier null,
	DeprecationDate datetime
)
