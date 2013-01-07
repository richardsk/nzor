IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestCache40]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TestCache40]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TestCache40]
as


----------------------------------
--Plant name references
select distinct	
	PublishedInId 
into #Refs
from [devserver02\sql2005].Name_Cache_Test.test_name.Name
	where PublishedInId is not null
union
select distinct	
	PublishedInId 
from [devserver02\sql2005].Name_Cache_Test.test_name.Vernacular
	where PublishedInId is not null
union
select distinct
	AccordingToId
from [devserver02\sql2005].Name_Cache_Test.test_name.taxonconcept
	where AccordingToId is not null
union	
select distinct
	AccordingToId
from [devserver02\sql2005].Name_Cache_Test.test_name.vernacularconcept
	where AccordingToId is not null
union
select distinct
	AccordingToId
from [devserver02\sql2005].Name_Cache_Test.test_name.taxonnameuse
	where AccordingToId is not null
union
select distinct
	AccordingToId
from [devserver02\sql2005].Name_Cache_Test.test_name.vernacularuse
	where AccordingToId is not null
union
select distinct
	AccordingToId
from [devserver02\sql2005].Name_Cache_Test.test_name.Biostatus
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


	
insert into [devserver02\sql2005].Name_Cache_Test.test_name.Publication(
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
from  [devserver02\sql2005].Name_Cache_Test.test_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Author, primary'


update pub
	set DateOfPublication = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Date, primary'


update pub
	set DateOnPublication = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Date, secondary'


update pub
	set Volume = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Volume'


update pub
	set Issue = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Issue'


update pub
	set PageStart = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Start page'


update pub
	set pageEnd = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'End page'


update pub
	set PublisherName = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Publisher'


update pub
	set PublisherCity = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name.Publication pub
	inner join tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Pub place'


insert into [devserver02\sql2005].Name_Cache_Test.test_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	1,
	'full'
	from
	[devserver02\sql2005].Name_Cache_Test.test_name.Publication pub
	inner join  tblReferenceField rf on Pub.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'



insert into [devserver02\sql2005].Name_Cache_Test.test_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	1,
	'abbreviation'
	from
	[devserver02\sql2005].Name_Cache_Test.test_name.Publication pub
	inner join  tblReferenceField rf on Pub.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Abbreviation'


insert into [devserver02\sql2005].Name_Cache_Test.test_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	2,
	'full'
	from
	[devserver02\sql2005].Name_Cache_Test.test_name.Publication pub
	inner join [devserver02\sql2005].Name_Cache_Test.test_name.Publication pub2
		on Pub.ParentPublicationId = pub2.publicationid
	inner join  tblReferenceField rf on Pub2.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'



insert into [devserver02\sql2005].Name_Cache_Test.test_name.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	3,
	'full'
	from
	[devserver02\sql2005].Name_Cache_Test.test_name.Publication pub
	inner join [devserver02\sql2005].Name_Cache_Test.test_name.Publication pub2
		on Pub.ParentPublicationId = pub2.publicationid
	inner join [devserver02\sql2005].Name_Cache_Test.test_name.Publication pub3
		on Pub2.ParentPublicationId = pub3.publicationid
	inner join  tblReferenceField rf on Pub3.PublicationId = rf.ReferenceID
	inner join tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'


--standardise
update [devserver02\sql2005].Name_Cache_Test.test_name.publication set [type] = 'Article' where [type] = 'Article in journal'
update [devserver02\sql2005].Name_Cache_Test.test_name.publication set [type] = 'Article' where [type] = 'Article in magazine'
update [devserver02\sql2005].Name_Cache_Test.test_name.publication set [type] = 'Serial' where [type] = 'Serial (book/monograph) in series'
update [devserver02\sql2005].Name_Cache_Test.test_name.publication set [type] = 'Book' where [type] = 'Book (in series)'
update [devserver02\sql2005].Name_Cache_Test.test_name.publication set [type] = 'Report' where [type] = 'Report (in series)'
update [devserver02\sql2005].Name_Cache_Test.test_name.publication set [type] = 'Chapter' where [type] = 'Chapter in book (in series)'
update [devserver02\sql2005].Name_Cache_Test.test_name.publication set [type] = 'Electronic source' where [type] = 'Electronic citation in electronic source'
update [devserver02\sql2005].Name_Cache_Test.test_name.publication set [type] = 'Unpublished' where [type] = 'Unpublished work (in series)'


delete #refs



----------------------------------
--Fungi name references
insert into #refs
select distinct	
	PublishedInId 
from [devserver02\sql2005].Name_Cache_Test.test_name_2.Name
	where PublishedInId is not null
union
select distinct	
	PublishedInId 
from [devserver02\sql2005].Name_Cache_Test.test_name_2.Vernacular
	where PublishedInId is not null
union
select distinct
	AccordingToId
from [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonconcept
	where AccordingToId is not null
union	
select distinct
	AccordingToId
from [devserver02\sql2005].Name_Cache_Test.test_name_2.vernacularconcept
	where AccordingToId is not null
union
select distinct
	AccordingToId
from [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonnameuse
	where AccordingToId is not null
union
select distinct
	AccordingToId
from [devserver02\sql2005].Name_Cache_Test.test_name_2.vernacularuse
	where AccordingToId is not null
union
select distinct
	AccordingToId
from [devserver02\sql2005].Name_Cache_Test.test_name_2.Biostatus
	where AccordingToId is not null


insert into #Refs(PublishedInId)
select distinct r.ReferenceParentID 
	from #Refs rl
	inner join funginamesfromprod.dbo.tblReference r on rl.PublishedInId = r.ReferenceID
	left join #Refs rp on r.ReferenceParentID = rp.PublishedInId
where rp.PublishedInId is null
	and r.ReferenceParentID is not null
	and r.ReferenceParentID <> '00000000-0000-0000-0000-000000000000'



insert into #Refs(PublishedInId)
select distinct r.ReferenceParentID 
	from #Refs rl
	inner join funginamesfromprod.dbo.tblReference r on rl.PublishedInId = r.ReferenceID
	left join #Refs rp on r.ReferenceParentID = rp.PublishedInId
where rp.PublishedInId is null
	and r.ReferenceParentID is not null
	and r.ReferenceParentID <> '00000000-0000-0000-0000-000000000000'


	
insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication(
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
		inner join funginamesfromprod.dbo.tblReference ref on rl.PublishedInId = ref.ReferenceID
		inner join funginamesfromprod.dbo.tblReferenceType rt on ref.ReferenceTypeID = rt.ReferenceTypeID
	where (ref.ReferenceIsDeleted is null or ref.ReferenceIsDeleted = 0)


update pub
	set AuthorsSimple = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub
	inner join funginamesfromprod.dbo.tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join funginamesfromprod.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Author, primary'


update pub
	set DateOfPublication = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub
	inner join funginamesfromprod.dbo.tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join funginamesfromprod.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Date, primary'


update pub
	set DateOnPublication = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub
	inner join funginamesfromprod.dbo.tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join funginamesfromprod.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Date, secondary'


update pub
	set Volume = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub
	inner join funginamesfromprod.dbo.tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join funginamesfromprod.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Volume'


update pub
	set Issue = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub
	inner join funginamesfromprod.dbo.tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join funginamesfromprod.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Issue'


update pub
	set PageStart = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub
	inner join funginamesfromprod.dbo.tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join funginamesfromprod.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Start page'


update pub
	set pageEnd = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub
	inner join funginamesfromprod.dbo.tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join funginamesfromprod.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'End page'


update pub
	set PublisherName = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub
	inner join funginamesfromprod.dbo.tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join funginamesfromprod.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Publisher'


update pub
	set PublisherCity = rf.ReferenceFieldText
from  [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub
	inner join funginamesfromprod.dbo.tblReferenceField rf on Pub.PublicationId
			= rf.ReferenceID
	inner join funginamesfromprod.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Pub place'


insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	1,
	'full'
	from
	[devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub
	inner join  funginamesfromprod.dbo.tblReferenceField rf on Pub.PublicationId = rf.ReferenceID
	inner join funginamesfromprod.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'



insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	1,
	'abbreviation'
	from
	[devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub
	inner join  funginamesfromprod.dbo.tblReferenceField rf on Pub.PublicationId = rf.ReferenceID
	inner join funginamesfromprod.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Abbreviation'


insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	2,
	'full'
	from
	[devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub
	inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub2
		on Pub.ParentPublicationId = pub2.publicationid
	inner join  funginamesfromprod.dbo.tblReferenceField rf on Pub2.PublicationId = rf.ReferenceID
	inner join funginamesfromprod.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'



insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.PublicationTitle(
	PublicationId, Title, [Level], [type])
select Pub.PublicationId,
	rf.ReferenceFieldText,
	3,
	'full'
	from
	[devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub
	inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub2
		on Pub.ParentPublicationId = pub2.publicationid
	inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication pub3
		on Pub2.ParentPublicationId = pub3.publicationid
	inner join  funginamesfromprod.dbo.tblReferenceField rf on Pub3.PublicationId = rf.ReferenceID
	inner join funginamesfromprod.dbo.tblReferenceFieldType rft on rf.ReferenceFieldTypeID = rft.ReferenceFieldTypeID
		and rft.ReferenceFieldTypeText = 'Title'


--standardise
update [devserver02\sql2005].Name_Cache_Test.test_name_2.publication set [type] = 'Article' where [type] = 'Article in journal'
update [devserver02\sql2005].Name_Cache_Test.test_name_2.publication set [type] = 'Article' where [type] = 'Article in magazine'
update [devserver02\sql2005].Name_Cache_Test.test_name_2.publication set [type] = 'Serial' where [type] = 'Serial (book/monograph) in series'
update [devserver02\sql2005].Name_Cache_Test.test_name_2.publication set [type] = 'Book' where [type] = 'Book (in series)'
update [devserver02\sql2005].Name_Cache_Test.test_name_2.publication set [type] = 'Report' where [type] = 'Report (in series)'
update [devserver02\sql2005].Name_Cache_Test.test_name_2.publication set [type] = 'Chapter' where [type] = 'Chapter in book (in series)'
update [devserver02\sql2005].Name_Cache_Test.test_name_2.publication set [type] = 'Electronic source' where [type] = 'Electronic citation in electronic source'
update [devserver02\sql2005].Name_Cache_Test.test_name_2.publication set [type] = 'Unpublished' where [type] = 'Unpublished work (in series)'



drop table #refs

