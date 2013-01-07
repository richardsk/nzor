use NZOR_Data
go

--create synonym pName for NZOR_Data.prov.Name
--create synonym pNameProp for NZOR_Data.prov.NameProperty
--create synonym pNamePropClass for NZOR_Data.dbo.NameClassProperty
--create synonym pRefField for NZOR_Data.prov.ReferenceField
--create synonym pRef for NZOR_Data.prov.Reference
--create synonym pRefType for NZOR_Data.dbo.ReferenceType
--create synonym pRefFieldType for NZOR_Data.dbo.ReferenceFieldType
--go

delete from NZOR_Data.prov.ConceptRelationship
delete from NZOR_Data.prov.Concept
go

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
		NZOR_Data.dbo.Provider where Name='Plant Names Database') as ProviderId
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
		from NZOR_Data.dbo.ConceptRelationshipType
			where [relationship]= 'is child of')
	from #Bibreltype bibt where bibt.typeId = 7 
go

update Bibt
	set NZORTypeId = (select ConceptRelationshipTypeID
		from NZOR_Data.dbo.ConceptRelationshipType
			where [relationship]= 'is synonym of')
	from #Bibreltype bibt where bibt.typeId = 8 
go

insert into NZOR_Data.prov.Concept(ConceptID, NameID,
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

insert into NZOR_Data.prov.ConceptRelationship(ConceptRelationshipID,
	FromConceptID, ToConceptID, RelationshipTypeID, InUse)
select cr.Id, cr.FromCon, cr.ToCon, NZORtypeId, IsActive
	from #ConRel	cr
go

drop table #BibRelType
drop table #Concept
drop table #ConRel

--drop synonym pName
--drop synonym pNameProp
--drop synonym pNamePropClass
--drop synonym pRefField
--drop synonym pRef
--drop synonym pRefType
--drop synonym pRefFieldType

