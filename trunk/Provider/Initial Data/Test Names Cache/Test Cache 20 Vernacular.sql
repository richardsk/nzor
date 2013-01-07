IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestCache20]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TestCache20]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TestCache20]
as

---------------------------------
--Plant Name vernaculars
--TODO not tested yet
/*
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
	 inner join tblVernacularArticle va on vu.VernacularUseCounterPk = va.VernacularArticleVernacularUseFK
		and va.VernacularArticleReferenceFk = bib.BibliographyReferenceFk
	where vu.VernacularUseSuppress = 0
		and (vu.VernacularUseIsDeleted = 0 or vu.VernacularUseIsDeleted is null)


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


/*
select *
	from #Vern
*/

insert into [devserver02\sql2005].PlantName_Cache.dbo.Vernacular(
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
			and vu.VernacularUseSuppress = 0
		inner join #Vern v on vu.VernacularUseVernacularFk = v.VernacularGUID
		inner join #ConRels cr on va.VernacularArticleCounterPK = cr.vernaculararticlecounterpk
	where VernacularArticleIsDeleted is null or VernacularArticleIsDeleted = 0



insert into [devserver02\sql2005].PlantName_Cache.dbo.VernacularConcept(
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

insert into [devserver02\sql2005].PlantName_Cache.dbo.ConceptApplication(
	ConceptApplicationId, FromConceptId, ToConceptId, Gender,
	PartOfTaxon, GeoRegion, GeographicSchema, InUse, [type])
select c.VernacularUseCounterPK,
	cr.VernacularArticleCounterPk FromConceptId,
	cr.BibliographyGuid ToConceptId, 
	--(Select top 1 conceptid from [devserver02\sql2005].PlantName_Cache.dbo.Concept con where
	--	con.ConceptId = cast(cr.bibliographyguid as varchar(50))) ToConceptId,
	null as gender,
	cr.VernacularUseAppliesTo,
	geo.GeoRegionName,
	geos.GeoRegionSchemaName,
	1,'is vernacular for'
	from #ConRels cr
	 inner join #Concept c on cr.VernacularUseCounterPk = c.VernacularUseCounterPk
	 inner join tblGeoRegion geo on cr.VernacularUseGeoregionfk = geo.GeoRegionCounterPK
	 inner join tblGeoRegionSchema geos on geo.GeoRegionSchemaFK = geos.GeoRegionSchemaCounterPK

 
 drop table #Vern
 drop table #Concept
 drop table #ConRels
 
 */