declare @code nvarchar(150)
select @code = Code from [admin].DataSource where DataSourceId = @dataSourceId

INSERT INTO
	provider.Reference
	(
	ReferenceID, 
	
	ReferenceTypeID, 
	DataSourceID, 

	ConsensusReferenceID, 
	IntegrationBatchId,
	LinkStatus, 
	MatchScore, 
	MatchPath,

	ProviderRecordID, 
	ProviderCreatedDate, 
	ProviderModifiedDate, 
	
	AddedDate, 
	ModifiedDate
	)
values(
	@ReferenceID, 
	
	@ReferenceTypeID, 
	@DataSourceID, 
	
	NULL, 
	NULL,
	NULL, 
	NULL, 
	NULL,

	@code + '_' + cast(newid() as varchar(38)),	 --temp id
	@date, 
	null, 

	getdate(),	
	null)

insert provider.ReferenceProperty
(
	ReferencePropertyID, 
	
	ReferenceID, 
	ReferencePropertyTypeID, 
	
	SubType, 
	Sequence, 
	Level, 
	Value
)
values
(
	newid(), 
	
	@ReferenceID, 
	'7F835876-B459-4023-90E4-6C22646FBE07', 
	
	null, 
	null, 
	null, 
	@code + ' ' + datename(month, @date) + ' ' + datename(year, @date)
)