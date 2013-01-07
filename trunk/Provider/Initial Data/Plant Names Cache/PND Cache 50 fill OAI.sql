IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PNDCache50]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[PNDCache50]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[PNDCache50]
as

delete plant_name.OAI

insert into plant_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Name', NameId, CreatedDate, ModifiedDate
	from plant_name.Name


insert into plant_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Vernacular', VernacularId, CreatedDate, ModifiedDate
	from plant_name.Vernacular


insert into plant_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'TaxonConcept', TaxonConceptId, CreatedDate, ModifiedDate
	from plant_name.taxonconcept


insert into plant_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'VernacularConcept', VernacularConceptId, CreatedDate, ModifiedDate
	from plant_name.vernacularconcept

insert into plant_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'VernacularUse', VernacularUseId, CreatedDate, ModifiedDate
	from plant_name.VernacularUse

insert into plant_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'TaxonNameUse', TaxonNameUseId, CreatedDate, ModifiedDate
	from plant_name.TaxonNameUse


insert into plant_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Biostatus', BiostatusId, CreatedDate, ModifiedDate
	from plant_name.Biostatus


insert into plant_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Publication', PublicationId, CreatedDate, ModifiedDate
	from plant_name.Publication

insert into plant_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Annotation', AnnotationId, CreatedDate, ModifiedDate
	from plant_name.Annotation
	
go

grant execute on dbo.[PNDCache50] to dbi_user

go
