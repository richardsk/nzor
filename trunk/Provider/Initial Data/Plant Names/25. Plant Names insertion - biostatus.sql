use NZOR_Data
go

--create synonym pName for NZOR_Data.prov.Name
--create synonym pNameProp for NZOR_Data.prov.NameProperty
--create synonym pNamePropClass for NZOR_Data.dbo.NameClassProperty
--create synonym pRefField for NZOR_Data.prov.ReferenceField
--create synonym pRef for NZOR_Data.prov.Reference
--create synonym pRefType for NZOR_Data.dbo.ReferenceType
--create synonym pRefFieldType for NZOR_Data.dbo.ReferenceFieldType
go


delete from NZOR_Data.prov.TaxonPropertyValue
delete from NZOR_Data.prov.TaxonProperty
go

select 
	newid() as ID,
	nb.NameBiostatusCounterPK as ProviderRecordId,
	nb.NameBiostatusUpdatedDate as ProviderUpdatedDate,
	nb.NameBiostatusNameFk as ProviderNameId,
	nb.NameBiostatusReferenceFk as ProviderReferenceId,
	gr.GeoRegionName as Georegion,
	grs.GeoRegionSchemaName as Geoschema,
	nb.NameBiostatusBiostatusFk,
	nb.NameBiostatusOccurrenceFK
 into #Biostatus
	from Proserver01.PlantNames.dbo.tblNameBiostatus nb
		inner join prov.Name pn on nb.NameBiostatusNameFk = pn.ProviderRecordId
		inner join Proserver01.PlantNames.dbo.tblGeoRegion gr on nb.NameBiostatusGeoRegionFK = gr.GeoRegionCounterPK
		inner join Proserver01.PlantNames.dbo.tblGeoRegionSchema grs on gr.GeoRegionSchemaFK = grs.GeoRegionSchemaCounterPK
	where (nb.NameBiostatusIsDeleted is null or nb.NameBiostatusIsDeleted = 0)
go

-- select * from #Biostatus

insert into NZOR_Data.prov.TaxonProperty(TaxonPropertyID, ProviderRecordId,
	ProviderUpdatedDate, ProviderNameId, ProviderReferenceId, Georegion, Geoschema,
	TaxonPropertyClassID, ProviderID)
select ID, providerrecordid, providerupdateddate, ProvidernameId,
	ProviderReferenceId, Georegion, Geoschema,
	(Select taxonPropertyClassId from NZOR_Data.dbo.TaxonPropertyClass
		where Name='Biostatus'),
	(Select ProviderId from NZOR_Data.dbo.Provider 
		where Name='Plant Names Database')
	from #Biostatus
go


-- insert the biostatus values - translating into NZOR schema
insert into NZOR_Data.prov.TaxonPropertyValue(TaxonPropertyValueID,
	TaxonPropertyID, TaxonPropertyTypeID, Value)
select NEWID(),
	b.ID,
	(Select TaxonPropertyTypeID from NZOR_Data.dbo.TaxonPropertyType
		where Name = 'Origin'),
	bs.BioStatusDescription
	from #Biostatus b
		inner join Proserver01.PlantNames.dbo.tblBioStatus bs on b.namebiostatusbiostatusfk = bs.BiostatusCodePK
	where b.namebiostatusbiostatusfk is not null
go	


insert into NZOR_Data.prov.TaxonPropertyValue(TaxonPropertyValueID,
	TaxonPropertyID, TaxonPropertyTypeID, Value)
select NEWID(),
	b.ID,
	(Select TaxonPropertyTypeID from NZOR_Data.dbo.TaxonPropertyType
		where Name = 'Occurrence'),
	case occ.OccurrenceDescription
		--when 'Extinct' then 'Extinct'
		--when 'Present' then 'Present'
		--when 'Sometimes present' then 'Sometimes present'
		when 'Wild' then 'Present'
		--when 'Uncertain' then 'Uncertain'
		--when 'Recorded in error' then 'Recorded in error'
		when 'Present in captivity/cultivation/culture' then 'Present'
		when 'Eradicated' then 'Eradicated/Destroyed'
		else occ.OccurrenceDescription
	end
	from #Biostatus b
		inner join Proserver01.PlantNames.dbo.tblOccurrence occ 
			on b.namebiostatusoccurrencefk = occ.OccurrenceCodePK
		where occ.OccurrenceDescription is not null
go

insert into NZOR_Data.prov.TaxonPropertyValue(TaxonPropertyValueID,
	TaxonPropertyID, TaxonPropertyTypeID, Value)
select NEWID(),
	b.ID,
	(Select TaxonPropertyTypeID from NZOR_Data.dbo.TaxonPropertyType
		where Name = 'Environmental Context'),
	case occ.OccurrenceDescription
		when 'Present in captivity/cultivation/culture' then 'Cultivation/Captivity'
		else 'Wild'
	end
	from #Biostatus b
		inner join Proserver01.PlantNames.dbo.tblOccurrence occ 
			on b.namebiostatusoccurrencefk = occ.OccurrenceCodePK
		where occ.OccurrenceDescription is not null
go


drop table #Biostatus