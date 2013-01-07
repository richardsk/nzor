IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PNDCache20]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[PNDCache20]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[PNDCache20]
as

select vu.* ,
	bib.BibliographyGuid,
	Bib.BibliographyReferenceFk,
	va.VernacularArticleCounterPK,
	va.VernacularArticleReferenceFk
into #ConRels
	from tblVernacularUse vu
	 inner join tblBibliography bib on vu.VernacularUseNameFk
		= bib.BibliographyNameFk
			and (bib.BibliographyIsDeleted is null or bib.BibliographyIsDeleted = 0)
	 left join tblVernacularArticle va on vu.VernacularUseCounterPk = va.VernacularArticleVernacularUseFK
		and va.VernacularArticleReferenceFk = bib.BibliographyReferenceFk	
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.name n on n.nameid = vu.VernacularUseNameFk
	where isnull(vu.VernacularUseSuppress,0) = 0
		and (vu.VernacularUseIsDeleted = 0 or vu.VernacularUseIsDeleted is null)
		and (bib.bibliographyreferencefk is null or bib.bibliographyreferencefk not in ('EB3B0740-917C-441D-8AE0-3185E7003ED9'))
		
select distinct 
	vn.VernacularGuid,
	vn.VernacularName,
	vn.VernacularAddedDate,
	vn.VernacularUpdatedDate,
	la.LanguageEnglish,
	g.GeoRegionName
	into #Vern
 from tblVernacular vn
	left join tblLanguage la on vn.VernacularLanguageFk = la.LanguageCounterPK
	left join tblGeoRegion g on vn.VernacularGeoRegionFk = g.GeoRegionCounterPK
	inner join #Conrels cr on vn.VernacularGuid = cr.VernacularUseVernacularFK
	where vn.vernacularSuppress is null or vn.vernacularsuppress = 0
		--and la.languageenglish is not null


/*
select *
	from #Vern
*/

insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.Vernacular(
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
	vu.VernacularUseIsTradeName
	into #Concept
	from tblVernacularArticle va
		inner join tblVernacularUse vu on va.VernacularArticleVernacularUseFK = vu.VernacularUseCounterPk
			and (vu.VernacularUseIsDeleted = 0 or vu.VernacularUseIsDeleted is null)
			and isnull(vu.VernacularUseSuppress,0) = 0
		inner join #Vern v on vu.VernacularUseVernacularFk = v.VernacularGUID
		inner join #ConRels cr on va.VernacularArticleCounterPK = cr.vernaculararticlecounterpk
	where VernacularArticleIsDeleted is null or VernacularArticleIsDeleted = 0
		and (va.vernaculararticlereferencefk is null or va.vernaculararticlereferencefk not in ('EB3B0740-917C-441D-8AE0-3185E7003ED9'))


insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.VernacularConcept(
	VernacularConceptId, --CreatedDate, ModifiedDate, 
	Name, NameId, AccordingTo, AccordingToId)
select distinct con.ConceptId,
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

insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.ConceptApplication(
	ConceptApplicationId, FromConceptId, ToConceptId, Gender,
	PartOfTaxon, GeoRegion, GeographicSchema, InUse, [type])
select c.VernacularUseCounterPK,
	cr.VernacularArticleCounterPk FromConceptId,
	cr.BibliographyGuid ToConceptId, 
	--(Select top 1 conceptid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.Concept con where
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



--insert "name based vernaculars" - ie vernaculars without a bibliogtraphy, just a vernacular connected to a scientific name 
insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.Vernacular(
	VernacularId, nameFull, --PublishedIn, PublishedInId,
	[Language], Country, CreatedDate, ModifiedDate)
select distinct 
	vn.VernacularGuid,
	vn.VernacularName,
	la.LanguageEnglish,
	g.GeoRegionName,
	vn.VernacularAddedDate,
	vn.VernacularUpdatedDate
from tblVernacular vn
	inner join tblVernacularUse vu on vn.VernacularGuid = vu.VernacularUseVernacularFK	
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.name n on n.nameid = vu.vernacularusenamefk
	left join tblLanguage la on vn.VernacularLanguageFk = la.LanguageCounterPK
	left join tblGeoRegion g on vn.VernacularGeoRegionFk = g.GeoRegionCounterPK
	left join #Conrels cr on vn.VernacularGuid = cr.VernacularUseVernacularFK	
where isnull(vn.vernacularSuppress,0) = 0 and cr.vernacularusecounterpk is null

declare @accToId uniqueidentifier
if (not exists(select publicationid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.publication where citation = 'NZFLORA'))
begin
	set @accToId = NEWID()
	insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.publication(PublicationID, Citation, [Type]) select @accToId, 'NZFLORA', 'Electronic source'
	insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.publicationTitle(PublicationID, Title, Level, Type) select @accToId, 'NZFLORA', 1, 'full'
end
else
begin
	select @accToId = publicationid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.publication where citation = 'NZFLORA'
end

insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.vernacularuse
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
left join tblVernacularUse vu on vn.VernacularGuid = vu.VernacularUseVernacularFK	
left join tblGeoRegion g on vn.VernacularGeoRegionFk = g.GeoRegionCounterPK
left join tblGeoRegionSchema geos on g.GeoRegionSchemaFK = geos.GeoRegionSchemaCounterPK
left join tblLanguage la on vu.VernacularUseLanguageFk = la.LanguageCounterPK		
left join tblVernacularArticle va on vu.VernacularUseCounterPk = va.VernacularArticleVernacularUseFK
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.name n on n.nameid = vu.vernacularusenamefk
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.vernacular v on v.vernacularid = vu.VernacularUseVernacularFk
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.vernacularconcept vc on vc.vernacularconceptid = va.VernacularArticleCounterPk
left join #Conrels cr on vn.VernacularGuid = cr.VernacularUseVernacularFK		
where isnull(vu.VernacularUseSuppress,0) = 0 and isnull(vu.VernacularUseIsDeleted,0) = 0 and
	vc.vernacularconceptid is null
	

 
 drop table #Vern
 drop table #Concept
 drop table #ConRels
 
go

grant execute on dbo.[PNDCache20] to dbi_user

go
