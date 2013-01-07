IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NZIBCache30]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[NZIBCache30]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[NZIBCache30]
as

delete [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.Biostatus;

with bios as (
	  select
		nb.NameBiostatusCounterPK,
		nb.NameBiostatusAddedDate,
		nb.NameBiostatusUpdatedDate,
		Na.NameFull,
		cast(null as uniqueidentifier) as TaxonConceptId,
		nb.NameBiostatusNameFk,
		ref.ReferenceGenCitation,
		ref.ReferenceID,
		geo.GeoRegionName,
		geos.GeoRegionSchemaName,
		ltrim(rtrim(dbo.fnGetBiome(nb.NameBiostatusComment))) as Biome,
		case occ.OccurrenceDescription
			when 'Present in captivity/cultivation/culture' then 'Cultivation/Captivity'
			else 'Wild'
		end as EnviromentContext,
		case b.BioStatusDescription 
			when 'Uncertain' then 'Unknown'
			when 'Exotic Fully Naturalised' then 'Exotic'
			else b.BioStatusDescription 
		end as BioStatusDescription,
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
		end as OccurrenceDescription, 
		ROW_NUMBER() OVER(PARTITION BY n.NameGuid, geo.georegionname 
                                 ORDER BY nb.NameBiostatusAddedDate DESC) AS rk

		 from tblNameBiostatus nb
		 inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].Name_Cache.nzib_name.Name na on nb.NameBiostatusNameFk
			= Na.NameId
		inner join tblname n on n.nameguid = nb.namebiostatusnamefk
		left join tblBioStatus b on nb.NameBiostatusBiostatusFk = b.BiostatusCodePK
		left join tblOccurrence occ on nb.NameBiostatusOccurrenceFK = occ.OccurrenceCodePK
		left join tblReference ref on nb.NameBiostatusReferenceFk = ref.ReferenceID
		left join tblGeoRegion geo on nb.NameBiostatusGeoRegionFK = geo.GeoRegionCounterPK
		left join tblGeoRegionSchema geos on geo.GeoRegionSchemaFK = geos.GeoRegionSchemaCounterPK	
	where (nb.NameBiostatusIsDeleted = 0 or nb.NameBiostatusIsDeleted is null)
		and (isnull(n.namecurrentfk, n.nameguid) = n.nameguid)
		and occ.OccurrenceDescription is not null
		and geos.GeoRegionSchemaName is not null)
	
select b.*
into #bios
from bios b
where b.rk = 1

update b
set b.TaxonConceptId = tc.taxonconceptid
from #bios b
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].Name_Cache.nzib_name.taxonconcept tc on 
	tc.nameid = b.NameBiostatusNameFk and tc.accordingtoid = b.ReferenceID

delete #bios where NameBiostatusNameFk is null and TaxonConceptId is null

update b
set b.biome = null
from #bios b
where b.biome = ''
	
insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.Biostatus(
	BiostatusId, CreatedDate, ModifiedDate, Taxon,
	ConceptId, NameId, AccordingTo, AccordingToId, Region,
	GeographicSchema, Biome, EnvironmentalContext, Origin, Occurrence)
select 
	NameBiostatusCounterPK,
	NameBiostatusAddedDate,
	NameBiostatusUpdatedDate,
	NameFull,
	TaxonConceptId,
	case when TaxonConceptId is null then NameBiostatusNameFk else null end,
	ReferenceGenCitation,
	case when TaxonConceptId is null then ReferenceID else null end,
	GeoRegionName,
	GeoRegionSchemaName,
	Biome,
	EnviromentContext,
	BiostatusDescription,
	OccurrenceDescription
from #bios

drop table #bios
	
	
go

grant execute on dbo.[NZIBCache30] to dbi_user

go
