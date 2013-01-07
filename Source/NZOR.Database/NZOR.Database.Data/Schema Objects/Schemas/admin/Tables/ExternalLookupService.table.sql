CREATE TABLE [admin].[ExternalLookupService]
(
	ExternalLookupServiceId uniqueidentifier not null primary key,
	Name nvarchar(250) not null,
	Description nvarchar(500) null,
	DataFormat nvarchar(150) not null,
	NameLookupEndpoint nvarchar(500) null,
	ConceptLookupEndpoint nvarchar(500) null,
	ReferenceLookupEndpoint nvarchar(500) null,
	IDLookupEndpoint nvarchar(500) null,
	SpaceCharacterSubstitute nvarchar(10) null,
	IconFilename nvarchar(100) null,
	LookupServiceClassName nvarchar(500) null
)
