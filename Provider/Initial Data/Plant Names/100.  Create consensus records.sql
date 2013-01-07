use NZOR_Data
go

TRUNCATE TABLE cons.ConceptRelationship
delete from cons.Concept
TRUNCATE TABLE cons.NameProperty
truncate table cons.taxonpropertyValue
TRUNCATE TABLE cons.TaxonProperty
delete from cons.referenceField
delete from cons.Reference
delete from cons.Name
go


update Na
	set TaxonRankId = tr.TaxonRankId
from prov.Name na
		inner join prov.NameProperty np on na.NameID = np.NameID
		inner join dbo.NameClassProperty ncp on np.NameClassPropertyID = ncp.NameClassPropertyID
			and ncp.PropertyName = 'Rank'
		inner join dbo.TaxonRank tr on np.Value = tr.Name
go

select NEWID() as ID,  na.FullName, na.TaxonRankID, na.NameClassID, na.OriginalOrthography,
	na.GoverningCode, GETDATE() as AddedDate, na.NameID as ProvNameID
 into #Name
	from prov.Name na

insert into cons.Name(NameID, FullName, TaxonRankID, NameClassID, OriginalOrthography,
	GoverningCode, AddedDate, UpdatedDate)
select ID,  na.FullName, na.TaxonRankID, na.NameClassID, na.OriginalOrthography,
	na.GoverningCode, na.AddedDate, null 
from #Name na

update na
	set ConsensusNameID = nb.ID,
		Na.LinkStatus = 'ManualInsert'
	from prov.Name na
		inner join #Name nb on na.NameID = nb.ProvNameId
drop table #Name
go

insert into cons.NameProperty(NamePropertyID, NameID,
	NameClassPropertyID, Value, RelatedID, Sequence, AddedDate, UpdatedDate)
select NEWID(), na.ConsensusNameID, 
	np.NameClassPropertyID, np.Value, np.RelatedID, np.Sequence, GETDATE(), null
from prov.Name na
		inner join prov.NameProperty np on na.NameID = np.NameID
go

select NEWID() ID, r.* 
into #Ref
from prov.Reference r
	--where linkstatus is null

insert into cons.Reference(ReferenceID, ReferenceTypeID, AddedDate, UpdatedDate)
select ID, ReferenceTypeID, GETDATE(), null
from #Ref
	
update pref
	set pref.ConsensusReferenceID =  r.ID,
	LinkStatus = 'ManualInsert'
	from prov.Reference pref
	inner join #Ref r on pref.ReferenceID = r.ReferenceId
go

update con
 set AccordingToReferenceID = r.ID
from prov.Concept con 
	inner join #Ref r on con.ProviderReferenceID = r.ProviderRecordId
go

drop table #Ref
go



insert into cons.referenceField(ReferenceFieldID,
	ReferenceFieldTypeID, ReferenceID,Value)
select NEWID(),
	rf.ReferenceFieldTypeID,
	ref.ConsensusReferenceID,
	rf.Value
 from prov.ReferenceField rf
	inner join prov.Reference ref on rf.ReferenceID = ref.ReferenceID
--order by ref.consensusreferenceid
go


select NEWID() ID,
	con.*,
	na.ConsensusNameID
into #Concept
from prov.Concept con
	inner join prov.Name na on con.NameID = na.NameID

insert into cons.Concept(ConceptID, NameID, AccordingToReferenceID, AddedDate, UpdatedDate)
select ID, consensusnameId, AccordingToReferenceId, GETDATE(), null
	from #Concept
	
update pcon
 set ConsensusConceptID = con.Id,
	LinkStatus = 'ManualInsert'
 from prov.Concept pcon	
	inner join #concept con on pcon.ConceptID = con.conceptid

drop table #Concept
go

insert into cons.ConceptRelationship(ConceptRelationshipID,
	FromConceptID, ToConceptID, ConceptRelationshipTypeID,
	Sequence, AddedDate, UpdatedDate)
select NEWID(),
	con1.ConsensusConceptID,
	con2.ConsensusConceptID,
	cr.RelationshipTypeID,
	Sequence, GETDATE(), null
from prov.ConceptRelationship cr
	inner join prov.Concept con1 on cr.FromConceptID = con1.ConceptID
	inner join prov.Concept con2 on cr.ToConceptID = con2.ConceptID
	
go

update tp
	set GeoRegionID = gr.GeoRegionID
from prov.TaxonProperty tp
		inner join GeoRegion gr on tp.GeoRegion = gr.RegionName
			and gr.GeoRegionSchemaID = '39257942-37EC-4BF2-8DE0-CAF60D66493E'
	where GeoSchema = 'Political Region'
go


update tp
	set GeoRegionID = gr.GeoRegionID
from prov.TaxonProperty tp
		inner join (select GeoRegionID, RegionName,
							case CHARINDEX('(', RegionName)
								when 0 then RegionName
								else rtrim(LEFT(Regionname, charindex('(', RegionName)-1))
							end as Region2
					from GeoRegion
					where GeoRegionSchemaID = '1FB78A56-6061-4E10-9696-906AB41D5BE0') gr 
		on tp.GeoRegion = gr.RegionName
	where GeoSchema = 'World Geographic Scheme for Recording Plant Distributions'
go

update tp	
	set GeoRegionId = '9C61AFE9-279A-4278-B6F7-F164ED140D63'
from prov.TaxonProperty tp
	where tp.GeoSchema = 'NZ Botanical Region'
		and GeoRegion = 'New Zealand'
go


update tp
	set NameId = pn.NameId
from prov.TaxonProperty tp
	inner join prov.Name pn on tp.ProviderNameID = pn.ProviderRecordID
go

update tp
	set ReferenceID = pr.ReferenceID
from prov.TaxonProperty tp
	inner join prov.Reference pr on tp.ProviderReferenceID = pr.ProviderRecordID
	where tp.ReferenceID is null
go

select NEWID() id,
	tp.*,
	na.ConsensusNameID,
	r.ConsensusReferenceID
into #TaxP
	from prov.TaxonProperty  tp
		inner join prov.Name na on tp.NameID = na.NameID
		left join prov.Reference r on tp.ReferenceID = r.ReferenceID
 where GeoRegionID is not null

insert into cons.TaxonProperty(TaxonPropertyID,
	TaxonPropertyClassID, GeoRegionID, ReferenceID,
	ConceptID, NameID, InUse, AddedDate,UpdatedDate)
select id,
	TaxonPropertyClassid, georegionid, ConsensusReferenceID,
	conceptid, ConsensusNameID, InUse, GETDATE(), null
	 from #TaxP


update tp
	set tp.ConsensusTaxonPropertyID = tap.id,
	LinkStatus = 'ManualInsert'
from prov.TaxonProperty tp
	inner join #TaxP tap on tp.TaxonPropertyID = tap.TaxonPropertyId
go


insert into cons.TaxonPropertyValue(TaxonPropertyValueID,
	TaxonPropertyTypeID, TaxonPropertyID,Value)
select NEWID(), TaxonPropertyTypeId, tp.id, tpv.value
	from #Taxp	 tp
		inner join prov.TaxonPropertyValue tpv on tp.TaxonPropertyId = tpv.TaxonPropertyID
go

drop table #TaxP

go