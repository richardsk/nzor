CREATE TABLE [admin].[Setting]
(
	SettingId uniqueidentifier NOT NULL, 
	Name nvarchar(500) not NULL,
	Value nvarchar(max) not null
)
