IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NZACCache40]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[NZACCache40]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[NZACCache40]
as

delete from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication

select distinct	
	PublishedInId 
into #Refs
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Name
	where PublishedInId is not null
union
select distinct	
	PublishedInId 
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Vernacular
	where PublishedInId is not null
union
select distinct
	AccordingToId
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept
	where AccordingToId is not null
union	
select distinct
	AccordingToId
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.vernacularconcept
	where AccordingToId is not null
union
select distinct
	AccordingToId
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.TaxonNameUse
	where AccordingToId is not null
union
select distinct
	AccordingToId
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Biostatus
	where AccordingToId is not null
union
select distinct
	AccordingToId 
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Vernacularuse
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


	
insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication(
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
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Author, primary'


update pub
	set DateOfPublication = rf.ReferenceFieldText
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Date, primary'


update pub
	set DateOnPublication = rf.ReferenceFieldText
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Date, secondary'


update pub
	set Volume = rf.ReferenceFieldText
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Volume'


update pub
	set Issue = rf.ReferenceFieldText
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Issue'


update pub
	set PageStart = rf.ReferenceFieldText
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Start page'


update pub
	set pageEnd = rf.ReferenceFieldText
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'End page'


update pub
	set PublisherName = rf.ReferenceFieldText
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Publisher'


update pub
	set PublisherCity = rf.ReferenceFieldText
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Pub place'


insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	1,
	'full'
	from
	[NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub
	inner join  tblReferenceField rf on Pub.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'



insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	1,
	'abbreviation'
	from
	[NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub
	inner join  tblReferenceField rf on Pub.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Abbreviation'


insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	2,
	'full'
	from
	[NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub2
		on Pub.ParentPublicationId = pub2.publicationid
	inner join  tblReferenceField rf on Pub2.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'



insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	3,
	'full'
	from
	[NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub2
		on Pub.ParentPublicationId = pub2.publicationid
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Publication pub3
		on Pub2.ParentPublicationId = pub3.publicationid
	inner join  tblReferenceField rf on Pub3.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'


--standardise
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.publication set [type] = 'Article' where [type] = 'Article in journal'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.publication set [type] = 'Article' where [type] = 'Article in magazine'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.publication set [type] = 'Serial' where [type] = 'Serial (book/monograph) in series'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.publication set [type] = 'Book' where [type] = 'Book (in series)'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.publication set [type] = 'Chapter' where [type] = 'Chapter in book (in series)'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.publication set [type] = 'Electronic source' where [type] = 'Electronic citation in electronic source'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.publication set [type] = 'Unpublished' where [type] = 'Unpublished work (in series)'



drop table #refs


go

grant execute on dbo.[NZACCache40] to dbi_user

go
