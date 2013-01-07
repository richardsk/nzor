use NZOR_Data
go

delete from NZOR_Data.prov.Concept
 from NZOR_Data.prov.Concept c inner join NZOR_Data.prov.Name n	on c.NameID = n.NameID
	where NameClassID = (Select NameClassId from 
		NZOR_Data.dbo.NameClass where Title = 'Vernacular Name')

delete from NZOR_Data.prov.Name	
	where NameClassID = (Select NameClassId from 
		NZOR_Data.dbo.NameClass where Title = 'Vernacular Name')
go

----select vu.VernacularUseCounterPk,
----		vu.VernacularUseVernacularFk,
----		vu.VernacularUseNameFk,
----		vu.VernacularUseSource,
----		geo.GeoRegionName,
----		geos.GeoRegionSchemaName,
----		vu.VernacularUseAppliesTo,
----		vu.VernacularUseIsTradeName,
----		vu.VernacularUseAddedDate,
----		vu.VernacularUseUpdatedDate
----	from tblVernacularUse vu
----		inner join tblLanguage la on vu.VernacularUseLanguageFk = la.LanguageCounterPK
----		inner join tblGeoRegion geo on vu.VernacularUseGeoRegionFk = geo.GeoRegionCounterPK
----		inner join tblGeoRegionSchema geos on geo.GeoRegionSchemaFK = geos.GeoRegionSchemaCounterPK
----		inner join tblVernacular v on vu.VernacularUseVernacularFk = v.VernacularGuid
----			and (v.VernacularSuppress = 0 or v.VernacularSuppress is null)
----where vu.VernacularUseSuppress = 0
----		and (vu.VernacularUseIsDeleted = 0 or vu.VernacularUseIsDeleted is null)

select vu.* ,
	bib.BibliographyGuid,
	Bib.BibliographyReferenceFk,
	va.VernacularArticleCounterPK,
	va.VernacularArticleReferenceFk
into #ConRels
	from Proserver01.PlantNames.dbo.tblVernacularUse vu
	 inner join Proserver01.PlantNames.dbo.tblBibliography bib on vu.VernacularUseNameFk
		= bib.BibliographyNameFk
			and (bib.BibliographyIsDeleted is null or bib.BibliographyIsDeleted = 0)
	 inner join Proserver01.PlantNames.dbo.tblVernacularArticle va on vu.VernacularUseCounterPk = va.VernacularArticleVernacularUseFK
		and va.VernacularArticleReferenceFk = bib.BibliographyReferenceFk
	where vu.VernacularUseSuppress = 0
		and (vu.VernacularUseIsDeleted = 0 or vu.VernacularUseIsDeleted is null)
go		

/*
select na.NameFull, v.VernacularName, con.* 
	from #Conrels con inner join tblName na on con.vernacularuseNamefk = na.nameguid
		inner join tblVernacular v on con.vernacularusevernacularfk = v.VernacularGuid
	order by na.NameFull, v.VernacularName
*/

select distinct --NEWID() as ID,
	vn.VernacularGuid,
	vn.VernacularName,
	vn.VernacularAddedDate,
	vn.VernacularUpdatedDate,
	la.LanguageEnglish
	into #Vern
 from Proserver01.PlantNames.dbo.tblVernacular vn
	inner join Proserver01.PlantNames.dbo.tblLanguage la on vn.VernacularLanguageFk = la.LanguageCounterPK
	inner join #Conrels cr on vn.VernacularGuid = cr.VernacularUseVernacularFK
	where vn.vernacularSuppress is null or vn.vernacularsuppress = 0
go
alter table #Vern add ID uniqueidentifier
go
update #Vern
	set ID = NEWID()
go

/*
select *
	from #Vern
*/

insert into NZOR_Data.prov.Name(NameID, FullName, NameClassID,
	ProviderID, ProviderRecordID, ProviderUpdatedDate, AddedDate)
select id, vernacularName, (Select NameClassId from 
		NZOR_Data.dbo.NameClass where Title = 'Vernacular Name'),
	(Select ProviderId from nzor_data.dbo.Provider where Name = 'Plant Names database'),
	VernacularGUID,
	VernacularUpdatedDate,
	getdate()
	from #Vern
go
-- select from vernacular article into concepts

select NEWID() as ID,
	va.VernacularArticleCounterPK,
	va.VernacularArticleReferenceFk,
	vu.VernacularUseCounterPk,
	vu.VernacularUseVernacularFk,
	vu.VernacularUseNameFk,
	vu.VernacularUseGeoRegionFk,
	vu.VernacularUseIsTradeName,
	v.id as NameId
	into #Concept
	from Proserver01.PlantNames.dbo.tblVernacularArticle va
		inner join Proserver01.PlantNames.dbo.tblVernacularUse vu on va.VernacularArticleVernacularUseFK = vu.VernacularUseCounterPk
			and (vu.VernacularUseIsDeleted = 0 or vu.VernacularUseIsDeleted is null)
			and vu.VernacularUseSuppress = 0
		inner join #Vern v on vu.VernacularUseVernacularFk = v.VernacularGUID
		inner join #ConRels cr on va.VernacularArticleCounterPK = cr.vernaculararticlecounterpk
	where VernacularArticleIsDeleted is null or VernacularArticleIsDeleted = 0
go


insert into NZOR_Data.prov.Concept(ConceptID, ProviderNameID,
	NameID, ProviderReferenceID, ProviderID, ProviderRecordID,
	ProviderUpdatedDate, AddedDate)
select ID, VernacularUseVernacularFk, 
	NameId, VernacularArticleReferenceFk,
	(Select ProviderId from nzor_data.dbo.Provider where Name = 'Plant Names database'),
	VernacularArticleCounterPK,
	null,
	GETDATE()
	from #Concept
go

/*
select * from #conrels
select * from #concept
*/

insert into NZOR_Data.prov.ConceptRelationship(ConceptRelationshipID, FromConceptID, ToConceptID,
	RelationshipTypeID,	InUse)
select newid(), 
	c.id,
	(Select top 1 conceptid from NZOR_Data.prov.Concept con where
		con.ProviderRecordID = cast(cr.bibliographyguid as varchar(50))) ToConceptId,
	(Select ConceptRelationshipTypeID from NZOR_Data.dbo.ConceptRelationshipType crt
		where crt.Relationship = 'is vernacular for'),
	1
	from #ConRels cr
	 inner join #Concept c on cr.VernacularUseCounterPk = c.VernacularUseCounterPk


 
 drop table #Vern
 drop table #Concept
 drop table #ConRels