use NZOR_Data_Test

create table #Name(
	NameId uniqueidentifier null,
	NameGuid uniqueidentifier null,
	FullName nvarchar(100) null,
	GoverningCode nvarchar(5) null, 
	ProviderModifiedDate datetime null,
	Rank nvarchar(200) null)

insert into #Name(NameID, NameGuid, FullName, GoverningCode, ProviderModifiedDate, Rank)
select newid(), NameGUID, NameFull, NameNomCode,
	NameUpdatedDate, (select TaxonRankID from TaxonRank where Name = tr.TaxonRankName collate Latin1_General_CI_AS)
	--, fn.FlatNameCanonical
from proserver01.Plantnames.dbo.tblName na
	left join proserver01.Plantnames.dbo.tblFlatName fn on na.NameGuid = fn.FlatNameSeedName
		and fn.FlatNameCanonical = 'KingdomTemp' and fn.FlatNameRankName = 'Kingdom'
	left join proserver01.plantnames.dbo.tbltaxonrank tr on tr.taxonrankpk = na.nametaxonrankfk
	where fn.FlatNameCanonical is null
		and na.NameSuppress = 0
go


insert into provider.Name(
	NameID, LinkStatus, ProviderRecordId, DataSourceID, FullName, 
	GoverningCode, ProviderModifiedDate, AddedDate, NameClassId, TaxonRankID)
select NameId, 'Unmatched', NameGUID, '38708533-E064-45A4-AB00-CDD76075C2B6', --test provider
	FullName, GoverningCode,
	ProviderModifiedDate, GETDATE(), 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A',
	Rank
from #Name
where Rank is not null

go


insert into provider.NameProperty(
	NamePropertyId, NameID, NamePropertyTypeID,
	Value)
select newid(), na.nameid, 
	(Select NamePropertyTypeID from
		dbo.NamePropertyType where Name = 'Year'),
	pna.NameYearOfPublication 
from #Name na 
	inner join proserver01.Plantnames.dbo.tblName pna on na.NameGuid = pna.NameGuid
where pna.NameYearOfPublication is not null
		and pna.NameYearOfPublication <> ''
		and na.Rank is not null
go

insert into provider.NameProperty(
	NamePropertyId, NameID, NamePropertyTypeID,
	Value)
select newid(), na.nameid, 
	(Select NamePropertyTypeID from
		dbo.NamePropertyType where Name = 'Authors'),
	nukuna.NameAuthors 
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.NameGuid
where nukuna.NameAuthors is not null
		and nukuna.NameAuthors <> ''
		and na.Rank is not null
go

insert into provider.NameProperty(
	NamePropertyId, NameID, NamePropertyTypeID,
	Value)
select newid(), na.nameid, 
	(Select NamePropertyTypeID from
		dbo.NamePropertyType
		where Name = 'MicroReference'),
	nukuna.NamePage 
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.NameGuid
where nukuna.NamePage is not null
		and nukuna.NamePage <> ''
		and na.Rank is not null
go
		
insert into provider.NameProperty(
	NamePropertyId, NameID, NamePropertyTypeID,
	Value)
select newid(), na.nameid, 
	(Select NamePropertyTypeID from
		dbo.NamePropertyType
		where Name = 'Rank'),
	tr.TaxonRankName
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.NameGUID
	inner join proserver01.Plantnames.dbo.tblTaxonRank tr 
		on nukuna.NameTaxonRankFk = tr.TaxonRankPk
		where na.Rank is not null
go
		
insert into provider.NameProperty(
	NamePropertyId, NameID, NamePropertyTypeID, Value)
select newid(), na.nameid, 
	(Select NamePropertyTypeID from
		dbo.NamePropertyType
		where Name = 'Canonical'),
	nukuna.NameCanonical 
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.NameGuid
	where na.Rank is not null
go	

insert into provider.NameProperty(
	NamePropertyId, NameID, NamePropertyTypeID, Value, ProviderRelatedID)
select newid(), 
	na.nameid, 
	(Select NamePropertyTypeID from dbo.NamePropertyType
		where Name = 'PublishedIn'),
	ref.ReferenceGenCitation,
	nukuna.NameReferenceFk 
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.nameguid
	inner join proserver01.Plantnames.dbo.tblReference ref on nukuna.NameReferenceFk = ref.ReferenceID
 where nukuna.NameReferenceFk is not null 
	and na.Rank is not null
go

 
insert into provider.NameProperty(
	NamePropertyId, NameID, NamePropertyTypeID,
	Value, ProviderRelatedID)
select newid(), 
	na.nameid, 
	(Select NamePropertyTypeID from
		dbo.NamePropertyType where Name = 'Basionym'),
	nukub.NameFull,
	nukuna.NameBasionymFk
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.NameGuid
	inner join proserver01.Plantnames.dbo.tblName nukub on nukuna.NameBasionymFk = nukub.NameGuid
 where nukuna.NameReferenceFk is not null
	and na.Rank is not null
go


insert into provider.NameProperty(
	NamePropertyId, NameID, NamePropertyTypeID, Value)
select newid(), na.nameid, 
	(Select NamePropertyTypeID from
		dbo.NamePropertyType
		where Name = 'Orthography'),
	nukuna.NameOrthographyVariant 
from proserver01.Plantnames.dbo.tblName nukuna inner join
	#Name na on nukuna.NameGuid = na.NameGuid
	where nukuna.NameOrthographyVariant is not null
	 and nukuna.NameOrthographyVariant <> ''
	 and na.Rank is not null
go


drop table #Name
--drop table #RefTypes
--drop table #RefFieldType
go


--Update ranks
update pn
set pn.TaxonRankID = tr.TaxonRankID
from provider.Name pn 
inner join provider.NameProperty np on np.NameID = pn.NameID
inner join dbo.TaxonRank tr on tr.Name = np.Value
where np.NamePropertyTypeID = 'A1D57520-3D64-4F7D-97C8-69B449AFA280' and pn.TaxonRankID is null

go


--- CONCEPTS ---

/**
	do we need providers nameid on provider.concept?
	do we need providers reference id on provider.concept?
**/
select pna.NameId as NameId,
	NEWID() as ConceptId,
	bib.BibliographyOrthographyVariant as Orthography,
	bib.BibliographyGuid as ProviderRecordId, 
	bib.BibliographyUpdatedDate as ProviderModifiedDate,
	GETDATE() as AddedDate,
	bib.BibliographyReferenceFk as ProviderReferenceId,
	Bib.BibliographyNameFk as ProviderNameId,
	bib.BibliographyExplicit,
	bib.BibliographyNotes,
	(select DataSourceID from
		SubDataSet where Code='TEST_DS') as ProviderId
	into #Concept
	from proserver01.PlantNames.dbo.tblBibliography bib
	inner join provider.Name pna on bib.BibliographyNameFk = pna.ProviderRecordId
	where (bib.BibliographyIsDeleted is null or
		bib.BibliographyIsDeleted = 0)
go

create table #BibRelType(
	TypeId int null,
	NZORTypeId uniqueidentifier null)
go
	
insert into #BibRelType(TypeId)
select distinct bibr.BibliographyRelationshipTypeFk as TypeFk
	from proserver01.PlantNames.dbo.tblBibliographyRelationship bibr
	inner join #Concept con on bibr.BibliographyRelationshipBibliographyFromFk
		= con.ProviderRecordId
go

update Bibt
	set NZORTypeId = (select ConceptRelationshipTypeID
		from ConceptRelationshipType
			where [relationship]= 'is child of')
	from #Bibreltype bibt where bibt.typeId = 7 
go

update Bibt
	set NZORTypeId = (select ConceptRelationshipTypeID
		from ConceptRelationshipType
			where [relationship]= 'is synonym of')
	from #Bibreltype bibt where bibt.typeId = 8 
go

insert into provider.Concept(ConceptID, LinkStatus, NameID,
	Orthography, ProviderRecordID, ProviderModifiedDate, AddedDate,
	ProviderNameId, ProviderReferenceId, DataSourceID)
select ConceptId, 'Unmatched', NameID, Orthography, ProviderRecordId, ProviderModifiedDate,
		AddedDate, providerNameId, ProviderReferenceId, ProviderId
	from #concept
go

select newid() as ID,
	bibr.BibliographyRelationshipBibliographyFromFk as FromFK,
	bibr.BibliographyRelationshipBibliographyToFk as ToFK,
	bibr.BibliographyRelationshipTypeFk as RelTypeFK,
	con1.conceptid as FromCon,
	con2.conceptid as ToCon,
	bibt.NZORTypeId, 
	bibr.BibliographyRelationshipIsActive as IsActive
into #ConRel
	from proserver01.PlantNames.dbo.tblBibliographyRelationship bibr
	inner join #Concept con1 on bibr.BibliographyRelationshipBibliographyFromFk
		= con1.providerrecordid
	inner join #Concept con2 on bibr.BibliographyRelationshipBibliographyToFk
		= con2.ProviderRecordId
	inner join #BibRelType bibt on bibr.BibliographyRelationshipTypeFk
		= bibt.TypeId
	where bibt.NZORTypeId is not null	

insert into provider.ConceptRelationship(ConceptRelationshipID,
	FromConceptID, ToConceptID, ConceptRelationshipTypeID, InUse)
select cr.Id, cr.FromCon, cr.ToCon, NZORtypeId, IsActive
	from #ConRel	cr
go

drop table #BibRelType
drop table #Concept
drop table #ConRel

