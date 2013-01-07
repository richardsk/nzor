IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.[PNDCache0]') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.[PNDCache0]
go

SET ANSI_NULLS ON
go

SET QUOTED_IDENTIFIER ON
go

CREATE procedure dbo.[PNDCache0]
as

/****** Object:  ForeignKey [FK_Biostatus_Name]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE plant_name.[Biostatus] DROP CONSTRAINT [FK_Biostatus_Name]

/****** Object:  ForeignKey [FK_Biostatus_Publication]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE plant_name.[Biostatus] DROP CONSTRAINT [FK_Biostatus_Publication]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE plant_name.[ConceptApplication] DROP CONSTRAINT [FK_ConceptApplication_VernacularConcept]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE plant_name.[ConceptApplication] DROP CONSTRAINT [FK_ConceptApplication_VernacularConcept1]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE plant_name.[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationship_TaxonConcept]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE plant_name.[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationship_TaxonConcept1]

/****** Object:  ForeignKey [FK_Name_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[Name] DROP CONSTRAINT [FK_Name_Name]

/****** Object:  ForeignKey [FK_Name_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[Name] DROP CONSTRAINT [FK_Name_Name1]

/****** Object:  ForeignKey [FK_Name_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[Name] DROP CONSTRAINT [FK_Name_Name2]

/****** Object:  ForeignKey [FK_Name_Name3]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[Name] DROP CONSTRAINT [FK_Name_Name3]

/****** Object:  ForeignKey [FK_Name_Name4]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[Name] DROP CONSTRAINT [FK_Name_Name4]

/****** Object:  ForeignKey [FK_Name_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[Name] DROP CONSTRAINT [FK_Name_Publication]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name1]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name2]

/****** Object:  ForeignKey [FK_NameBasedConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Publication]

/****** Object:  ForeignKey [FK_PublicationTitle_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[PublicationTitle] DROP CONSTRAINT [FK_PublicationTitle_Publication]

/****** Object:  ForeignKey [FK_TaxonConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[TaxonConcept] DROP CONSTRAINT [FK_TaxonConcept_Name]

/****** Object:  ForeignKey [FK_TaxonConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[TaxonConcept] DROP CONSTRAINT [FK_TaxonConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[VernacularConcept] DROP CONSTRAINT [FK_VernacularConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Vernacular]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[VernacularConcept] DROP CONSTRAINT [FK_VernacularConcept_Vernacular]

ALTER TABLE [plant_name].[Annotation]  drop CONSTRAINT [FK_Annotation_Name] 

ALTER TABLE [plant_name].[Annotation]  drop CONSTRAINT [FK_Annotation_Publication] 

ALTER TABLE [plant_name].[Annotation]  drop CONSTRAINT [FK_Annotation_TaxonConcept] 



--truncate
truncate table plant_name.OAI
truncate table plant_name.deprecated
truncate table plant_name.biostatus
truncate table plant_name.annotation
truncate table plant_name.conceptapplication
truncate table plant_name.conceptRelationship
truncate table plant_name.vernacularconcept
truncate table plant_name.Vernacular
truncate table plant_name.vernacularuse
truncate table plant_name.taxonConcept
truncate table plant_name.TaxonNameUse
truncate table plant_name.Name
truncate table plant_name.publicationtitle
truncate table plant_name.publication
truncate table plant_name.metadata



--recreate constraints

/****** Object:  ForeignKey [FK_Biostatus_Name]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE plant_name.[Biostatus]  WITH NOCHECK ADD  CONSTRAINT [FK_Biostatus_Name] FOREIGN KEY([NameId])
REFERENCES plant_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE plant_name.[Biostatus] NOCHECK CONSTRAINT [FK_Biostatus_Name]

/****** Object:  ForeignKey [FK_Biostatus_Publication]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE plant_name.[Biostatus]  WITH NOCHECK ADD  CONSTRAINT [FK_Biostatus_Publication] FOREIGN KEY([AccordingToId])
REFERENCES plant_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE plant_name.[Biostatus] NOCHECK CONSTRAINT [FK_Biostatus_Publication]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE plant_name.[ConceptApplication]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptApplication_VernacularConcept] FOREIGN KEY([FromConceptId])
REFERENCES plant_name.[VernacularConcept] ([VernacularConceptId])
NOT FOR REPLICATION

ALTER TABLE plant_name.[ConceptApplication] NOCHECK CONSTRAINT [FK_ConceptApplication_VernacularConcept]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE plant_name.[ConceptApplication]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptApplication_VernacularConcept1] FOREIGN KEY([ToConceptId])
REFERENCES plant_name.[VernacularConcept] ([VernacularConceptId])
NOT FOR REPLICATION

ALTER TABLE plant_name.[ConceptApplication] NOCHECK CONSTRAINT [FK_ConceptApplication_VernacularConcept1]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE plant_name.[ConceptRelationship]  WITH CHECK ADD  CONSTRAINT [FK_ConceptRelationship_TaxonConcept] FOREIGN KEY([FromConceptId])
REFERENCES plant_name.[TaxonConcept] ([TaxonConceptId])

ALTER TABLE plant_name.[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_TaxonConcept]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE plant_name.[ConceptRelationship]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptRelationship_TaxonConcept1] FOREIGN KEY([ToConceptId])
REFERENCES plant_name.[TaxonConcept] ([TaxonConceptId])

ALTER TABLE plant_name.[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_TaxonConcept1]

/****** Object:  ForeignKey [FK_Name_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name] FOREIGN KEY([TypeNameID])
REFERENCES plant_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE plant_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name]

/****** Object:  ForeignKey [FK_Name_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name1] FOREIGN KEY([BasionymID])
REFERENCES plant_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE plant_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name1]

/****** Object:  ForeignKey [FK_Name_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name2] FOREIGN KEY([LaterHomonymOfId])
REFERENCES plant_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE plant_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name2]

/****** Object:  ForeignKey [FK_Name_Name3]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name3] FOREIGN KEY([BlockedNameId])
REFERENCES plant_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE plant_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name3]

/****** Object:  ForeignKey [FK_Name_Name4]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name4] FOREIGN KEY([RecombinedNameId])
REFERENCES plant_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE plant_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name4]

/****** Object:  ForeignKey [FK_Name_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Publication] FOREIGN KEY([PublishedInID])
REFERENCES plant_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE plant_name.[Name] NOCHECK CONSTRAINT [FK_Name_Publication]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name] FOREIGN KEY([NameId])
REFERENCES plant_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE plant_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name1] FOREIGN KEY([AcceptedNameId])
REFERENCES plant_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE plant_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name1]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name2] FOREIGN KEY([ParentNameId])
REFERENCES plant_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE plant_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name2]

/****** Object:  ForeignKey [FK_NameBasedConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES plant_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE plant_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Publication]

/****** Object:  ForeignKey [FK_PublicationTitle_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[PublicationTitle]  WITH NOCHECK ADD  CONSTRAINT [FK_PublicationTitle_Publication] FOREIGN KEY([PublicationId])
REFERENCES plant_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE plant_name.[PublicationTitle] NOCHECK CONSTRAINT [FK_PublicationTitle_Publication]

/****** Object:  ForeignKey [FK_TaxonConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[TaxonConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_TaxonConcept_Name] FOREIGN KEY([NameId])
REFERENCES plant_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE plant_name.[TaxonConcept] NOCHECK CONSTRAINT [FK_TaxonConcept_Name]

/****** Object:  ForeignKey [FK_TaxonConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[TaxonConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_TaxonConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES plant_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE plant_name.[TaxonConcept] NOCHECK CONSTRAINT [FK_TaxonConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[VernacularConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_VernacularConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES plant_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE plant_name.[VernacularConcept] NOCHECK CONSTRAINT [FK_VernacularConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Vernacular]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE plant_name.[VernacularConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_VernacularConcept_Vernacular] FOREIGN KEY([NameId])
REFERENCES plant_name.[Vernacular] ([VernacularId])
NOT FOR REPLICATION

ALTER TABLE plant_name.[VernacularConcept] NOCHECK CONSTRAINT [FK_VernacularConcept_Vernacular]

ALTER TABLE [plant_name].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_Name] FOREIGN KEY([NameId])
REFERENCES [plant_name].[Name] ([NameId])

ALTER TABLE [plant_name].[Annotation] CHECK CONSTRAINT [FK_Annotation_Name]

ALTER TABLE [plant_name].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_Publication] FOREIGN KEY([AccordingToId])
REFERENCES [plant_name].[Publication] ([PublicationID])

ALTER TABLE [plant_name].[Annotation] CHECK CONSTRAINT [FK_Annotation_Publication]

ALTER TABLE [plant_name].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_TaxonConcept] FOREIGN KEY([ConceptId])
REFERENCES [plant_name].[TaxonConcept] ([TaxonConceptId])

ALTER TABLE [plant_name].[Annotation] CHECK CONSTRAINT [FK_Annotation_TaxonConcept]


go

grant execute on dbo.[PNDCache0] to dbi_user

go
