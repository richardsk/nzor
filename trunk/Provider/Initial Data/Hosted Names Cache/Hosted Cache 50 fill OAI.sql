IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HostedCache50]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[HostedCache50]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[HostedCache50]
as

delete Hosted_name.OAI

insert into Hosted_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Name', NameId, CreatedDate, ModifiedDate
	from Hosted_name.Name


insert into Hosted_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Vernacular', VernacularId, CreatedDate, ModifiedDate
	from Hosted_name.Vernacular


insert into Hosted_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'TaxonConcept', TaxonConceptId, CreatedDate, ModifiedDate
	from Hosted_name.taxonconcept


insert into Hosted_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'VernacularConcept', VernacularConceptId, CreatedDate, ModifiedDate
	from Hosted_name.vernacularconcept


insert into Hosted_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'TaxonNameUse', TaxonNameUseId, CreatedDate, ModifiedDate
	from Hosted_name.TaxonNameUse
	
insert into Hosted_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'VernacularUse', VernacularUseId, CreatedDate, ModifiedDate
	from Hosted_name.VernacularUse


insert into Hosted_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Biostatus', BiostatusId, CreatedDate, ModifiedDate
	from Hosted_name.Biostatus


insert into Hosted_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Publication', PublicationId, CreatedDate, ModifiedDate
	from Hosted_name.Publication

insert into hosted_name.OAI(oai_id, tableName, TableId, AddedDate, UpdatedDate)
select newid(), 'Annotation', AnnotationId, CreatedDate, ModifiedDate
	from hosted_name.Annotation
	
go

grant execute on dbo.[HostedCache50] to dbi_user

go
