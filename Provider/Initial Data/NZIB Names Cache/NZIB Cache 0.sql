IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.[NZIBCache0]') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.[NZIBCache0]
go

SET ANSI_NULLS ON
go

SET QUOTED_IDENTIFIER ON
go

CREATE procedure dbo.[NZIBCache0]
as



/****** Object:  ForeignKey [FK_Biostatus_Name]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE nzib_name.[Biostatus] DROP CONSTRAINT [FK_Biostatus_Name]

/****** Object:  ForeignKey [FK_Biostatus_Publication]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE nzib_name.[Biostatus] DROP CONSTRAINT [FK_Biostatus_Publication]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE nzib_name.[ConceptApplication] DROP CONSTRAINT [FK_ConceptApplication_VernacularConcept]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE nzib_name.[ConceptApplication] DROP CONSTRAINT [FK_ConceptApplication_VernacularConcept1]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE nzib_name.[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationship_TaxonConcept]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE nzib_name.[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationship_TaxonConcept1]

/****** Object:  ForeignKey [FK_Name_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[Name] DROP CONSTRAINT [FK_Name_Name]

/****** Object:  ForeignKey [FK_Name_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[Name] DROP CONSTRAINT [FK_Name_Name1]

/****** Object:  ForeignKey [FK_Name_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[Name] DROP CONSTRAINT [FK_Name_Name2]

/****** Object:  ForeignKey [FK_Name_Name3]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[Name] DROP CONSTRAINT [FK_Name_Name3]

/****** Object:  ForeignKey [FK_Name_Name4]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[Name] DROP CONSTRAINT [FK_Name_Name4]

/****** Object:  ForeignKey [FK_Name_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[Name] DROP CONSTRAINT [FK_Name_Publication]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name1]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name2]

/****** Object:  ForeignKey [FK_NameBasedConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Publication]

/****** Object:  ForeignKey [FK_PublicationTitle_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[PublicationTitle] DROP CONSTRAINT [FK_PublicationTitle_Publication]

/****** Object:  ForeignKey [FK_TaxonConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[TaxonConcept] DROP CONSTRAINT [FK_TaxonConcept_Name]

/****** Object:  ForeignKey [FK_TaxonConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[TaxonConcept] DROP CONSTRAINT [FK_TaxonConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[VernacularConcept] DROP CONSTRAINT [FK_VernacularConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Vernacular]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[VernacularConcept] DROP CONSTRAINT [FK_VernacularConcept_Vernacular]

ALTER TABLE [nzib_name].[Annotation]  drop CONSTRAINT [FK_Annotation_Name] 

ALTER TABLE [nzib_name].[Annotation]  drop CONSTRAINT [FK_Annotation_Publication] 

ALTER TABLE [nzib_name].[Annotation]  drop CONSTRAINT [FK_Annotation_TaxonConcept] 



--truncate
truncate table nzib_name.OAI
truncate table nzib_name.deprecated
truncate table nzib_name.biostatus
truncate table nzib_name.annotation
truncate table nzib_name.conceptapplication
truncate table nzib_name.conceptRelationship
truncate table nzib_name.vernacularconcept
truncate table nzib_name.Vernacular
truncate table nzib_name.taxonConcept
truncate table nzib_name.TaxonNameUse
truncate table nzib_name.VernacularUse
truncate table nzib_name.Name
truncate table nzib_name.publicationtitle
truncate table nzib_name.publication
truncate table nzib_name.metadata



--recreate constraints

/****** Object:  ForeignKey [FK_Biostatus_Name]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE nzib_name.[Biostatus]  WITH NOCHECK ADD  CONSTRAINT [FK_Biostatus_Name] FOREIGN KEY([NameId])
REFERENCES nzib_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[Biostatus] NOCHECK CONSTRAINT [FK_Biostatus_Name]

/****** Object:  ForeignKey [FK_Biostatus_Publication]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE nzib_name.[Biostatus]  WITH NOCHECK ADD  CONSTRAINT [FK_Biostatus_Publication] FOREIGN KEY([AccordingToId])
REFERENCES nzib_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[Biostatus] NOCHECK CONSTRAINT [FK_Biostatus_Publication]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE nzib_name.[ConceptApplication]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptApplication_VernacularConcept] FOREIGN KEY([FromConceptId])
REFERENCES nzib_name.[VernacularConcept] ([VernacularConceptId])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[ConceptApplication] NOCHECK CONSTRAINT [FK_ConceptApplication_VernacularConcept]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE nzib_name.[ConceptApplication]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptApplication_VernacularConcept1] FOREIGN KEY([ToConceptId])
REFERENCES nzib_name.[VernacularConcept] ([VernacularConceptId])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[ConceptApplication] NOCHECK CONSTRAINT [FK_ConceptApplication_VernacularConcept1]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE nzib_name.[ConceptRelationship]  WITH CHECK ADD  CONSTRAINT [FK_ConceptRelationship_TaxonConcept] FOREIGN KEY([FromConceptId])
REFERENCES nzib_name.[TaxonConcept] ([TaxonConceptId])

ALTER TABLE nzib_name.[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_TaxonConcept]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE nzib_name.[ConceptRelationship]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptRelationship_TaxonConcept1] FOREIGN KEY([ToConceptId])
REFERENCES nzib_name.[TaxonConcept] ([TaxonConceptId])

ALTER TABLE nzib_name.[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_TaxonConcept1]

/****** Object:  ForeignKey [FK_Name_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name] FOREIGN KEY([TypeNameID])
REFERENCES nzib_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name]

/****** Object:  ForeignKey [FK_Name_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name1] FOREIGN KEY([BasionymID])
REFERENCES nzib_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name1]

/****** Object:  ForeignKey [FK_Name_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name2] FOREIGN KEY([LaterHomonymOfId])
REFERENCES nzib_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name2]

/****** Object:  ForeignKey [FK_Name_Name3]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name3] FOREIGN KEY([BlockedNameId])
REFERENCES nzib_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name3]

/****** Object:  ForeignKey [FK_Name_Name4]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name4] FOREIGN KEY([RecombinedNameId])
REFERENCES nzib_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name4]

/****** Object:  ForeignKey [FK_Name_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Publication] FOREIGN KEY([PublishedInID])
REFERENCES nzib_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[Name] NOCHECK CONSTRAINT [FK_Name_Publication]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name] FOREIGN KEY([NameId])
REFERENCES nzib_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name1] FOREIGN KEY([AcceptedNameId])
REFERENCES nzib_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name1]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name2] FOREIGN KEY([ParentNameId])
REFERENCES nzib_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name2]

/****** Object:  ForeignKey [FK_NameBasedConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES nzib_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Publication]

/****** Object:  ForeignKey [FK_PublicationTitle_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[PublicationTitle]  WITH NOCHECK ADD  CONSTRAINT [FK_PublicationTitle_Publication] FOREIGN KEY([PublicationId])
REFERENCES nzib_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[PublicationTitle] NOCHECK CONSTRAINT [FK_PublicationTitle_Publication]

/****** Object:  ForeignKey [FK_TaxonConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[TaxonConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_TaxonConcept_Name] FOREIGN KEY([NameId])
REFERENCES nzib_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[TaxonConcept] NOCHECK CONSTRAINT [FK_TaxonConcept_Name]

/****** Object:  ForeignKey [FK_TaxonConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[TaxonConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_TaxonConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES nzib_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[TaxonConcept] NOCHECK CONSTRAINT [FK_TaxonConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[VernacularConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_VernacularConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES nzib_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[VernacularConcept] NOCHECK CONSTRAINT [FK_VernacularConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Vernacular]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE nzib_name.[VernacularConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_VernacularConcept_Vernacular] FOREIGN KEY([NameId])
REFERENCES nzib_name.[Vernacular] ([VernacularId])
NOT FOR REPLICATION

ALTER TABLE nzib_name.[VernacularConcept] NOCHECK CONSTRAINT [FK_VernacularConcept_Vernacular]

ALTER TABLE [nzib_name].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_Name] FOREIGN KEY([NameId])
REFERENCES [nzib_name].[Name] ([NameId])

ALTER TABLE [nzib_name].[Annotation] CHECK CONSTRAINT [FK_Annotation_Name]

ALTER TABLE [nzib_name].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_Publication] FOREIGN KEY([AccordingToId])
REFERENCES [nzib_name].[Publication] ([PublicationID])

ALTER TABLE [nzib_name].[Annotation] CHECK CONSTRAINT [FK_Annotation_Publication]

ALTER TABLE [nzib_name].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_TaxonConcept] FOREIGN KEY([ConceptId])
REFERENCES [nzib_name].[TaxonConcept] ([TaxonConceptId])

ALTER TABLE [nzib_name].[Annotation] CHECK CONSTRAINT [FK_Annotation_TaxonConcept]


go

grant execute on dbo.[NZIBCache0] to dbi_user

go
