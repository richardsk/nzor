IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestCache10]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TestCache10]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TestCache10]
as

set concat_null_yields_null off

------------------------------------------
--Plant names in Parmeliaceae
INSERT INTO [devserver02\sql2005].Name_cache_Test.test_name.[Name](NameId, IsScientific,
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
	--BasionymAuthors,[done below]
	--CombiningAuthors,[done below]
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
	na.NameNomCode,
	na.NameIsRecombination
	from dbo.tblName na
		inner join dbo.tblTaxonRank ta on na.NameTaxonRankFk = ta.TaxonRankPk
		left join dbo.tblReference ref on na.NameReferenceFk = ref.ReferenceID
		left join dbo.tblName natype on na.NameTypeTaxonFk = natype.nameguid
		left join dbo.tblName bas on na.NameBasionymFk = bas.NameGuid
		left join dbo.tblName hom on na.NameBlockingFk = hom.nameguid
		left join dbo.tblName based on na.NameBasedonFk = based.nameguid
		left join dbo.tblname pref on pref.nameguid = na.namecurrentfk
		inner join dbo.tblFlatName fn on na.NameGuid = fn.FlatNameSeedName
where na.NameSuppress = 0 and pref.namesuppress = 0
	and fn.FlatNameNameUFk = '3CB3ABBE-4EC5-4653-8785-8F4171B05D1C' --Parmeliaceae



INSERT INTO [devserver02\sql2005].Name_cache_Test.test_name.[Name](NameId, IsScientific,
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
	--BasionymAuthors,[done below]
	--CombiningAuthors,[done below]
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
	na.NameNomCode,
	na.NameIsRecombination
	from dbo.tblName na
		inner join dbo.tblTaxonRank ta on na.NameTaxonRankFk = ta.TaxonRankPk
		left join dbo.tblReference ref on na.NameReferenceFk = ref.ReferenceID
		left join dbo.tblName natype on na.NameTypeTaxonFk = natype.nameguid
		left join dbo.tblName bas on na.NameBasionymFk = bas.NameGuid
		left join dbo.tblName hom on na.NameBlockingFk = hom.nameguid
		left join dbo.tblName based on na.NameBasedonFk = based.nameguid
		left join dbo.tblname pref on pref.nameguid = na.namecurrentfk
		inner join dbo.tblFlatName fn on na.NameGuid = fn.FlatNameNameUFk
where na.NameSuppress = 0 and pref.namesuppress = 0
	and fn.FlatNameSeedName = '3CB3ABBE-4EC5-4653-8785-8F4171B05D1C' --hierarchy above Parmeliaceae
	and na.NameFull <> 'root' and na.NameFull <> 'biotic' and fn.FlatNameNameDepth <> 0


-------------------------------------------
--Fungi names in Parmeliaceae
INSERT INTO [devserver02\sql2005].Name_cache_Test.test_name_2.[Name](NameId, IsScientific,
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
	--BasionymAuthors,[done below]
	--CombiningAuthors,[done below]
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
	na.NameNomCode,
	na.NameIsRecombination
	from funginamesfromprod.dbo.tblName na
		inner join funginamesfromprod.dbo.tblTaxonRank ta on na.NameTaxonRankFk = ta.TaxonRankPk
		left join funginamesfromprod.dbo.tblReference ref on na.NameReferenceFk = ref.ReferenceID
		left join funginamesfromprod.dbo.tblName natype on na.NameTypeTaxonFk = natype.nameguid
		left join funginamesfromprod.dbo.tblName bas on na.NameBasionymFk = bas.NameGuid
		left join funginamesfromprod.dbo.tblName hom on na.NameBlockingFk = hom.nameguid
		left join funginamesfromprod.dbo.tblName based on na.NameBasedonFk = based.nameguid
		left join funginamesfromprod.dbo.tblname pref on pref.nameguid = na.namecurrentfk
		inner join funginamesfromprod.dbo.tblFlatName fn on na.NameGuid = fn.FlatNameSeedName
where na.NameSuppress = 0 and pref.namesuppress = 0
	and fn.FlatNameNameUFk = '1CB1CD87-36B9-11D5-9548-00D0592D548C' --Parmeliaceae



INSERT INTO [devserver02\sql2005].Name_cache_Test.test_name_2.[Name](NameId, IsScientific,
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
	--BasionymAuthors,[done below]
	--CombiningAuthors,[done below]
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
	na.NameNomCode
	from funginamesfromprod.dbo.tblName na
		inner join funginamesfromprod.dbo.tblTaxonRank ta on na.NameTaxonRankFk = ta.TaxonRankPk
		left join funginamesfromprod.dbo.tblReference ref on na.NameReferenceFk = ref.ReferenceID
		left join funginamesfromprod.dbo.tblName natype on na.NameTypeTaxonFk = natype.nameguid
		left join funginamesfromprod.dbo.tblName bas on na.NameBasionymFk = bas.NameGuid
		left join funginamesfromprod.dbo.tblName hom on na.NameBlockingFk = hom.nameguid
		left join funginamesfromprod.dbo.tblName based on na.NameBasedonFk = based.nameguid
		left join funginamesfromprod.dbo.tblname pref on pref.nameguid = na.namecurrentfk
		inner join funginamesfromprod.dbo.tblFlatName fn on na.NameGuid = fn.FlatNameNameUFk
where na.NameSuppress = 0 and pref.namesuppress = 0
	and fn.FlatNameSeedName = '1CB1CD87-36B9-11D5-9548-00D0592D548C'  --hierarchy above Parmeliaceae
	and na.NameFull <> 'root' and na.NameFull <> 'biotic' and fn.FlatNameNameDepth <> 0


----------------------------------
--Other Plant name details
update na	
	set 
		BasionymAuthors = case CHARINDEX(')', authorship)
		when 0 then Authorship
		else replace(LEFT(authorship, charindex(')', authorship) - 1), '(', '')
	end,
	 CombiningAuthors = (case CHARINDEX(')', authorship)
		when 0 then null
		else ltrim(RIGHT(authorship, len(authorship) - charindex(')', authorship)))
	end)
from [devserver02\sql2005].Name_cache_Test.test_name.Name na
	where Authorship is not null and Authorship <> ''


update Na
	set na.NomenclaturalStatus = Note + 
	case when DS.NameIllegitimate = 1 and charindex('nom. illeg.', Note) = 0 and charindex('nom. illegit.', Note) = 0 then 'nom. illeg.' else '' end +
	case when DS.NameInvalid = 1 and charindex('nom. inval.', Note) = 0 and charindex('nom. inv.', Note) = 0 then 'nom. inv.' else '' end +
	case when DS.NameProParte = 1 and charindex('pro. parte', Note) = 0 then 'pro. parte' else '' end +
	case when DS.NameDubium = 1 and charindex('nom. dub.', Note) = 0 and charindex('nomen dubium', Note) = 0 then 'nomen dubium' else '' end +	
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
	inner join [spidey\sql2005].name_cache_test.test_name.Name na on DS.NameGuid = Na.NameId
where Note is not null

update n
set NomenclaturalStatus = null
from [spidey\sql2005].name_cache_test.test_name.name n
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
inner join [devserver02\sql2005].Name_cache_Test.test_name.[Name] n2 on n2.nameid = n.NameGuid

--standardise quality codes (title case)
update [devserver02\sql2005].Name_cache_Test.test_name.[Name] 
set qualitycode = 'Validated Primary Source'
where qualitycode = 'Validated primary source'

update [devserver02\sql2005].Name_cache_Test.test_name.[Name] 
set qualitycode = 'Validated Secondary Source'
where qualitycode = 'Validated Secondary source'


-- For any names that have a suppressed parent, we need to include the parent names (only the basic info)
INSERT INTO [devserver02\sql2005].Name_cache_Test.test_name.[Name](NameId, IsScientific,
	createdDate, 
	modifiedDate, 
	NameFull, 	
	[rank], 
	CanonicalName, 
	QualityCode)
select distinct 
	pn.NameGuid, 1,
	pn.NameAddedDate, 
	pn.NameUpdatedDate, 
	pn.NamePart, 
	ta.TaxonRankName,
	pn.NameCanonical,
	'Unvalidated'
from tblName na
	inner join tblFlatName fn on fn.FlatNameSeedName = na.NameGuid
	inner join tblName pn on pn.NameGuid = fn.FlatNameNameUFk
	inner join tblTaxonRank ta on pn.NameTaxonRankFk = ta.TaxonRankPk
	inner join tblFlatName kfn on na.NameGuid = kfn.FlatNameSeedName
			and kfn.FlatNameRankName = 'kingdom'
where pn.NameSuppress = 1 and na.NameSuppress = 0
	and (kfn.FlatNameCanonical is null or kfn.FlatNameCanonical <> 'kingdomtemp') 
	
	

--------------------------------------------------
--Other Fungi name details
update na	
	set 
		BasionymAuthors = case CHARINDEX(')', authorship)
		when 0 then Authorship
		else replace(LEFT(authorship, charindex(')', authorship) - 1), '(', '')
	end,
	 CombiningAuthors = (case CHARINDEX(')', authorship)
		when 0 then null
		else ltrim(RIGHT(authorship, len(authorship) - charindex(')', authorship)))
	end)
from [devserver02\sql2005].Name_cache_Test.test_name_2.Name na
	where Authorship is not null and Authorship <> ''


update Na
	set na.NomenclaturalStatus = Note + 
	case when DS.NameIllegitimate = 1 and charindex('nom. illeg.', Note) = 0 and charindex('nom. illegit.', Note) = 0 then 'nom. illeg.' else '' end +
	case when DS.NameInvalid = 1 and charindex('nom. inval.', Note) = 0 and charindex('nom. inv.', Note) = 0 then 'nom. inv.' else '' end +
	case when DS.NameProParte = 1 and charindex('pro. parte', Note) = 0 then 'pro. parte' else '' end +
	case when DS.NameDubium = 1 and charindex('nom. dub.', Note) = 0 and charindex('nomen dubium', Note) = 0 then 'nomen dubium' else '' end +	
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
	inner join [spidey\sql2005].name_cache_test.test_name_2.Name na on DS.NameGuid = Na.NameId
where Note is not null

update n
set NomenclaturalStatus = null
from [spidey\sql2005].name_cache_test.test_name_2.name n
inner join tblName n2 on n2.NameGuid = n.NameId
where NomenclaturalStatus = ''



--add quality codes
update n2
set qualitycode = 
	case when (select COUNT(FieldStatusCounterPK) from funginamesfromprod.dbo.tblFieldStatus where FieldStatusRecordFk = cast(n.NameGuid as varchar(38))) = 0 then null
		when (select count(distinct FieldStatusLevelFK) from funginamesfromprod.dbo.tblFieldStatus where FieldStatusRecordFk = cast(n.NameGuid as varchar(38)) and FieldStatusLevelFK <> 4) = 1 then 
			(select top 1 fl.FieldStatusLevelText from funginamesfromprod.dbo.tblFieldStatus fs inner join funginamesfromprod.dbo.tblFieldStatusLevel fl on fl.FieldStatusLevelCounterPK = fs.FieldStatusLevelFK where FieldStatusRecordFk = cast(n.NameGuid as varchar(38)))
		when (select count(distinct FieldStatusLevelFK) from funginamesfromprod.dbo.tblFieldStatus where FieldStatusRecordFk = cast(n.NameGuid as varchar(38)) and FieldStatusLevelFK < 3) = 1 then 
			'Validated Mixed Sources'
		else
			'Partially Validated'
		end
from funginamesfromprod.dbo.tblName n
inner join [devserver02\sql2005].Name_cache_Test.test_name_2.[Name] n2 on n2.nameid = n.NameGuid

--standardise quality codes (title case)
update [devserver02\sql2005].Name_cache_Test.test_name_2.[Name] 
set qualitycode = 'Validated Primary Source'
where qualitycode = 'Validated primary source'

update [devserver02\sql2005].Name_cache_Test.test_name_2.[Name] 
set qualitycode = 'Validated Secondary Source'
where qualitycode = 'Validated Secondary source'


-- For any names that have a suppressed parent, we need to include the parent names (only the basic info)
INSERT INTO [devserver02\sql2005].Name_cache_Test.test_name_2.[Name](NameId, IsScientific,
	createdDate, 
	modifiedDate, 
	NameFull, 	
	[rank], 
	CanonicalName, 
	QualityCode)
select distinct 
	pn.NameGuid, 1,
	pn.NameAddedDate, 
	pn.NameUpdatedDate, 
	pn.NamePart, 
	ta.TaxonRankName,
	pn.NameCanonical,
	'Unvalidated'
from funginamesfromprod.dbo.tblName na
	inner join funginamesfromprod.dbo.tblFlatName fn on fn.FlatNameSeedName = na.NameGuid
	inner join funginamesfromprod.dbo.tblName pn on pn.NameGuid = fn.FlatNameNameUFk
	inner join funginamesfromprod.dbo.tblTaxonRank ta on pn.NameTaxonRankFk = ta.TaxonRankPk
	inner join funginamesfromprod.dbo.tblFlatName kfn on na.NameGuid = kfn.FlatNameSeedName
			and kfn.FlatNameRankName = 'kingdom'
where pn.NameSuppress = 1 and na.NameSuppress = 0
	and (kfn.FlatNameCanonical is null or kfn.FlatNameCanonical <> 'kingdomtemp') 
	
	
