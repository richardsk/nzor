IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestCache30]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TestCache30]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TestCache30]
as

---------------------------------
--Plant name biostatus
delete [devserver02\sql2005].Name_Cache_Test.test_name.Biostatus

insert into [devserver02\sql2005].Name_Cache_Test.test_name.Biostatus(
	BiostatusId, CreatedDate, ModifiedDate, Taxon,
	ConceptId, NameId, AccordingTo, AccordingToId, Region,
	GeographicSchema, Biome, EnvironmentalContext, Origin, Occurrence)
select 
	nb.NameBiostatusCounterPK,
	nb.NameBiostatusAddedDate,
	nb.NameBiostatusUpdatedDate,
	Na.NameFull,
	null,
	nb.NameBiostatusNameFk,
	ref.ReferenceGenCitation,
	ref.ReferenceID,
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
	 inner join [devserver02\sql2005].Name_Cache_Test.test_name.Name na on nb.NameBiostatusNameFk
		= Na.NameId
	left join tblBioStatus b on nb.NameBiostatusBiostatusFk = b.BiostatusCodePK
	left join tblOccurrence occ on nb.NameBiostatusOccurrenceFK = occ.OccurrenceCodePK
	left join tblReference ref on nb.NameBiostatusReferenceFk = ref.ReferenceID
	left join tblGeoRegion geo on nb.NameBiostatusGeoRegionFK = geo.GeoRegionCounterPK
	left join tblGeoRegionSchema geos on geo.GeoRegionSchemaFK = geos.GeoRegionSchemaCounterPK
where (nb.NameBiostatusIsDeleted = 0 or nb.NameBiostatusIsDeleted is null)


---------------------------------
--Fungi name biostatus
delete [devserver02\sql2005].Name_Cache_Test.test_name_2.Biostatus

insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.Biostatus(
	BiostatusId, CreatedDate, ModifiedDate, Taxon,
	ConceptId, NameId, AccordingTo, AccordingToId, Region,
	GeographicSchema, Biome, EnvironmentalContext, Origin, Occurrence)
select 
	nb.NameBiostatusCounterPK,
	nb.NameBiostatusAddedDate,
	nb.NameBiostatusUpdatedDate,
	Na.NameFull,
	null,
	nb.NameBiostatusNameFk,
	ref.ReferenceGenCitation,
	ref.ReferenceID,
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
	 from funginamesfromprod.dbo.tblNameBiostatus nb
	 inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.Name na on nb.NameBiostatusNameFk
		= Na.NameId
	left join funginamesfromprod.dbo.tblBioStatus b on nb.NameBiostatusBiostatusFk = b.BiostatusCodePK
	left join funginamesfromprod.dbo.tblOccurrence occ on nb.NameBiostatusOccurrenceFK = occ.OccurrenceCodePK
	left join funginamesfromprod.dbo.tblReference ref on nb.NameBiostatusReferenceFk = ref.ReferenceID
	left join funginamesfromprod.dbo.tblGeoRegion geo on nb.NameBiostatusGeoRegionFK = geo.GeoRegionCounterPK
	left join funginamesfromprod.dbo.tblGeoRegionSchema geos on geo.GeoRegionSchemaFK = geos.GeoRegionSchemaCounterPK
where (nb.NameBiostatusIsDeleted = 0 or nb.NameBiostatusIsDeleted is null)

