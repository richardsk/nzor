use NZOR_Data
go

create table #Name(
	NameId uniqueidentifier null,
	NameGuid uniqueidentifier null,
	FullName nvarchar(100) null,
	OriginalOrthography nvarchar(100) null, 
	GoverningCode nvarchar(5) null, 
	ProviderUpdatedDate datetime null)

insert into #Name(
	NameID, NameGuid, FullName, OriginalOrthography, 
	GoverningCode, ProviderUpdatedDate)
select newid(), NameGUID, NameFull, NameOrthographyVariant, NameNomCode,
	NameUpdatedDate
	--, fn.FlatNameCanonical
from proserver01.Plantnames.dbo.tblName na
	left join proserver01.Plantnames.dbo.tblFlatName fn on na.NameGuid = fn.FlatNameSeedName
		and fn.FlatNameCanonical = 'KingdomTemp' and fn.FlatNameRankName = 'Kingdom'
	where fn.FlatNameCanonical is null
		and na.NameSuppress = 0
go


insert into prov.Name(
	NameID, ProviderRecordId, ProviderId, FullName, OriginalOrthography, 
	GoverningCode, ProviderUpdatedDate, AddedDate, NameClassId)
select NameId, NameGUID, 'B8E1EF06-1F7D-43CE-BF30-71735E600A96',
	FullName, OriginalOrthography, GoverningCode,
	ProviderUpdatedDate, GETDATE(), 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A'
from #Name
go


insert into prov.NameProperty(
	NamePropertyId, NameID, NameClassPropertyId,
	Value)
select newid(), na.nameid, 
	(Select NameClassPropertyId from
		dbo.NameClassProperty where PropertyName = 'Year'),
	pna.NameYearOfPublication 
from #Name na 
	inner join proserver01.Plantnames.dbo.tblName pna on na.NameGuid = pna.NameGuid
where pna.NameYearOfPublication is not null
		and pna.NameYearOfPublication <> ''
go

insert into prov.NameProperty(
	NamePropertyId, NameID, NameClassPropertyId,
	Value)
select newid(), na.nameid, 
	(Select NameClassPropertyId from
		dbo.NameClassProperty where PropertyName = 'Authors'),
	nukuna.NameAuthors 
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.NameGuid
where nukuna.NameAuthors is not null
		and nukuna.NameAuthors <> ''
go

insert into prov.NameProperty(
	NamePropertyId, NameID, NameClassPropertyId,
	Value)
select newid(), na.nameid, 
	(Select NameClassPropertyId from
		dbo.NameClassProperty
		where PropertyName = 'MicroReference'),
	nukuna.NamePage 
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.NameGuid
where nukuna.NamePage is not null
		and nukuna.NamePage <> ''
go
		
insert into prov.NameProperty(
	NamePropertyId, NameID, NameClassPropertyId,
	Value)
select newid(), na.nameid, 
	(Select NameClassPropertyId from
		dbo.NameClassProperty
		where PropertyName = 'Rank'),
	tr.TaxonRankName
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.NameGUID
	inner join proserver01.Plantnames.dbo.tblTaxonRank tr 
		on nukuna.NameTaxonRankFk = tr.TaxonRankPk
go
		
insert into prov.NameProperty(
	NamePropertyId, NameID, NameClassPropertyId, Value)
select newid(), na.nameid, 
	(Select NameClassPropertyId from
		dbo.NameClassProperty
		where PropertyName = 'Canonical'),
	nukuna.NameCanonical 
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.NameGuid
go	

insert into prov.NameProperty(
	NamePropertyId, NameID, NameClassPropertyId, Value, ProviderRelatedID)
select newid(), 
	na.nameid, 
	(Select NameClassPropertyId from dbo.NameClassProperty
		where PropertyName = 'PublishedIn'),
	ref.ReferenceGenCitation,
	nukuna.NameReferenceFk 
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.nameguid
	inner join proserver01.Plantnames.dbo.tblReference ref on nukuna.NameReferenceFk = ref.ReferenceID
 where nukuna.NameReferenceFk is not null
go

 
insert into prov.NameProperty(
	NamePropertyId, NameID, NameClassPropertyId,
	Value, ProviderRelatedID)
select newid(), 
	na.nameid, 
	(Select NameClassPropertyId from
		dbo.NameClassProperty where PropertyName = 'Basionym'),
	nukub.NameFull,
	nukuna.NameBasionymFk
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.NameGuid
	inner join proserver01.Plantnames.dbo.tblName nukub on nukuna.NameBasionymFk = nukub.NameGuid
 where nukuna.NameReferenceFk is not null
go


insert into prov.NameProperty(
	NamePropertyId, NameID, NameClassPropertyId, Value)
select newid(), na.nameid, 
	(Select NameClassPropertyId from
		dbo.NameClassProperty
		where PropertyName = 'Orthography'),
	nukuna.NameOrthographyVariant 
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.NameGuid
	where nukuna.NameOrthographyVariant is not null
	 and nukuna.NameOrthographyVariant <> ''
go

/**
References to do
1) expand to get parent references
2) add additional fields
3) add parent field id
**/

/*
select distinct ReferenceID as NzorRefId, (select ProviderId from
	NZOR_Data.dbo.Provider where Name='Plant Names Database') as ProviderId,
	refs.ReferenceTypeID, refs.ReferenceID, refs.ReferenceUpdatedDate, GETDATE() as AddedDate,
	ReferenceTypeID as NzorRefTypeId
into #Refs
from proserver01.Plantnames.dbo.tblReference refs 
	inner join proserver01.Plantnames.dbo.tblName nan on refs.ReferenceID = nan.NameReferenceFk
	inner join #Name na1 on nan.NameGuid = na1.NameGuid

update #Refs
	set NzorRefTypeId = null
go

update #Refs
	set NzorRefId = NEWID()
go

select distinct #Refs.ReferenceTypeId, NzorRefTypeId, rt.ReferenceTypeText 
	into #RefTypes
from #Refs
	inner join proserver01.Plantnames.dbo.tblReferenceType rt on #Refs.ReferenceTypeId = rt.ReferenceTypeID
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from dbo.ReferenceType
		where [TYPE] = 'Journal')
from #RefTypes as rt where ReferenceTypeText = 'Journal'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from dbo.ReferenceType
		where [TYPE] = 'Article')
from #RefTypes as  rt where ReferenceTypeText = 'Article in journal'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from dbo.ReferenceType
		where [TYPE] = 'Article')
from #RefTypes as  rt where ReferenceTypeText = 'Article in magazine'
go

update rt
	set NzorRefTypeId = (Select ReferenceTypeId from dbo.ReferenceType
		where [TYPE] = 'Chapter')
from #RefTypes as  rt where ReferenceTypeText = 'Chapter in book (in series)'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from dbo.ReferenceType
		where [TYPE] = 'Book')
from #RefTypes as  rt where ReferenceTypeText = 'Book (in series)'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from dbo.ReferenceType
		where [TYPE] = 'Catalog')
from #RefTypes as  rt where ReferenceTypeText = 'Catalog'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from dbo.ReferenceType
		where [TYPE] = 'Pamphlet')
from #RefTypes as  rt where ReferenceTypeText = 'Pamphlet'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from dbo.ReferenceType
		where [TYPE] = 'Abstract')
from #RefTypes as  rt where ReferenceTypeText = 'Abstract'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from dbo.ReferenceType
		where [TYPE] = 'Magazine')
from #RefTypes as  rt where ReferenceTypeText = 'Magazine'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from dbo.ReferenceType
		where [TYPE] = 'Series')
from #RefTypes as  rt where ReferenceTypeText = 'Series'
go
update rt
	set NzorRefTypeId = (Select ReferenceTypeId from dbo.ReferenceType
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


update refs
	set NzorRefTypeId = rt.NzorRefTypeId
from #Refs as Refs
	inner join #RefTypes rt on refs.ReferenceTypeId = rt.ReferenceTypeId
go


insert into prov.Reference(ReferenceId, ReferenceTypeId, ProviderId,
	ProviderRecordId, ProviderUpdatedDate, AddedDate)
select NZORRefId, NzorRefTypeId, ProviderId, ReferenceId, 
	ReferenceUpdatedDate, AddedDate
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
		inner join proserver01.Plantnames.dbo.tblReferenceField rf on #refs.referenceid = rf.ReferenceID
		inner join proserver01.Plantnames.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
go


update rft
	set NZORFieldTypeId = (Select ReferenceFieldTypeId from
		dbo.ReferenceFieldType where [TYPE] = 'Date')
 from #RefFieldType rft
	where rft.ReferenceFieldTypeText = 'Date, Primary'
go

update rft
	set NZORFieldTypeId = (Select ReferenceFieldTypeId from
		dbo.ReferenceFieldType where [TYPE] = 'Author')
 from #RefFieldType rft
	where rft.ReferenceFieldTypeText = 'Author, primary'
go

update rft
	set NZORFieldTypeId = (Select ReferenceFieldTypeId from
		dbo.ReferenceFieldType where [TYPE] = 'Title')
 from #RefFieldType rft
	where rft.ReferenceFieldTypeText = 'Title'
go

update rft
	set NZORFieldTypeId = (Select ReferenceFieldTypeId from
		dbo.ReferenceFieldType where [TYPE] = 'End page')
 from #RefFieldType rft
	where rft.ReferenceFieldTypeText = 'End Page'
go

update rft
	set NZORFieldTypeId = (Select ReferenceFieldTypeId from
		dbo.ReferenceFieldType where [TYPE] = 'Start Page')
 from #RefFieldType rft
	where rft.ReferenceFieldTypeText = 'Start Page'
go

insert into prov.ReferenceField(ReferenceFieldId, ReferenceId, ReferenceFieldTypeId, [Value])
select newid(), ref.NzorRefId, rft.NZORFieldTypeId, rf.ReferenceFieldText
	from #Refs ref
	 inner join proserver01.Plantnames.dbo.tblReferenceField rf on ref.referenceId = rf.ReferenceID
	 inner join #RefFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeId
		and rft.NZORFieldTypeId is not null
go


*/


drop table #Name
--drop table #RefTypes
--drop table #RefFieldType
go
