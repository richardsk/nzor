
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
select NameId, NameGUID, '7B5CC893-C710-4119-ADE5-B00A997CEEAA', --test provider
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


drop table #Name
--drop table #RefTypes
--drop table #RefFieldType
go


--Update ranks
update pn
set pn.TaxonRankID = tr.TaxonRankID
from prov.Name pn 
inner join prov.NameProperty np on np.NameID = pn.NameID
inner join dbo.TaxonRank tr on tr.Name = np.Value
where np.NameClassPropertyID = 'A1D57520-3D64-4F7D-97C8-69B449AFA280' and pn.TaxonRankID is null

go


--- CONCEPTS ---

/**
	do we need providers nameid on prov.concept?
	do we need providers reference id on prov.concept?
**/
select pna.NameId as NameId,
	NEWID() as ConceptId,
	bib.BibliographyOrthographyVariant as Orthography,
	bib.BibliographyGuid as ProviderRecordId, 
	bib.BibliographyUpdatedDate as ProviderUpdatedDate,
	GETDATE() as AddedDate,
	bib.BibliographyReferenceFk as ProviderReferenceId,
	Bib.BibliographyNameFk as ProviderNameId,
	bib.BibliographyExplicit,
	bib.BibliographyNotes,
	(select ProviderId from
		Provider where Name='Test Provider') as ProviderId
	into #Concept
	from proserver01.PlantNames.dbo.tblBibliography bib
	inner join prov.Name pna on bib.BibliographyNameFk = pna.ProviderRecordId
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

insert into prov.Concept(ConceptID, NameID,
	Orthography, ProviderRecordID, ProviderUpdatedDate, AddedDate,
	ProviderNameId, ProviderReferenceId, ProviderID)
select ConceptId, NameID, Orthography, ProviderRecordId, ProviderUpdatedDate,
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

insert into prov.ConceptRelationship(ConceptRelationshipID,
	FromConceptID, ToConceptID, RelationshipTypeID, InUse)
select cr.Id, cr.FromCon, cr.ToCon, NZORtypeId, IsActive
	from #ConRel	cr
go

drop table #BibRelType
drop table #Concept
drop table #ConRel

