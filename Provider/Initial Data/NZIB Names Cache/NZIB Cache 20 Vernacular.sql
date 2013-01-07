IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NZIBCache20]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[NZIBCache20]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[NZIBCache20]
as

select vu.* ,
	bib.BibliographyGuid,
	Bib.BibliographyReferenceFk,
	va.VernacularArticleCounterPK,
	va.VernacularArticleReferenceFk	
into #ConRels
	from tblVernacularUse vu
	 inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.name n on n.nameid = vu.vernacularusenamefk
	 inner join tblVernacularArticle va on vu.VernacularUseCounterPk = va.VernacularArticleVernacularUseFK
	 left join tblBibliography bib on vu.VernacularUseNameFk = bib.BibliographyNameFk
			and isnull(bib.BibliographyIsDeleted,0) = 0
			and bib.BibliographyReferenceFk = va.VernacularArticleReferenceFk
	where isnull(vu.VernacularUseSuppress,0) = 0
		and isnull(vu.VernacularUseIsDeleted,0) = 0
	

select distinct 
	vn.VernacularGuid,
	vn.VernacularName,
	vn.VernacularAddedDate,
	vn.VernacularUpdatedDate,
	la.LanguageEnglish,
	g.GeoRegionName
	into #Vern
 from tblVernacular vn
	left join tblGeoRegion g on vn.VernacularGeoRegionFk = g.GeoRegionCounterPK
	inner join #Conrels cr on vn.VernacularGuid = cr.VernacularUseVernacularFK	
	left join tblLanguage la on cr.VernacularUseLanguageFk = la.LanguageCounterPK
	where isnull(vn.vernacularSuppress,0) = 0


/*
select *
	from #Vern
*/

insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.Vernacular(
	VernacularId, nameFull, --PublishedIn, PublishedInId,
	[Language], Country, CreatedDate, ModifiedDate)
select VernacularGuid,
	vernacularName,
	languageEnglish,
	GeoregionName,
	VernacularaddedDate,
	vernacularUpdatedDate
from #Vern

-- select from vernacular article into concepts

select va.VernacularArticleCounterPK as ConceptId,
	va.VernacularArticleReferenceFk as AccordingToId,
	vu.VernacularUseCounterPk,
	vu.VernacularUseVernacularFk,
	vu.VernacularUseNameFk,
	vu.VernacularUseGeoRegionFk,
	vu.VernacularUseIsTradeName,
	vu.VernacularUseAddedDate,
	vu.VernacularUseUpdatedDate
	into #Concept
	from tblVernacularArticle va
		inner join tblVernacularUse vu on va.VernacularArticleVernacularUseFK = vu.VernacularUseCounterPk
			and isnull(vu.VernacularUseIsDeleted,0) = 0
			and isnull(vu.VernacularUseSuppress,0) = 0
		inner join #Vern v on vu.VernacularUseVernacularFk = v.VernacularGUID
		inner join #ConRels cr on va.VernacularArticleCounterPK = cr.vernaculararticlecounterpk		
	where isnull(VernacularArticleIsDeleted,0) = 0


insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.VernacularConcept(
	VernacularConceptId, CreatedDate, ModifiedDate, 
	Name, NameId, AccordingTo, AccordingToId)
select distinct con.ConceptId,
	con.VernacularUseAddedDate,
	con.VernacularUseUpdatedDate,
	vern.VernacularName,
	 con.VernacularUseVernacularFk, 
	ref.ReferenceGenCitation,
	con.AccordingToId
	from #Concept con		
		left join #Vern vern on con.VernacularUseVernacularFk = vern.VernacularGuid
		left join tblReference ref on con.AccordingToId = ref.ReferenceID

/*
select * from #conrels
select * from #concept
*/

insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.ConceptApplication(
	ConceptApplicationId, FromConceptId, ToConceptId, ToNameId, Gender,
	PartOfTaxon, GeoRegion, GeographicSchema, InUse, [type])
select distinct c.VernacularUseCounterPK,
	cr.VernacularArticleCounterPk FromConceptId,
	cr.BibliographyGuid ToConceptId, 
	cr.vernacularusenamefk ToNameId,
	--(Select top 1 conceptid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.Concept con where
	--	con.ConceptId = cast(cr.bibliographyguid as varchar(50))) ToConceptId,
	null as gender,
	cr.VernacularUseAppliesTo,
	geo.GeoRegionName,
	geos.GeoRegionSchemaName,
	1,'is vernacular for'
	from #ConRels cr
	 inner join #Concept c on cr.VernacularUseCounterPk = c.VernacularUseCounterPk
	 left join tblGeoRegion geo on cr.VernacularUseGeoregionfk = geo.GeoRegionCounterPK
	 left join tblGeoRegionSchema geos on geo.GeoRegionSchemaFK = geos.GeoRegionSchemaCounterPK
	 left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonConcept tc on tc.TaxonConceptId = cr.BibliographyGuid
	 left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonNameUse nbc on nbc.NameID = c.VernacularUseNameFk
		--and isnull(nbc.accordingtoid, '00000000-0000-0000-0000-000000000000') = isnull(c.accordingtoid,'00000000-0000-0000-0000-000000000000')
where tc.TaxonConceptID is not null or nbc.TaxonNameUseID is not null
	
	
--insert "name based vernaculars" - ie vernaculars without a bibliogtraphy, just a vernacular connected to a scientific name 
insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.Vernacular(
	VernacularId, nameFull, --PublishedIn, PublishedInId,
	[Language], Country, CreatedDate, ModifiedDate)
select distinct 
	vn.VernacularGuid,
	vn.VernacularName,
	isnull(la.LanguageEnglish,'English'),
	g.GeoRegionName,
	vn.VernacularAddedDate,
	vn.VernacularUpdatedDate
from tblVernacular vn
	inner join tblVernacularUse vu on vn.VernacularGuid = vu.VernacularUseVernacularFK	
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.name n on n.nameid = vu.vernacularusenamefk
	left join tblLanguage la on vn.VernacularLanguageFk = la.LanguageCounterPK
	left join tblGeoRegion g on vn.VernacularGeoRegionFk = g.GeoRegionCounterPK
	left join #Conrels cr on vn.VernacularGuid = cr.VernacularUseVernacularFK
where isnull(vn.vernacularSuppress,0) = 0 and cr.vernacularusecounterpk is null

declare @accToId uniqueidentifier
if (not exists(select publicationid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.publication where citation = 'NZIB'))
begin
	set @accToId = NEWID()
	insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.publication(PublicationID, Citation, [Type]) select @accToId, 'NZIB', 'Electronic source'
	insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.publicationTitle(PublicationID, Title, Level, Type) select @accToId, 'NZIB', 1, 'full'
end
else
begin
	select @accToId = publicationid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.publication where citation = 'NZIB'
end

insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.vernacularuse
	([VernacularId]
		,CreatedDate
       ,[AccordingToId]
       ,[TaxonNameId]
       ,[Rank]
       ,[Gender]
       ,[PartOfTaxon]
       ,[GeoRegion]
       ,[GeographicSchema]
       ,[LifeStage])
select distinct 
	vn.VernacularGuid,
	getdate(),
	isnull(va.VernacularArticleReferenceFk, @accToId),
	vu.VernacularUseNameFk,	
	null as Rank,
	null as Gender,	
	null as PartOfTaxon,
	g.GeoRegionName,
	geos.GeoRegionSchemaName,
	null as LifeStage
from tblVernacular vn
inner join tblVernacularUse vu on vn.VernacularGuid = vu.VernacularUseVernacularFK	
left join tblGeoRegion g on vn.VernacularGeoRegionFk = g.GeoRegionCounterPK
left join tblGeoRegionSchema geos on g.GeoRegionSchemaFK = geos.GeoRegionSchemaCounterPK
left join tblLanguage la on vu.VernacularUseLanguageFk = la.LanguageCounterPK		
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.name n on n.nameid = vu.vernacularusenamefk
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.vernacular v on v.vernacularid = vu.VernacularUseVernacularFk
left join #Conrels cr on vn.VernacularGuid = cr.VernacularUseVernacularFK
left join tblVernacularArticle va on vu.VernacularUseCounterPk = va.VernacularArticleVernacularUseFK		
where isnull(vu.VernacularUseSuppress,0) = 0 and isnull(vu.VernacularUseIsDeleted,0) = 0
	and cr.vernacularusecounterpk is null

  
 drop table #Vern
 drop table #Concept
 drop table #ConRels
 
  
go

grant execute on dbo.[NZIBCache20] to dbi_user

go
