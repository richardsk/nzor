IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FungiCache40]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FungiCache40]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[FungiCache40]
as

select distinct	
	PublishedInId 
into #Refs
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Name
	where PublishedInId is not null
union
select distinct	
	PublishedInId 
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Vernacular
	where PublishedInId is not null
union
select distinct
	AccordingToId
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.taxonconcept
	where AccordingToId is not null
union	
select distinct
	AccordingToId
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.vernacularconcept
	where AccordingToId is not null
union
select distinct
	AccordingToId
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.taxonnameuse
	where AccordingToId is not null
union
select distinct
	AccordingToId
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.VernacularUse
	where AccordingToId is not null
union
select distinct
	AccordingToId
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Biostatus
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


	
insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication(
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
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Author, primary'


update pub
	set DateOfPublication = left(rf.ReferenceFieldText, 100)
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Date, primary'


update pub
	set DateOnPublication = left(rf.ReferenceFieldText, 100)
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Date, secondary'


update pub
	set Volume = left(rf.ReferenceFieldText, 50)
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Volume'


update pub
	set Issue = left(rf.ReferenceFieldText, 150)
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Issue'


update pub
	set PageStart = left(rf.ReferenceFieldText, 50)
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Start page'


update pub
	set pageEnd = left(rf.ReferenceFieldText, 50)
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'End page'


update pub
	set PublisherName = left(rf.ReferenceFieldText, 150)
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Publisher'


update pub
	set PublisherCity = left(rf.ReferenceFieldText, 150)
from  [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Pub place'


insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	left(rf.ReferenceFieldText, 500),
	1,
	'full'
	from
	[NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub
	inner join  tblReferenceField rf on Pub.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'



insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	1,
	'abbreviation'
	from
	[NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub
	inner join  tblReferenceField rf on Pub.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Abbreviation'


insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	left(rf.ReferenceFieldText, 500),
	2,
	'full'
	from
	[NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub2
		on Pub.ParentPublicationId = pub2.publicationid
	inner join  tblReferenceField rf on Pub2.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'



insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	left(rf.ReferenceFieldText, 500),
	3,
	'full'
	from
	[NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub2
		on Pub.ParentPublicationId = pub2.publicationid
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Publication pub3
		on Pub2.ParentPublicationId = pub3.publicationid
	inner join  tblReferenceField rf on Pub3.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'


--standardise
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.publication set [type] = 'Article' where [type] = 'Article in journal'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.publication set [type] = 'Article' where [type] = 'Article in magazine'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.publication set [type] = 'Serial' where [type] = 'Serial (book/monograph) in series'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.publication set [type] = 'Book' where [type] = 'Book (in series)'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.publication set [type] = 'Chapter' where [type] = 'Chapter in book (in series)'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.publication set [type] = 'Electronic source' where [type] = 'Electronic citation in electronic source'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.publication set [type] = 'Unpublished' where [type] = 'Unpublished work (in series)'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.publication set [type] = 'Conference proceedings' where [type] = 'Conference article in proceedings'
update [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.publication set [type] = 'Report' where [type] = 'Report (in series)'


drop table #refs

go

grant execute on dbo.[FungiCache40] to dbi_user

go
