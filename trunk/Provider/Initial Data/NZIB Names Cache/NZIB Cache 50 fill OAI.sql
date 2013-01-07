IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NZIBCache50]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[NZIBCache50]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[NZIBCache50]
as

delete nzib_name.OAI

insert into nzib_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Name', NameId, CreatedDate, ModifiedDate
	from nzib_name.Name


insert into nzib_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Vernacular', VernacularId, CreatedDate, ModifiedDate
	from nzib_name.Vernacular


insert into nzib_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'TaxonConcept', TaxonConceptId, CreatedDate, ModifiedDate
	from nzib_name.taxonconcept


insert into nzib_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'VernacularConcept', VernacularConceptId, CreatedDate, ModifiedDate
	from nzib_name.vernacularconcept

insert into nzib_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'VernacularUse', VernacularUseId, CreatedDate, ModifiedDate
	from nzib_name.vernacularuse

insert into nzib_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'TaxonNameUse', TaxonNameUseId, CreatedDate, ModifiedDate
	from nzib_name.TaxonNameUse
	


insert into nzib_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Biostatus', BiostatusId, CreatedDate, ModifiedDate
	from nzib_name.Biostatus


insert into nzib_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Publication', PublicationId, CreatedDate, ModifiedDate
	from nzib_name.Publication

insert into nzib_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Annotation', AnnotationId, CreatedDate, ModifiedDate
	from nzib_name.Annotation
	
go

grant execute on dbo.[NZIBCache50] to dbi_user

go
