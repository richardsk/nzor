IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HostedCache10]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[HostedCache10]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[HostedCache10]
as

set concat_null_yields_null off

delete [spidey\sql2005].name_cache.Hosted_name.[Name]

INSERT INTO [spidey\sql2005].name_cache.Hosted_name.[Name](NameId, IsScientific,
	createdDate, modifiedDate, Namefull, --NamePart,
	--NameHTML, 
	[rank], 
	CanonicalName, 
	--Genus,[done below]
	--InfragenericEpithet,
	--SpecificEpithet,[done below]
	--InfraspecificEpithet,[done below]
	--CultivarNameGroup,[done below]
	Authorship,
	BasionymAuthors,
	CombiningAuthors,
	PublishedIn,
	PublishedInID,
	[Year],
	MicroReference,
	TypeName,
	TypeNameID,
	ProtologueOrthography,
	Basionym,
	BasionymID,
	LaterHomonymOf,
	LaterHomonymOfId,
	BlockedName,
	BlockedNameId,
	RecombinedName,
	RecombinedNameId,
	--NomenclaturalStatus, [done below]
	NomenclaturalCode

)
select na.NameGuid, 1,
	na.NameAddedDate, na.NameUpdatedDate, na.NameFull, 
	--rtrim(replace(REPLACE(na.NameFull, na.NameAuthors, ''), '  ', ' ')),
	--replace(replace(replace(dbo.GetFullName(na.NameGUID, 1,0,1,0,0), '</i> <i>', ' '), '<i>', '&lt;em&gt;'), '</i>', '&lt;/em&gt;'),
	ta.TaxonRankName,
	na.NameCanonical,
	na.NameAuthors,	
	case when na.NameIsRecombination = 1 and na.NameAuthors <> '' then na.NameAuthors
		else null 
		end,
	case when na.NameAuthors <> '' then na.NameAuthors else null end,
	dbo.GetLiteratureCitation(ref.ReferenceId,'DAB35AA4-C7A6-49D1-BE20-23A76574162F' ,0),
	ref.ReferenceID	,
	na.NameYearOfPublication,
	na.NamePage,
	natype.NameFull as TypeTaxon,
	na.NameTypeTaxonFk,
	na.NameOrthographyVariant,
	bas.NameFull as Basionym,
	bas.NameGuid,
	case na.NameNovum
		when 1 then null
		when 0 then hom.NameFull
	end as homonymfor,
	case na.NameNovum
		when 1 then null
		when 0 then hom.nameguid
	end homonymId,
	case na.NameNovum
		when 1 then based.NameFull
		when 0 then null
	end BlockedName,
	case na.NameNovum
		when 1 then based.NameGUID
		when 0 then null
	end BlockedNameID,
	case na.NameNovum 
		when 1 then case when na.NameBasionymFk = na.NameGuid then null else bas.NameFull end
		when 0 then null
	end RecombinedName,
	case na.NameNovum 
		when 1 then case when na.NameBasionymFk = na.NameGuid then null else na.NameBasionymFk end
		when 0 then null
	end RecombinedNameId,	
	case when na.NameNomCode is null then 'ICZN'
		when na.NameNomCode = '' then 'ICZN'
		else na.NameNomCode end
	from dbo.tblName na
		inner join dbo.tblTaxonRank ta on na.NameTaxonRankFk = ta.TaxonRankPk
		left join dbo.tblReference ref on na.NameReferenceFk = ref.ReferenceID
		left join dbo.tblName natype on na.NameTypeTaxonFk = natype.nameguid
		left join dbo.tblName bas on na.NameBasionymFk = bas.NameGuid
		left join dbo.tblName hom on na.NameBlockingFk = hom.nameguid
		left join dbo.tblName based on na.NameBasedonFk = based.nameguid
		left join dbo.tblname pref on pref.nameguid = na.namecurrentfk
		left join dbo.tblFlatName fn on na.NameGuid = fn.FlatNameSeedName
			and fn.FlatNameRankName = 'kingdom'
where na.NameSuppress = 0 and isnull(pref.namesuppress,0) = 0 and isnull(na.namemisapplied,0) = 0
	and (fn.FlatNameCanonical is null or fn.FlatNameCanonical <> 'kingdomtemp')
	and na.namefull <> 'root'


update Na
	set na.NomenclaturalStatus = Note
from (select n.namefull, n.nameguid,
	(Select distinct nst.NomenclaturalStatusTypeText + ' ' as [text()] 
		from tblNomenclaturalStatus ns2
			inner join tblNomenclaturalStatusType nst 
				on ns2.NomenclaturalStatusStatusTypeFK = nst.NomenclaturalStatusTypeCounterPK
		where NomenclaturalStatusNameFk = n.NameGuid 
			and (NomenclaturalStatusStatusTypeFK = 5
				or ns2.NomenclaturalStatusStatusTypeFK = 10
				or ns2.NomenclaturalStatusStatusTypeFK = 4)
			and ns2.NomenclaturalStatusNameFk = n.nameguid
			and (ns2.NomenclaturalStatusIsDeleted is null or ns2.NomenclaturalStatusIsDeleted = 0)
		for XML path('')) as Note
from dbo.tblName n ) as DS
	inner join [spidey\sql2005].name_cache.Hosted_name.Name na on DS.NameGuid = Na.NameId
where Note is not null

update n
set NomenclaturalStatus = isnull(NomenclaturalStatus + ' ','') + 'unavailable'
from [spidey\sql2005].name_cache.Hosted_name.name n
inner join tblName n2 on n2.NameGuid = n.NameId
where n2.NameInvalid = 1

update n
set NomenclaturalStatus = isnull(NomenclaturalStatus + ' ','') + 'invalid'
from [spidey\sql2005].name_cache.Hosted_name.name n
inner join tblName n2 on n2.NameGuid = n.NameId
where n2.NameIllegitimate = 1



--add quality codes
update n2
set qualitycode = 
	case when (select COUNT(FieldStatusCounterPK) from tblFieldStatus where FieldStatusRecordFk = cast(n.NameGuid as varchar(38))) = 0 then null
		when (select count(distinct FieldStatusLevelFK) from tblFieldStatus where FieldStatusRecordFk = cast(n.NameGuid as varchar(38)) and FieldStatusLevelFK <> 4) = 1 then 
			(select top 1 fl.FieldStatusLevelText from tblFieldStatus fs inner join tblFieldStatusLevel fl on fl.FieldStatusLevelCounterPK = fs.FieldStatusLevelFK where FieldStatusRecordFk = cast(n.NameGuid as varchar(38)))
		when (select count(distinct FieldStatusLevelFK) from tblFieldStatus where FieldStatusRecordFk = cast(n.NameGuid as varchar(38)) and FieldStatusLevelFK < 3) = 1 then 
			'Validated Mixed Sources'
		else
			'Partially Validated'
		end
from tblName n
inner join [spidey\sql2005].name_cache.Hosted_name.[Name] n2 on n2.nameid = n.NameGuid


--standardise quality codes (title case)
update [spidey\sql2005].name_cache.plant_name.[Name] 
set qualitycode = 'Validated Primary Source'
where qualitycode = 'Validated primary source'

update [spidey\sql2005].name_cache.plant_name.[Name] 
set qualitycode = 'Validated Secondary Source'
where qualitycode = 'Validated Secondary source'

-- For any names that have a suppressed parent, we need to include the parent names (only the basic info)
INSERT INTO [spidey\sql2005].name_cache.Hosted_name.[Name](NameId, IsScientific,
	createdDate, 
	modifiedDate, 
	NameFull, 	
	[rank], 
	CanonicalName, 
	QualityCode,
	NomenclaturalCode)
select distinct 
	pn.NameGuid, 1,
	pn.NameAddedDate, 
	pn.NameUpdatedDate, 
	pn.NamePart, 
	ta.TaxonRankName,
	pn.NameCanonical,
	'Unvalidated',
	case when pn.NameNomCode is null then 'ICZN'
		when pn.NameNomCode = '' then 'ICZN'
		else pn.NameNomCode end
from dbo.tblName na
	inner join tblFlatName fn on fn.FlatNameSeedName = na.NameGuid
	inner join tblName pn on pn.NameGuid = fn.FlatNameNameUFk
	inner join dbo.tblTaxonRank ta on pn.NameTaxonRankFk = ta.TaxonRankPk
	inner join dbo.tblFlatName kfn on na.NameGuid = kfn.FlatNameSeedName
			and kfn.FlatNameRankName = 'kingdom'
where (pn.NameSuppress = 1 or pn.NameMisapplied = 1) and na.NameSuppress = 0 and isnull(na.NameMisapplied,0) = 0
	and (kfn.FlatNameCanonical is null or kfn.FlatNameCanonical <> 'kingdomtemp') 
	
	

go

grant execute on dbo.[HostedCache10] to dbi_user

go
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HostedCache15]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[HostedCache15]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[HostedCache15]
as

set concat_null_yields_null off

delete [spidey\sql2005].name_cache.Hosted_name.TaxonConcept
delete [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse


insert into [spidey\sql2005].name_cache.Hosted_name.TaxonConcept(taxonconceptid,
	CreatedDate, ModifiedDate, Name, NameId, AccordingTo, AccordingToId, [Rank])
select bib.BibliographyGuid, BibliographyAddedDate, BibliographyUpdatedDate,
	bib.BibliographyName, bib.BibliographyNameFk, 
	ref.ReferenceGenCitation,
	bib.BibliographyReferenceFk, null
	from tblBibliography bib
	inner join [spidey\sql2005].name_cache.Hosted_name.Name n on n.nameid = bib.BibliographyNameFk
		left join tblReference ref on bib.BibliographyReferenceFk = ref.ReferenceID
where (bib.BibliographyIsDeleted = 0 or bib.BibliographyIsDeleted is null)


update con
	set AcceptedConceptId  = bibr.BibliographyRelationshipBibliographyToFk,
	AcceptedConceptInUse = 
		case when (select count(BibliographyRelationshipPk)  
			from tblBibliographyRelationship pbr
			inner join tblBibliography pb on pb.BibliographyGuid = pbr.BibliographyRelationshipBibliographyFromFk 
			where pb.BibliographyNameFk = b.BibliographyNameFk and pbr.BibliographyRelationshipTypeFk = 8
				and isnull(pb.BibliographyIsDeleted,0) = 0 and pbr.BibliographyRelationshipIsActive = 1) = 1
			then 
				case when (select top 1 BibliographyGuid from tblBibliographyRelationship pbr
							inner join tblBibliography pb on pb.BibliographyGuid = pbr.BibliographyRelationshipBibliographyFromFk
							where pb.BibliographyNameFk = b.BibliographyNameFk and pbr.BibliographyRelationshipTypeFk = 8
							and isnull(pb.BibliographyIsDeleted,0) = 0 and pbr.BibliographyRelationshipIsActive = 1) = con.taxonconceptid
				then 
					1
				else
					null
				end 
		else
			null
		end
from [spidey\sql2005].name_cache.Hosted_name.TaxonConcept con
	inner join tblBibliographyRelationship bibr on con.taxonconceptid = bibr.BibliographyRelationshipBibliographyFromFk
		and bibr.BibliographyRelationshipTypeFk = 8
	inner join tblbibliography b on b.BibliographyGuid = bibr.BibliographyRelationshipBibliographyFromFk
	inner join [spidey\sql2005].name_cache.Hosted_name.TaxonConcept cto on cto.taxonconceptid = bibr.BibliographyRelationshipBibliographyToFk
		
update con
	set ParentconceptId  = bibr.BibliographyRelationshipBibliographyToFk,
	ParentConceptInUse = 
		case when (select count(BibliographyRelationshipPk)  --only one parent concept for this name
			from tblBibliographyRelationship pbr
			inner join tblBibliography pb on pb.BibliographyGuid = pbr.BibliographyRelationshipBibliographyFromFk 
			where pb.BibliographyNameFk = b.BibliographyNameFk and pbr.BibliographyRelationshipTypeFk = 7 
				and isnull(pb.BibliographyIsDeleted,0) = 0 and pbr.BibliographyRelationshipIsActive = 1) = 1			
			then 
				case when (select top 1 BibliographyGuid from tblBibliographyRelationship pbr
							inner join tblBibliography pb on pb.BibliographyGuid = pbr.BibliographyRelationshipBibliographyFromFk
							where pb.BibliographyNameFk = b.BibliographyNameFk and pbr.BibliographyRelationshipTypeFk = 7 
							and isnull(pb.BibliographyIsDeleted,0) = 0 and pbr.BibliographyRelationshipIsActive = 1) = con.taxonconceptid
				then 
					1
				else
					null
				end 
		else
			null
		end		
from [spidey\sql2005].name_cache.Hosted_name.TaxonConcept con
	inner join tblBibliographyRelationship bibr on con.Taxonconceptid = bibr.BibliographyRelationshipBibliographyFromFk
		and bibr.BibliographyRelationshipTypeFk = 7
	inner join tblbibliography b on b.BibliographyGuid = bibr.BibliographyRelationshipBibliographyFromFk
	inner join [spidey\sql2005].name_cache.Hosted_name.TaxonConcept cto on cto.taxonconceptid = bibr.BibliographyRelationshipBibliographyToFk

--add missing concepts as name based concepts

declare @accToId uniqueidentifier
if (not exists(select publicationid from [spidey\sql2005].name_cache.Hosted_name.publication where citation = 'Hosted'))
begin
	set @accToId = NEWID()
	insert [spidey\sql2005].name_cache.Hosted_name.publication(PublicationID, Citation) select @accToId, 'Hosted'
	insert [spidey\sql2005].name_cache.Hosted_name.publicationTitle(PublicationID, Title, Level, Type) select @accToId, 'Hosted', 1, 'full'
end
else
begin
	select @accToId = publicationid from [spidey\sql2005].name_cache.Hosted_name.publication where citation = 'Hosted'
end

insert [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse 
	(TaxonNameUseid, CreatedDate, ModifiedDate, NameId, AccordingToId, AcceptedNameId, ParentNameId)
select newid(), n.nameaddeddate, n.nameupdateddate, n.NameGuid, @accToId, NameCurrentFk, NameParentFk
from tblName n
inner join [spidey\sql2005].name_cache.Hosted_name.Name n1 on n1.nameid = n.nameguid
--left join [spidey\sql2005].name_cache.Hosted_name.TaxonConcept con on con.nameid = n.NameGuid 
left join [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse nbc on nbc.nameid = n.NameGuid 
where nbc.TaxonNameUseid is null 
	and NameParentFk <> '2F5CA98D-EE05-461D-901A-419A88071133'
	and not exists(select taxonconceptid from [spidey\sql2005].name_cache.Hosted_name.taxonconcept 
					where nameid = n1.nameid and parentconceptid is not null and parentconceptinuse = 1)
		
--null out accepted concepts if they have been provided by another 
update nbc
set acceptednameid = null
from [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse nbc
inner join [spidey\sql2005].name_cache.Hosted_name.taxonconcept tc on tc.nameid = nbc.nameid 
	and acceptedconceptid is not null and acceptedconceptinuse = 1
	

/*  other relationships..

insert into [spidey\sql2005].name_cache.Hosted_name.ConceptRelationship(
	ConceptRelationshipID, FromConceptId, ToConceptId, [Type], InUse)
select BibliographyRelationshipPk, BibliographyRelationshipBibliographyFromFk,
	BibliographyRelationshipBibliographyToFk, 
	Case BibliographyRelationshipTypeFk
		when 
	end,
	BibliographyRelationshipIsActive
	from tblBibliographyRelationship
	where BibliographyRelationshipTypeFk <> 8
	 and BibliographyRelationshipTypeFk <> 7
	 and BibliographyRelationshipTypeFk <> 0
*/

-- to do HigherClassification
-- to do Hybrid concept relationships


go

grant execute on dbo.[HostedCache15] to dbi_user

go
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HostedCache20]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[HostedCache20]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[HostedCache20]
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

insert into [spidey\sql2005].name_cache.Hosted_name.Vernacular(
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



insert into [spidey\sql2005].name_cache.Hosted_name.VernacularConcept(
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

insert into [spidey\sql2005].name_cache.Hosted_name.ConceptApplication(
	ConceptApplicationId, FromConceptId, ToConceptId, Gender,
	PartOfTaxon, GeoRegion, GeographicSchema, InUse, [type])
select c.VernacularUseCounterPK,
	cr.VernacularArticleCounterPk FromConceptId,
	cr.BibliographyGuid ToConceptId, 
	--(Select top 1 conceptid from [spidey\sql2005].name_cache.Hosted_name..Concept con where
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
insert into [spidey\sql2005].name_cache.Hosted_name.Vernacular(
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
	inner join [spidey\sql2005].name_cache.Hosted_name.name n on n.nameid = vu.vernacularusenamefk
	left join tblLanguage la on vn.VernacularLanguageFk = la.LanguageCounterPK
	left join tblGeoRegion g on vn.VernacularGeoRegionFk = g.GeoRegionCounterPK
	left join #Conrels cr on vn.VernacularGuid = cr.VernacularUseVernacularFK
where isnull(vn.vernacularSuppress,0) = 0 and cr.vernacularusecounterpk is null

declare @accToId uniqueidentifier
if (not exists(select publicationid from [spidey\sql2005].name_cache.Hosted_name.publication where citation = 'Hosted'))
begin
	set @accToId = NEWID()
	insert [spidey\sql2005].name_cache.Hosted_name.publication(PublicationID, Citation) select @accToId, 'Hosted'
	insert [spidey\sql2005].name_cache.Hosted_name.publicationTitle(PublicationID, Title, Level, Type) select @accToId, 'Hosted', 1, 'full'
end
else
begin
	select @accToId = publicationid from [spidey\sql2005].name_cache.Hosted_name.publication where citation = 'Hosted'
end

insert into [spidey\sql2005].name_cache.Hosted_name.vernacularuse
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
inner join [spidey\sql2005].name_cache.Hosted_name.name n on n.nameid = vu.vernacularusenamefk
inner join [spidey\sql2005].name_cache.Hosted_name.vernacular v on v.vernacularid = vu.VernacularUseVernacularFk
left join #Conrels cr on vn.VernacularGuid = cr.VernacularUseVernacularFK
left join tblVernacularArticle va on vu.VernacularUseCounterPk = va.VernacularArticleVernacularUseFK		
where isnull(vu.VernacularUseSuppress,0) = 0 and isnull(vu.VernacularUseIsDeleted,0) = 0
	and cr.vernacularusecounterpk is null

 
 drop table #Vern
 drop table #Concept
 drop table #ConRels
 
go

grant execute on dbo.[HostedCache20] to dbi_user

go
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HostedCache30]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[HostedCache30]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[HostedCache30]
as

delete [spidey\sql2005].name_cache.Hosted_name.Biostatus

insert into [spidey\sql2005].name_cache.Hosted_name.Biostatus(
	BiostatusId, CreatedDate, ModifiedDate, Taxon,
	ConceptId, NameId, AccordingTo, AccordingToId, Region,
	GeographicSchema, Biome, EnvironmentalContext, Origin, Occurrence)
select 
	nb.NameBiostatusCounterPK,
	nb.NameBiostatusAddedDate,
	nb.NameBiostatusUpdatedDate,
	Na.NameFull,
	tc.taxonconceptid,
	case when tc.taxonconceptid is null then nb.NameBiostatusNameFk else null end,
	ref.ReferenceGenCitation,
	case when tc.taxonconceptid is null then ref.ReferenceID else null end,
	geo.GeoRegionName,
	geos.GeoRegionSchemaName,
	null as Biome,
	case occ.OccurrenceDescription
		when 'Present in captivity/cultivation/culture' then 'Cultivation/Captivity'
		else 'Wild'
	end as EnviromentContext,
	case b.BioStatusDescription 
		when 'Uncertain' then 'Unknown'
		else b.BioStatusDescription 
	end,
	case occ.OccurrenceDescription
		--when 'Extinct' then 'Extinct'
		--when 'Present' then 'Present'
		--when 'Sometimes present' then 'Sometimes present'
		when 'Wild' then 'Present'
		when 'Uncertain' then 'Unknown'
		when 'Border intercept' then 'Border Intercept'  --captialisation
		when 'Sometimes present' then 'Sometimes Present' --captialisation
		--when 'Recorded in error' then 'Recorded in error'
		when 'Present in captivity/cultivation/culture' then 'Present'
		when 'Eradicated' then 'Eradicated/Destroyed'
		else occ.OccurrenceDescription
	end
	 from tblNameBiostatus nb
	inner join tblname n on n.nameguid = nb.namebiostatusnamefk
	 inner join [spidey\sql2005].name_cache.Hosted_name.Name na on nb.NameBiostatusNameFk
		= Na.NameId
	left join tblBioStatus b on nb.NameBiostatusBiostatusFk = b.BiostatusCodePK
	left join tblOccurrence occ on nb.NameBiostatusOccurrenceFK = occ.OccurrenceCodePK
	left join tblReference ref on nb.NameBiostatusReferenceFk = ref.ReferenceID
	left join tblGeoRegion geo on nb.NameBiostatusGeoRegionFK = geo.GeoRegionCounterPK
	left join tblGeoRegionSchema geos on geo.GeoRegionSchemaFK = geos.GeoRegionSchemaCounterPK
	left join [spidey\sql2005].Name_Cache.Hosted_name.taxonconcept tc on tc.nameid = n.nameguid and tc.accordingtoid = nb.namebiostatusreferencefk
where (nb.NameBiostatusIsDeleted = 0 or nb.NameBiostatusIsDeleted is null)
	and (isnull(n.namecurrentfk, n.nameguid) = n.nameguid or tc.taxonconceptid is not null)
	
go

grant execute on dbo.[HostedCache30] to dbi_user

go
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HostedCache40]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[HostedCache40]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[HostedCache40]
as

delete from [spidey\sql2005].name_cache.Hosted_name.Publication

select distinct	
	PublishedInId 
into #Refs
from [spidey\sql2005].name_cache.Hosted_name.Name
	where PublishedInId is not null
union
select distinct	
	PublishedInId 
from [spidey\sql2005].name_cache.Hosted_name.Vernacular
	where PublishedInId is not null
union
select distinct
	AccordingToId
from [spidey\sql2005].name_cache.Hosted_name.taxonconcept
	where AccordingToId is not null
union	
select distinct
	AccordingToId
from [spidey\sql2005].name_cache.Hosted_name.vernacularconcept
	where AccordingToId is not null
union
select distinct
	AccordingToId
from [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse
	where AccordingToId is not null
union
select distinct
	AccordingToId
from [spidey\sql2005].name_cache.Hosted_name.Biostatus
	where AccordingToId is not null


insert into #Refs(PublishedInId)
select distinct r.ReferenceParentID 
	from #Refs rl
	inner join tblReference r on rl.PublishedInId = r.ReferenceID
	left join #Refs rp on r.ReferenceParentID = rp.PublishedInId
where rp.PublishedInId is null
	and r.ReferenceParentID is not null
	and r.ReferenceParentID <> '00000000-0000-0000-0000-000000000000'



insert into #Refs(PublishedInId)
select distinct r.ReferenceParentID 
	from #Refs rl
	inner join tblReference r on rl.PublishedInId = r.ReferenceID
	left join #Refs rp on r.ReferenceParentID = rp.PublishedInId
where rp.PublishedInId is null
	and r.ReferenceParentID is not null
	and r.ReferenceParentID <> '00000000-0000-0000-0000-000000000000'


	
insert into [spidey\sql2005].name_cache.Hosted_name.Publication(
		PublicationID, ParentPublicationId, [Type],
		CreatedDate, ModifiedDate, Citation)	
select 
	ref.ReferenceID,
	ref.ReferenceParentID,
	rt.ReferenceTypeText,
	ref.ReferenceAddedDate,
	ref.ReferenceUpdatedDate ,
	ref.ReferenceGenCitation
	from #Refs rl 
		inner join tblReference ref on rl.PublishedInId = ref.ReferenceID
		inner join tblReferenceType rt on ref.ReferenceTypeID = rt.ReferenceTypeID
	where (ref.ReferenceIsDeleted is null or ref.ReferenceIsDeleted = 0)


update pub
	set AuthorsSimple = rf.ReferenceFieldText
from  [spidey\sql2005].name_cache.Hosted_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Author, primary'


update pub
	set DateOfPublication = rf.ReferenceFieldText
from  [spidey\sql2005].name_cache.Hosted_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Date, primary'


update pub
	set DateOnPublication = rf.ReferenceFieldText
from  [spidey\sql2005].name_cache.Hosted_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Date, secondary'


update pub
	set Volume = rf.ReferenceFieldText
from  [spidey\sql2005].name_cache.Hosted_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Volume'


update pub
	set Issue = rf.ReferenceFieldText
from  [spidey\sql2005].name_cache.Hosted_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Issue'


update pub
	set PageStart = rf.ReferenceFieldText
from  [spidey\sql2005].name_cache.Hosted_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Start page'


update pub
	set pageEnd = rf.ReferenceFieldText
from  [spidey\sql2005].name_cache.Hosted_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'End page'


update pub
	set PublisherName = rf.ReferenceFieldText
from  [spidey\sql2005].name_cache.Hosted_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Publisher'


update pub
	set PublisherCity = rf.ReferenceFieldText
from  [spidey\sql2005].name_cache.Hosted_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Pub place'


insert into [spidey\sql2005].name_cache.Hosted_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	1,
	'full'
	from
	[spidey\sql2005].name_cache.Hosted_name.Publication pub
	inner join  tblReferenceField rf on Pub.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'



insert into [spidey\sql2005].name_cache.Hosted_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	1,
	'abbreviation'
	from
	[spidey\sql2005].name_cache.Hosted_name.Publication pub
	inner join  tblReferenceField rf on Pub.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Abbreviation'


insert into [spidey\sql2005].name_cache.Hosted_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	2,
	'full'
	from
	[spidey\sql2005].name_cache.Hosted_name.Publication pub
	inner join [spidey\sql2005].name_cache.Hosted_name.Publication pub2
		on Pub.ParentPublicationId = pub2.publicationid
	inner join  tblReferenceField rf on Pub2.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'



insert into [spidey\sql2005].name_cache.Hosted_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	3,
	'full'
	from
	[spidey\sql2005].name_cache.Hosted_name.Publication pub
	inner join [spidey\sql2005].name_cache.Hosted_name.Publication pub2
		on Pub.ParentPublicationId = pub2.publicationid
	inner join [spidey\sql2005].name_cache.Hosted_name.Publication pub3
		on Pub2.ParentPublicationId = pub3.publicationid
	inner join  tblReferenceField rf on Pub3.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'


--standardise
update [spidey\sql2005].name_cache.Hosted_name.publication set [type] = 'Article' where [type] = 'Article in journal'
update [spidey\sql2005].name_cache.Hosted_name.publication set [type] = 'Article' where [type] = 'Article in magazine'
update [spidey\sql2005].name_cache.Hosted_name.publication set [type] = 'Serial' where [type] = 'Serial (book/monograph) in series'
update [spidey\sql2005].name_cache.Hosted_name.publication set [type] = 'Book' where [type] = 'Book (in series)'
update [spidey\sql2005].name_cache.Hosted_name.publication set [type] = 'Chapter' where [type] = 'Chapter in book (in series)'
update [spidey\sql2005].name_cache.Hosted_name.publication set [type] = 'Electronic source' where [type] = 'Electronic citation in electronic source'
update [spidey\sql2005].name_cache.Hosted_name.publication set [type] = 'Unpublished' where [type] = 'Unpublished work (in series)'



drop table #refs


go

grant execute on dbo.[HostedCache40] to dbi_user

go
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HostedCache6]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[HostedCache6]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[HostedCache6]
as
--test transfer is ok

set concat_null_yields_null off

--test there are no names where the parent does not exist
if (exists(select * from [spidey\sql2005].name_cache.Hosted_name.name n 
			inner join [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse nbc on nbc.nameid = n.nameid
			left join [spidey\sql2005].name_cache.Hosted_name.name n2 on n2.nameid = nbc.parentnameid
			where nbc.parentnameid is not null and n2.nameid is null and n.rank <> 'kingdom'))
begin
	declare @pnames nvarchar(max), @pmsg nvarchar(max)
	set @pnames = char(13) 
	select @pnames = @pnames + n.NameFull + char(13) from [spidey\sql2005].name_cache.Hosted_name.Name n
		inner join [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse nbc on nbc.nameid = n.nameid
		left join [spidey\sql2005].name_cache.Hosted_name.name n2 on n2.nameid = nbc.parentnameid
		where nbc.parentnameid is not null and n2.nameid is null and n.rank <> 'kingdom'
	set @pmsg = 'HostedName_Cache ERROR : There are names that exist where the parent name is absent.  ' + @pnames
	print(@pmsg)
	
	--FOR NOW delete these names
	--TODO - remove these and add raiserror back in
	declare @done bit
	set @done = 0
	while (@done = 0) 
	begin
		delete n
		from [spidey\sql2005].name_cache.Hosted_name.Name n
			inner join [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse nbc on nbc.nameid = n.nameid
			left join [spidey\sql2005].name_cache.Hosted_name.name n2 on n2.nameid = nbc.parentnameid
			where nbc.parentnameid is not null and n2.nameid is null and n.rank <> 'kingdom'
		
		if (@@ROWCOUNT = 0) set @done = 1
	end 
		
	delete nbc
	from [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse nbc
	left join [spidey\sql2005].name_cache.Hosted_name.Name n on n.nameid = nbc.nameid
	where n.nameid is null
		
	delete nbc
	from [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse nbc
	left join [spidey\sql2005].name_cache.Hosted_name.Name n on n.nameid = nbc.parentnameid
	where n.nameid is null and nbc.parentnameid is not null
	
	delete nbc
	from [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse nbc
	left join [spidey\sql2005].name_cache.Hosted_name.Name n on n.nameid = nbc.acceptednameid
	where n.nameid is null and nbc.acceptednameid is not null
	
	--raiserror(@pmsg, 1, 1)
end

--same for taxonconcepts
if (exists(select * from [spidey\sql2005].name_cache.Hosted_name.name n 
			inner join [spidey\sql2005].name_cache.Hosted_name.taxonconcept tc on tc.nameid = n.nameid
			left join [spidey\sql2005].name_cache.Hosted_name.taxonconcept ptc on ptc.taxonconceptid = tc.parentconceptid
			where tc.parentconceptid is not null and ptc.taxonconceptid is null and n.rank <> 'kingdom'))
begin	
	set @done = 0
	while (@done = 0) 
	begin
		delete n
		from [spidey\sql2005].name_cache.Hosted_name.Name n
			inner join [spidey\sql2005].name_cache.Hosted_name.taxonconcept tc on tc.nameid = n.nameid
			left join [spidey\sql2005].name_cache.Hosted_name.taxonconcept ptc on ptc.taxonconceptid = tc.parentconceptid
			where tc.parentconceptid is not null and ptc.taxonconceptid is null and n.rank <> 'kingdom'
		
		if (@@ROWCOUNT = 0) set @done = 1
	end 

	delete tc
	from [spidey\sql2005].name_cache.Hosted_name.taxonconcept tc
	left join [spidey\sql2005].name_cache.Hosted_name.Name n on n.nameid = tc.nameid
	where n.nameid is null 
	
	
	--raiserror(@pmsg, 1, 1)
end

delete tc
from [spidey\sql2005].name_cache.Hosted_name.taxonconcept tc
left join [spidey\sql2005].name_cache.Hosted_name.name n on n.nameid = tc.nameid
where n.nameid is null
		
		
--test there are no names where the preferred name does not exist
if (exists(select * from [spidey\sql2005].name_cache.Hosted_name.name n 
			inner join [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse nbc on nbc.nameid = n.nameid
			left join [spidey\sql2005].name_cache.Hosted_name.name n2 on n2.nameid = nbc.acceptednameid
			where nbc.acceptednameid is not null and n2.nameid is null))
begin
	declare @names nvarchar(max), @msg nvarchar(max)
	set @names = char(13) + CHAR(10)
	select @names = @names + n.NameFull + char(13) + CHAR(10) from [spidey\sql2005].name_cache.Hosted_name.Name n
		inner join [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse nbc on nbc.nameid = n.nameid
		left join [spidey\sql2005].name_cache.Hosted_name.name n2 on n2.nameid = nbc.acceptednameid
		where nbc.acceptednameid is not null and n2.nameid is null
	set @msg = 'HostedName_Cache ERROR : There are names that exist where the accepted name is absent.  ' + @names
	print @msg
	
	--FOR NOW set accepted name to NULL
	--TODO - remove these and add raiserror back in
	update nbc set nbc.AcceptedNameId = null
	from [spidey\sql2005].name_cache.Hosted_name.Name n
		inner join [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse nbc on nbc.nameid = n.nameid
		left join [spidey\sql2005].name_cache.Hosted_name.name n2 on n2.nameid = nbc.acceptednameid
		where nbc.acceptednameid is not null and n2.nameid is null 
		
	update tc set tc.AcceptedconceptId = null
	from [spidey\sql2005].name_cache.Hosted_name.taxonconcept tc
		left join [spidey\sql2005].name_cache.Hosted_name.taxonconcept ptc on ptc.taxonconceptid = tc.acceptedconceptid
		where tc.acceptedconceptid is not null and ptc.taxonconceptid is null 
		
	--raiserror(@msg, 1, 1)
end
		
	
if (exists(
		select distinct c.parentconceptid from [spidey\sql2005].name_cache.Hosted_name.taxonconcept c 
		left join [spidey\sql2005].name_cache.Hosted_name.taxonconcept c2 on c2.taxonconceptid = c.parentconceptid
		left join [spidey\sql2005].name_cache.Hosted_name.TaxonNameUse c3 on c3.TaxonNameUseid = c.parentconceptid
		inner join tblbibliography b on b.bibliographyguid = c.parentconceptid
		where c2.taxonconceptid is null and c3.TaxonNameUseid is null and c.parentconceptid is not null))
begin
	raiserror('TaxonConcepts exist where the ParentConceptId is neither in the TaxonConcept table or the TaxonNameUse table',
		10, 1)
end

go

grant execute on dbo.[HostedCache6] to dbi_user

go
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferToHostedNameCache]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TransferToHostedNameCache]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TransferToHostedNameCache]
as

exec [spidey\sql2005].name_cache.dbo.HostedCache0
exec dbo.HostedCache10
exec dbo.HostedCache15
exec dbo.HostedCache20
exec dbo.HostedCache30
exec dbo.HostedCache40
exec [spidey\sql2005].name_cache.dbo.HostedCache5
exec [spidey\sql2005].name_cache.dbo.HostedCache50
exec dbo.HostedCache6


go

grant execute on dbo.[TransferToHostedNameCache] to dbi_user

go
