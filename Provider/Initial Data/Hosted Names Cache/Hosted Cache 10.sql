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

delete [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.Hosted_name.[Name]

INSERT INTO [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.Hosted_name.[Name](NameId, IsScientific,
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
	NomenclaturalCode,
	IsRecombination
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
		else na.NameNomCode end,
	na.NameIsRecombination
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
	set na.NomenclaturalStatus = Note + 
	case when DS.NameIllegitimate = 1 and charindex('nom. illeg.', Note) = 0 and charindex('nom. illegit.', Note) = 0 then 'nom. illeg.' else '' end +
	case when DS.NameInvalid = 1 and charindex('nom. inval.', Note) = 0 and charindex('nom. inv.', Note) = 0 then 'nom. inv.' else '' end +
	case when DS.NameNovum = 1 and charindex('nom. nov.', Note) = 0 then 'nom. nov.' else '' end	
from (select n.namefull, n.nameguid, n.nameillegitimate, n.nameinvalid, n.nameproparte, n.NameDubium, n.NameNovum,
	ISNULL((Select distinct nst.NomenclaturalStatusTypeText + ' ' as [text()] 
		from tblNomenclaturalStatus ns2
			inner join tblNomenclaturalStatusType nst 
				on ns2.NomenclaturalStatusStatusTypeFK = nst.NomenclaturalStatusTypeCounterPK
		where NomenclaturalStatusNameFk = n.NameGuid 
			and ns2.NomenclaturalStatusNameFk = n.nameguid
			and (ns2.NomenclaturalStatusIsDeleted is null or ns2.NomenclaturalStatusIsDeleted = 0)
		for XML path('')),'') as Note
from dbo.tblName n ) DS
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.hosted_name.Name na on DS.NameGuid = Na.NameId
where Note is not null

update n
set NomenclaturalStatus = null
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.hosted_name.name n
inner join tblName n2 on n2.NameGuid = n.NameId
where NomenclaturalStatus = ''



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
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.Hosted_name.[Name] n2 on n2.nameid = n.NameGuid


--standardise quality codes (title case)
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.[Name] 
set qualitycode = 'Validated Primary Source'
where qualitycode = 'Validated primary source'

update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.[Name] 
set qualitycode = 'Validated Secondary Source'
where qualitycode = 'Validated Secondary source'

-- For any names that have a suppressed parent, we need to include the parent names (only the basic info)
INSERT INTO [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.Hosted_name.[Name](NameId, IsScientific,
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
