IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestCache50]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TestCache50]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TestCache50]
as


--------------------------
--Plant names
delete [devserver02\sql2005].Name_Cache_Test.test_name.OAI

insert into [devserver02\sql2005].Name_Cache_Test.test_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Name', NameId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name.Name


insert into [devserver02\sql2005].Name_Cache_Test.test_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Vernacular', VernacularId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name.Vernacular


insert into [devserver02\sql2005].Name_Cache_Test.test_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'TaxonConcept', TaxonConceptId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name.taxonconcept


insert into [devserver02\sql2005].Name_Cache_Test.test_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'VernacularConcept', VernacularConceptId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name.vernacularconcept


insert into [devserver02\sql2005].Name_Cache_Test.test_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'TaxonNameUse', taxonnameuseId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name.taxonnameuse

	
insert into [devserver02\sql2005].Name_Cache_Test.test_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'VernacularUse', VernacularUseId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name.vernacularuse

insert into [devserver02\sql2005].Name_Cache_Test.test_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Biostatus', BiostatusId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name.Biostatus


insert into [devserver02\sql2005].Name_Cache_Test.test_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Publication', PublicationId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name.Publication
	
insert into [devserver02\sql2005].Name_Cache_Test.test_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Annotation', AnnotationId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name.Annotation
	
	
--------------------------
--Fungi names
delete [devserver02\sql2005].Name_Cache_Test.test_name_2.OAI

insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Name', NameId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.Name


insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Vernacular', VernacularId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.Vernacular


insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'TaxonConcept', TaxonConceptId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonconcept


insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'VernacularConcept', VernacularConceptId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.vernacularconcept


insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'TaxonNameUse', taxonnameuseId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonnameuse
	
insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'VernacularUse', VernacularUseId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.vernacularuse


insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Biostatus', BiostatusId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.Biostatus


insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Publication', PublicationId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.Publication

insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Annotation', AnnotationId, CreatedDate, ModifiedDate
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.Annotation
