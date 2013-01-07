IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.[TestCache0]') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.[TestCache0]
go

SET ANSI_NULLS ON
go

SET QUOTED_IDENTIFIER ON
go

CREATE procedure dbo.[TestCache0]
as


--------------------------------
--Plant names

/****** Object:  ForeignKey [FK_Biostatus_Name]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name.[Biostatus] DROP CONSTRAINT [FK_Biostatus_Name]

/****** Object:  ForeignKey [FK_Biostatus_Publication]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name.[Biostatus] DROP CONSTRAINT [FK_Biostatus_Publication]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name.[ConceptApplication] DROP CONSTRAINT [FK_ConceptApplication_VernacularConcept]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name.[ConceptApplication] DROP CONSTRAINT [FK_ConceptApplication_VernacularConcept1]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name.[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationship_TaxonConcept]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name.[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationship_TaxonConcept1]

/****** Object:  ForeignKey [FK_Name_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[Name] DROP CONSTRAINT [FK_Name_Name]

/****** Object:  ForeignKey [FK_Name_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[Name] DROP CONSTRAINT [FK_Name_Name1]

/****** Object:  ForeignKey [FK_Name_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[Name] DROP CONSTRAINT [FK_Name_Name2]

/****** Object:  ForeignKey [FK_Name_Name3]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[Name] DROP CONSTRAINT [FK_Name_Name3]

/****** Object:  ForeignKey [FK_Name_Name4]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[Name] DROP CONSTRAINT [FK_Name_Name4]

/****** Object:  ForeignKey [FK_Name_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[Name] DROP CONSTRAINT [FK_Name_Publication]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name1]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name2]

/****** Object:  ForeignKey [FK_NameBasedConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Publication]

/****** Object:  ForeignKey [FK_PublicationTitle_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[PublicationTitle] DROP CONSTRAINT [FK_PublicationTitle_Publication]

/****** Object:  ForeignKey [FK_TaxonConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[TaxonConcept] DROP CONSTRAINT [FK_TaxonConcept_Name]

/****** Object:  ForeignKey [FK_TaxonConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[TaxonConcept] DROP CONSTRAINT [FK_TaxonConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[VernacularConcept] DROP CONSTRAINT [FK_VernacularConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Vernacular]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[VernacularConcept] DROP CONSTRAINT [FK_VernacularConcept_Vernacular]

ALTER TABLE [test_name].[Annotation]  drop CONSTRAINT [FK_Annotation_Name] 

ALTER TABLE [test_name].[Annotation]  drop CONSTRAINT [FK_Annotation_Publication] 

ALTER TABLE [test_name].[Annotation]  drop CONSTRAINT [FK_Annotation_TaxonConcept] ]



--truncate
truncate table test_name.OAI
truncate table test_name.deprecated
truncate table test_name.biostatus
truncate table test_name.annotation
truncate table test_name.conceptapplication
truncate table test_name.conceptRelationship
truncate table test_name.vernacularconcept
truncate table test_name.Vernacular
truncate table test_name.taxonConcept
truncate table test_name.TaxonNameUse
truncate table test_name.vernacularuse
truncate table test_name.Name
truncate table test_name.publicationtitle
truncate table test_name.publication
truncate table test_name.metadata



--recreate constraints

/****** Object:  ForeignKey [FK_Biostatus_Name]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name.[Biostatus]  WITH NOCHECK ADD  CONSTRAINT [FK_Biostatus_Name] FOREIGN KEY([NameId])
REFERENCES test_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name.[Biostatus] NOCHECK CONSTRAINT [FK_Biostatus_Name]

/****** Object:  ForeignKey [FK_Biostatus_Publication]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name.[Biostatus]  WITH NOCHECK ADD  CONSTRAINT [FK_Biostatus_Publication] FOREIGN KEY([AccordingToId])
REFERENCES test_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE test_name.[Biostatus] NOCHECK CONSTRAINT [FK_Biostatus_Publication]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name.[ConceptApplication]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptApplication_VernacularConcept] FOREIGN KEY([FromConceptId])
REFERENCES test_name.[VernacularConcept] ([VernacularConceptId])
NOT FOR REPLICATION

ALTER TABLE test_name.[ConceptApplication] NOCHECK CONSTRAINT [FK_ConceptApplication_VernacularConcept]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name.[ConceptApplication]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptApplication_VernacularConcept1] FOREIGN KEY([ToConceptId])
REFERENCES test_name.[VernacularConcept] ([VernacularConceptId])
NOT FOR REPLICATION

ALTER TABLE test_name.[ConceptApplication] NOCHECK CONSTRAINT [FK_ConceptApplication_VernacularConcept1]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name.[ConceptRelationship]  WITH CHECK ADD  CONSTRAINT [FK_ConceptRelationship_TaxonConcept] FOREIGN KEY([FromConceptId])
REFERENCES test_name.[TaxonConcept] ([TaxonConceptId])

ALTER TABLE test_name.[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_TaxonConcept]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name.[ConceptRelationship]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptRelationship_TaxonConcept1] FOREIGN KEY([ToConceptId])
REFERENCES test_name.[TaxonConcept] ([TaxonConceptId])

ALTER TABLE test_name.[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_TaxonConcept1]

/****** Object:  ForeignKey [FK_Name_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name] FOREIGN KEY([TypeNameID])
REFERENCES test_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name]

/****** Object:  ForeignKey [FK_Name_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name1] FOREIGN KEY([BasionymID])
REFERENCES test_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name1]

/****** Object:  ForeignKey [FK_Name_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name2] FOREIGN KEY([LaterHomonymOfId])
REFERENCES test_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name2]

/****** Object:  ForeignKey [FK_Name_Name3]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name3] FOREIGN KEY([BlockedNameId])
REFERENCES test_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name3]

/****** Object:  ForeignKey [FK_Name_Name4]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name4] FOREIGN KEY([RecombinedNameId])
REFERENCES test_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name4]

/****** Object:  ForeignKey [FK_Name_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Publication] FOREIGN KEY([PublishedInID])
REFERENCES test_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE test_name.[Name] NOCHECK CONSTRAINT [FK_Name_Publication]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name] FOREIGN KEY([NameId])
REFERENCES test_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name1] FOREIGN KEY([AcceptedNameId])
REFERENCES test_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name1]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name2] FOREIGN KEY([ParentNameId])
REFERENCES test_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name2]

/****** Object:  ForeignKey [FK_NameBasedConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES test_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE test_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Publication]

/****** Object:  ForeignKey [FK_PublicationTitle_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[PublicationTitle]  WITH NOCHECK ADD  CONSTRAINT [FK_PublicationTitle_Publication] FOREIGN KEY([PublicationId])
REFERENCES test_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE test_name.[PublicationTitle] NOCHECK CONSTRAINT [FK_PublicationTitle_Publication]

/****** Object:  ForeignKey [FK_TaxonConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[TaxonConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_TaxonConcept_Name] FOREIGN KEY([NameId])
REFERENCES test_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name.[TaxonConcept] NOCHECK CONSTRAINT [FK_TaxonConcept_Name]

/****** Object:  ForeignKey [FK_TaxonConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[TaxonConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_TaxonConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES test_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE test_name.[TaxonConcept] NOCHECK CONSTRAINT [FK_TaxonConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[VernacularConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_VernacularConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES test_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE test_name.[VernacularConcept] NOCHECK CONSTRAINT [FK_VernacularConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Vernacular]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name.[VernacularConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_VernacularConcept_Vernacular] FOREIGN KEY([NameId])
REFERENCES test_name.[Vernacular] ([VernacularId])
NOT FOR REPLICATION

ALTER TABLE test_name.[VernacularConcept] NOCHECK CONSTRAINT [FK_VernacularConcept_Vernacular]

ALTER TABLE [test_name].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_Name] FOREIGN KEY([NameId])
REFERENCES [test_name].[Name] ([NameId])

ALTER TABLE [test_name].[Annotation] CHECK CONSTRAINT [FK_Annotation_Name]

ALTER TABLE [test_name].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_Publication] FOREIGN KEY([AccordingToId])
REFERENCES [test_name].[Publication] ([PublicationID])

ALTER TABLE [test_name].[Annotation] CHECK CONSTRAINT [FK_Annotation_Publication]

ALTER TABLE [test_name].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_TaxonConcept] FOREIGN KEY([ConceptId])
REFERENCES [test_name].[TaxonConcept] ([TaxonConceptId])

ALTER TABLE [test_name].[Annotation] CHECK CONSTRAINT [FK_Annotation_TaxonConcept]



--------------------------------
--Fungi names

/****** Object:  ForeignKey [FK_Biostatus_Name]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name_2.[Biostatus] DROP CONSTRAINT [FK_Biostatus_Name]

/****** Object:  ForeignKey [FK_Biostatus_Publication]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name_2.[Biostatus] DROP CONSTRAINT [FK_Biostatus_Publication]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name_2.[ConceptApplication] DROP CONSTRAINT [FK_ConceptApplication_VernacularConcept]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name_2.[ConceptApplication] DROP CONSTRAINT [FK_ConceptApplication_VernacularConcept1]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name_2.[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationship_TaxonConcept]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name_2.[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationship_TaxonConcept1]

/****** Object:  ForeignKey [FK_Name_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[Name] DROP CONSTRAINT [FK_Name_Name]

/****** Object:  ForeignKey [FK_Name_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[Name] DROP CONSTRAINT [FK_Name_Name1]

/****** Object:  ForeignKey [FK_Name_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[Name] DROP CONSTRAINT [FK_Name_Name2]

/****** Object:  ForeignKey [FK_Name_Name3]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[Name] DROP CONSTRAINT [FK_Name_Name3]

/****** Object:  ForeignKey [FK_Name_Name4]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[Name] DROP CONSTRAINT [FK_Name_Name4]

/****** Object:  ForeignKey [FK_Name_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[Name] DROP CONSTRAINT [FK_Name_Publication]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name1]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name2]

/****** Object:  ForeignKey [FK_NameBasedConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Publication]

/****** Object:  ForeignKey [FK_PublicationTitle_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[PublicationTitle] DROP CONSTRAINT [FK_PublicationTitle_Publication]

/****** Object:  ForeignKey [FK_TaxonConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[TaxonConcept] DROP CONSTRAINT [FK_TaxonConcept_Name]

/****** Object:  ForeignKey [FK_TaxonConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[TaxonConcept] DROP CONSTRAINT [FK_TaxonConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[VernacularConcept] DROP CONSTRAINT [FK_VernacularConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Vernacular]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[VernacularConcept] DROP CONSTRAINT [FK_VernacularConcept_Vernacular]

ALTER TABLE [test_name_2].[Annotation]  drop CONSTRAINT [FK_Annotation_Name] 

ALTER TABLE [test_name_2].[Annotation]  drop CONSTRAINT [FK_Annotation_Publication] 

ALTER TABLE [test_name_2].[Annotation]  drop CONSTRAINT [FK_Annotation_TaxonConcept] ]



--truncate
truncate table test_name_2.OAI
truncate table test_name_2.deprecated
truncate table test_name_2.biostatus
truncate table test_name_2.annotation
truncate table test_name_2.conceptapplication
truncate table test_name_2.conceptRelationship
truncate table test_name_2.vernacularconcept
truncate table test_name_2.Vernacular
truncate table test_name_2.taxonConcept
truncate table test_name_2.TaxonNameUse
truncate table test_name_2.VernacularUse
truncate table test_name_2.Name
truncate table test_name_2.publicationtitle
truncate table test_name_2.publication
truncate table test_name_2.metadata



--recreate constraints

/****** Object:  ForeignKey [FK_Biostatus_Name]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name_2.[Biostatus]  WITH NOCHECK ADD  CONSTRAINT [FK_Biostatus_Name] FOREIGN KEY([NameId])
REFERENCES test_name_2.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[Biostatus] NOCHECK CONSTRAINT [FK_Biostatus_Name]

/****** Object:  ForeignKey [FK_Biostatus_Publication]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name_2.[Biostatus]  WITH NOCHECK ADD  CONSTRAINT [FK_Biostatus_Publication] FOREIGN KEY([AccordingToId])
REFERENCES test_name_2.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[Biostatus] NOCHECK CONSTRAINT [FK_Biostatus_Publication]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name_2.[ConceptApplication]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptApplication_VernacularConcept] FOREIGN KEY([FromConceptId])
REFERENCES test_name_2.[VernacularConcept] ([VernacularConceptId])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[ConceptApplication] NOCHECK CONSTRAINT [FK_ConceptApplication_VernacularConcept]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name_2.[ConceptApplication]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptApplication_VernacularConcept1] FOREIGN KEY([ToConceptId])
REFERENCES test_name_2.[VernacularConcept] ([VernacularConceptId])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[ConceptApplication] NOCHECK CONSTRAINT [FK_ConceptApplication_VernacularConcept1]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name_2.[ConceptRelationship]  WITH CHECK ADD  CONSTRAINT [FK_ConceptRelationship_TaxonConcept] FOREIGN KEY([FromConceptId])
REFERENCES test_name_2.[TaxonConcept] ([TaxonConceptId])

ALTER TABLE test_name_2.[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_TaxonConcept]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE test_name_2.[ConceptRelationship]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptRelationship_TaxonConcept1] FOREIGN KEY([ToConceptId])
REFERENCES test_name_2.[TaxonConcept] ([TaxonConceptId])

ALTER TABLE test_name_2.[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_TaxonConcept1]

/****** Object:  ForeignKey [FK_Name_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name] FOREIGN KEY([TypeNameID])
REFERENCES test_name_2.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[Name] NOCHECK CONSTRAINT [FK_Name_Name]

/****** Object:  ForeignKey [FK_Name_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name1] FOREIGN KEY([BasionymID])
REFERENCES test_name_2.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[Name] NOCHECK CONSTRAINT [FK_Name_Name1]

/****** Object:  ForeignKey [FK_Name_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name2] FOREIGN KEY([LaterHomonymOfId])
REFERENCES test_name_2.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[Name] NOCHECK CONSTRAINT [FK_Name_Name2]

/****** Object:  ForeignKey [FK_Name_Name3]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name3] FOREIGN KEY([BlockedNameId])
REFERENCES test_name_2.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[Name] NOCHECK CONSTRAINT [FK_Name_Name3]

/****** Object:  ForeignKey [FK_Name_Name4]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name4] FOREIGN KEY([RecombinedNameId])
REFERENCES test_name_2.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[Name] NOCHECK CONSTRAINT [FK_Name_Name4]

/****** Object:  ForeignKey [FK_Name_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Publication] FOREIGN KEY([PublishedInID])
REFERENCES test_name_2.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[Name] NOCHECK CONSTRAINT [FK_Name_Publication]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name] FOREIGN KEY([NameId])
REFERENCES test_name_2.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name1] FOREIGN KEY([AcceptedNameId])
REFERENCES test_name_2.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name1]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name2] FOREIGN KEY([ParentNameId])
REFERENCES test_name_2.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name2]

/****** Object:  ForeignKey [FK_NameBasedConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES test_name_2.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Publication]

/****** Object:  ForeignKey [FK_PublicationTitle_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[PublicationTitle]  WITH NOCHECK ADD  CONSTRAINT [FK_PublicationTitle_Publication] FOREIGN KEY([PublicationId])
REFERENCES test_name_2.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[PublicationTitle] NOCHECK CONSTRAINT [FK_PublicationTitle_Publication]

/****** Object:  ForeignKey [FK_TaxonConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[TaxonConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_TaxonConcept_Name] FOREIGN KEY([NameId])
REFERENCES test_name_2.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[TaxonConcept] NOCHECK CONSTRAINT [FK_TaxonConcept_Name]

/****** Object:  ForeignKey [FK_TaxonConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[TaxonConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_TaxonConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES test_name_2.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[TaxonConcept] NOCHECK CONSTRAINT [FK_TaxonConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[VernacularConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_VernacularConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES test_name_2.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[VernacularConcept] NOCHECK CONSTRAINT [FK_VernacularConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Vernacular]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE test_name_2.[VernacularConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_VernacularConcept_Vernacular] FOREIGN KEY([NameId])
REFERENCES test_name_2.[Vernacular] ([VernacularId])
NOT FOR REPLICATION

ALTER TABLE test_name_2.[VernacularConcept] NOCHECK CONSTRAINT [FK_VernacularConcept_Vernacular]


ALTER TABLE [test_name_2].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_Name] FOREIGN KEY([NameId])
REFERENCES [test_name_2].[Name] ([NameId])

ALTER TABLE [test_name_2].[Annotation] CHECK CONSTRAINT [FK_Annotation_Name]

ALTER TABLE [test_name_2].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_Publication] FOREIGN KEY([AccordingToId])
REFERENCES [test_name_2].[Publication] ([PublicationID])

ALTER TABLE [test_name_2].[Annotation] CHECK CONSTRAINT [FK_Annotation_Publication]

ALTER TABLE [test_name_2].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_TaxonConcept] FOREIGN KEY([ConceptId])
REFERENCES [test_name_2].[TaxonConcept] ([TaxonConceptId])

ALTER TABLE [test_name_2].[Annotation] CHECK CONSTRAINT [FK_Annotation_TaxonConcept]
