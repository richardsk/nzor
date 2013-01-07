CREATE TABLE [admin].[BrokeredName]
(
	BrokeredNameId uniqueidentifier not null, 
	NameRequestId uniqueidentifier not null,
	ExternalLookupServiceId uniqueidentifier not null,
	ProviderRecordId nvarchar(1000) not null,
	NZORProviderNameId uniqueidentifier null,
	DataUrl nvarchar(2000) null,
	WebUrl nvarchar(2000) null,
    AddedDate datetime null,
    AddedBy nvarchar(150) null,
    ModifiedDate datetime null,
    ModifiedBy nvarchar(150) null	
)
