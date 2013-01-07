IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FungiCache50]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FungiCache50]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[FungiCache50]
as

delete fungi_name.OAI

insert into fungi_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Name', NameId, getdate(), getdate()
	from fungi_name.Name
	


insert into fungi_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Vernacular', VernacularId, CreatedDate, ModifiedDate
	from fungi_name.Vernacular


insert into fungi_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'TaxonConcept', TaxonConceptId, CreatedDate, ModifiedDate
	from fungi_name.taxonconcept


insert into fungi_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'VernacularConcept', VernacularConceptId, CreatedDate, ModifiedDate
	from fungi_name.vernacularconcept

insert into fungi_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'VernacularUse', VernacularUseId, CreatedDate, ModifiedDate
	from fungi_name.VernacularUse

insert into fungi_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'TaxonNameUse', TaxonNameUseId, CreatedDate, ModifiedDate
	from fungi_name.taxonnameuse


insert into fungi_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Biostatus', BiostatusId, CreatedDate, ModifiedDate
	from fungi_name.Biostatus


insert into fungi_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Publication', PublicationId, CreatedDate, ModifiedDate
	from fungi_name.Publication
	
insert into fungi_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Annotation', AnnotationId, CreatedDate, ModifiedDate
	from fungi_name.Annotation

go

grant execute on dbo.[FungiCache50] to dbi_user

go
