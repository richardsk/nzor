Use NZOR_Data
go

select distinct con.ProviderReferenceID
into #Missing
	from prov.Concept con
	left join prov.Reference ref on con.ProviderReferenceID = ref.ProviderRecordID
	where ref.ReferenceID is null
UNION	
select distinct np.ProviderRelatedID 
	from 
	prov.NameProperty np 
	inner join dbo.NameClassProperty ncp on np.NameClassPropertyID = ncp.NameClassPropertyID
	 and ncp.PropertyName = 'PublishedIn'
	left join prov.Reference r on np.ProviderRelatedID = r.ProviderRecordID
	where r.ReferenceID is null
union	
select distinct tp.ProviderReferenceID
	from prov.TaxonProperty tp
		left join prov.Reference ref on tp.ProviderReferenceID = ref.ProviderRecordID
	where ref.ReferenceID is null and tp.ProviderReferenceID is not null

select 
	NEWID() as ID,
	(select ProviderId from NZOR_Data.dbo.Provider where Name='Plant Names Database') as ProviderId,
	refs.ReferenceTypeID, refs.ReferenceID, refs.ReferenceUpdatedDate, 
	ReferenceTypeID as NzorRefTypeId
into #Refs
from proserver01.PlantNames.dbo.tblReference refs
	inner join #Missing m on refs.ReferenceID = m.ProviderReferenceId

update #Refs
	set NzorRefTypeId = null
go


select distinct #Refs.ReferenceTypeId, NzorRefTypeId, rt.ReferenceTypeText 
	into #RefTypes
from #Refs
	inner join proserver01.PlantNames.dbo.tblReferenceType rt on #Refs.ReferenceTypeId = rt.ReferenceTypeID
go

update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Journal')
from #RefTypes as rt where ReferenceTypeText = 'Journal'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Article')
from #RefTypes as  rt where ReferenceTypeText = 'Article in journal'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Article')
from #RefTypes as  rt where ReferenceTypeText = 'Article in magazine'
go

update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Chapter')
from #RefTypes as  rt where ReferenceTypeText = 'Chapter in book (in series)'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Book')
from #RefTypes as  rt where ReferenceTypeText = 'Book (in series)'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Catalog')
from #RefTypes as  rt where ReferenceTypeText = 'Catalog'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Pamphlet')
from #RefTypes as  rt where ReferenceTypeText = 'Pamphlet'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Abstract')
from #RefTypes as  rt where ReferenceTypeText = 'Abstract'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Magazine')
from #RefTypes as  rt where ReferenceTypeText = 'Magazine'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Series')
from #RefTypes as  rt where ReferenceTypeText = 'Series'
go

update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Serial')
from #RefTypes as  rt where ReferenceTypeText = 'Serial (book/monograph) in series'
go



update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Unpublished')
from #RefTypes as  rt where ReferenceTypeText = 'Unpublished work (in series)'
go

update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'In press')
from #RefTypes as  rt where ReferenceTypeText = 'In press'
go

update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Electronic Source')
from #RefTypes as  rt where ReferenceTypeText = 'Electronic citation in electronic source'
go

update rt
	set NzorRefTypeId = (Select ReferenceTypeId from ReferenceType
		where [TYPE] = 'Electronic Source')
from #RefTypes as  rt where ReferenceTypeText = 'Electronic source'
go


update refs
	set NzorRefTypeId = rt.NzorRefTypeId
from #Refs as Refs
	inner join #RefTypes rt on refs.ReferenceTypeId = rt.ReferenceTypeId
go
/*
select * from #RefTypes
select * from #Refs
*/

insert into prov.Reference(ReferenceId, ReferenceTypeId, ProviderId,
	ProviderRecordId, ProviderUpdatedDate, AddedDate)
select ID, NzorRefTypeId, ProviderId, ReferenceId, 
	ReferenceUpdatedDate, GETDATE()
	from #Refs
go

create table #RefFieldType(
	ReferenceFieldTypeId uniqueidentifier, 
	ReferenceFieldTypeText nvarchar(50) null,
	NZORFieldTypeId uniqueidentifier null)
go
insert into #RefFieldType(ReferenceFieldTypeId, ReferenceFieldTypeText)
select distinct rft.ReferenceFieldTypeID, ReferenceFieldTypeText
	from #Refs
		inner join proserver01.PlantNames.dbo.tblReferenceField rf on #refs.referenceid = rf.ReferenceID
		inner join proserver01.PlantNames.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
go

update rft
	set NZORFieldTypeId = (Select ReferenceFieldTypeId from
		ReferenceFieldType where [TYPE] = 'Date')
 from #RefFieldType rft
	where rft.ReferenceFieldTypeText = 'Date, Primary'
go

update rft
	set NZORFieldTypeId = (Select ReferenceFieldTypeId from
		ReferenceFieldType where [TYPE] = 'Author')
 from #RefFieldType rft
	where rft.ReferenceFieldTypeText = 'Author, primary'
go

update rft
	set NZORFieldTypeId = (Select ReferenceFieldTypeId from
		ReferenceFieldType where [TYPE] = 'Title')
 from #RefFieldType rft
	where rft.ReferenceFieldTypeText = 'Title'
go

update rft
	set NZORFieldTypeId = (Select ReferenceFieldTypeId from
		ReferenceFieldType where [TYPE] = 'End page')
 from #RefFieldType rft
	where rft.ReferenceFieldTypeText = 'End Page'
go

update rft
	set NZORFieldTypeId = (Select ReferenceFieldTypeId from
		ReferenceFieldType where [TYPE] = 'Start Page')
 from #RefFieldType rft
	where rft.ReferenceFieldTypeText = 'Start Page'
go


/*
select * from #RefFieldtype

*/
insert into prov.ReferenceField(ReferenceFieldId, ReferenceId, ReferenceFieldTypeId, [Value])
select newid(), ref.ID, rft.NZORFieldTypeId, rf.ReferenceFieldText
	from #Refs ref
	 inner join proserver01.PlantNames.dbo.tblReferenceField rf on ref.referenceId = rf.ReferenceID
	 inner join #RefFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeId
		and rft.NZORFieldTypeId is not null
go


drop table #Refs
drop table #RefTypes	
drop table #RefFieldType
drop table #missing