CREATE TABLE [admin].[NameRequest]
(
	NameRequestId uniqueidentifier not null, 
	FullName nvarchar(500) not null,
	RequestDate datetime not null,
	ApiKey nvarchar(250) null,
	BatchMatchId uniqueidentifier null,
	RequesterEmail nvarchar(1000) not null,
	ExternalLookupServiceId uniqueidentifier null,
	ExternalLookupDataUrl nvarchar(1000) null,
	Status nvarchar(200) not null,
    AddedDate datetime null,
    AddedBy nvarchar(150) null,
    ModifiedDate datetime null,
    ModifiedBy nvarchar(150) null
)
