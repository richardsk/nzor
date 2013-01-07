IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.[HostedCache0]') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.[HostedCache0]
go

SET ANSI_NULLS ON
go

SET QUOTED_IDENTIFIER ON
go

CREATE procedure dbo.[HostedCache0]
as



/****** Object:  ForeignKey [FK_Biostatus_Name]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE Hosted_name.[Biostatus] DROP CONSTRAINT [FK_Biostatus_Name]

/****** Object:  ForeignKey [FK_Biostatus_Publication]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE Hosted_name.[Biostatus] DROP CONSTRAINT [FK_Biostatus_Publication]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE Hosted_name.[ConceptApplication] DROP CONSTRAINT [FK_ConceptApplication_VernacularConcept]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE Hosted_name.[ConceptApplication] DROP CONSTRAINT [FK_ConceptApplication_VernacularConcept1]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE Hosted_name.[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationship_TaxonConcept]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE Hosted_name.[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationship_TaxonConcept1]

/****** Object:  ForeignKey [FK_Name_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[Name] DROP CONSTRAINT [FK_Name_Name]

/****** Object:  ForeignKey [FK_Name_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[Name] DROP CONSTRAINT [FK_Name_Name1]

/****** Object:  ForeignKey [FK_Name_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[Name] DROP CONSTRAINT [FK_Name_Name2]

/****** Object:  ForeignKey [FK_Name_Name3]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[Name] DROP CONSTRAINT [FK_Name_Name3]

/****** Object:  ForeignKey [FK_Name_Name4]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[Name] DROP CONSTRAINT [FK_Name_Name4]

/****** Object:  ForeignKey [FK_Name_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[Name] DROP CONSTRAINT [FK_Name_Publication]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name1]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Name2]

/****** Object:  ForeignKey [FK_NameBasedConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[TaxonNameUse] DROP CONSTRAINT [FK_NameBasedConcept_Publication]

/****** Object:  ForeignKey [FK_PublicationTitle_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[PublicationTitle] DROP CONSTRAINT [FK_PublicationTitle_Publication]

/****** Object:  ForeignKey [FK_TaxonConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[TaxonConcept] DROP CONSTRAINT [FK_TaxonConcept_Name]

/****** Object:  ForeignKey [FK_TaxonConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[TaxonConcept] DROP CONSTRAINT [FK_TaxonConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[VernacularConcept] DROP CONSTRAINT [FK_VernacularConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Vernacular]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[VernacularConcept] DROP CONSTRAINT [FK_VernacularConcept_Vernacular]

ALTER TABLE [hosted_name].[Annotation]  drop CONSTRAINT [FK_Annotation_Name] 

ALTER TABLE [hosted_name].[Annotation]  drop CONSTRAINT [FK_Annotation_Publication] 

ALTER TABLE [hosted_name].[Annotation]  drop CONSTRAINT [FK_Annotation_TaxonConcept] 



--truncate
truncate table Hosted_name.OAI
truncate table Hosted_name.deprecated
truncate table Hosted_name.biostatus
truncate table hosted_name.annotation
truncate table Hosted_name.conceptapplication
truncate table Hosted_name.conceptRelationship
truncate table Hosted_name.vernacularconcept
truncate table Hosted_name.Vernacular
truncate table Hosted_name.vernacularuse
truncate table Hosted_name.taxonConcept
truncate table Hosted_name.TaxonNameUse
truncate table Hosted_name.Name
truncate table Hosted_name.publicationtitle
truncate table Hosted_name.publication
truncate table Hosted_name.metadata



--recreate constraints

/****** Object:  ForeignKey [FK_Biostatus_Name]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE Hosted_name.[Biostatus]  WITH NOCHECK ADD  CONSTRAINT [FK_Biostatus_Name] FOREIGN KEY([NameId])
REFERENCES Hosted_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[Biostatus] NOCHECK CONSTRAINT [FK_Biostatus_Name]

/****** Object:  ForeignKey [FK_Biostatus_Publication]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE Hosted_name.[Biostatus]  WITH NOCHECK ADD  CONSTRAINT [FK_Biostatus_Publication] FOREIGN KEY([AccordingToId])
REFERENCES Hosted_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[Biostatus] NOCHECK CONSTRAINT [FK_Biostatus_Publication]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE Hosted_name.[ConceptApplication]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptApplication_VernacularConcept] FOREIGN KEY([FromConceptId])
REFERENCES Hosted_name.[VernacularConcept] ([VernacularConceptId])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[ConceptApplication] NOCHECK CONSTRAINT [FK_ConceptApplication_VernacularConcept]

/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE Hosted_name.[ConceptApplication]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptApplication_VernacularConcept1] FOREIGN KEY([ToConceptId])
REFERENCES Hosted_name.[VernacularConcept] ([VernacularConceptId])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[ConceptApplication] NOCHECK CONSTRAINT [FK_ConceptApplication_VernacularConcept1]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE Hosted_name.[ConceptRelationship]  WITH CHECK ADD  CONSTRAINT [FK_ConceptRelationship_TaxonConcept] FOREIGN KEY([FromConceptId])
REFERENCES Hosted_name.[TaxonConcept] ([TaxonConceptId])

ALTER TABLE Hosted_name.[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_TaxonConcept]

/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept1]    Script Date: 01/14/2011 10:53:07 ******/
ALTER TABLE Hosted_name.[ConceptRelationship]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptRelationship_TaxonConcept1] FOREIGN KEY([ToConceptId])
REFERENCES Hosted_name.[TaxonConcept] ([TaxonConceptId])

ALTER TABLE Hosted_name.[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_TaxonConcept1]

/****** Object:  ForeignKey [FK_Name_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name] FOREIGN KEY([TypeNameID])
REFERENCES Hosted_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name]

/****** Object:  ForeignKey [FK_Name_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name1] FOREIGN KEY([BasionymID])
REFERENCES Hosted_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name1]

/****** Object:  ForeignKey [FK_Name_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name2] FOREIGN KEY([LaterHomonymOfId])
REFERENCES Hosted_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name2]

/****** Object:  ForeignKey [FK_Name_Name3]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name3] FOREIGN KEY([BlockedNameId])
REFERENCES Hosted_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name3]

/****** Object:  ForeignKey [FK_Name_Name4]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name4] FOREIGN KEY([RecombinedNameId])
REFERENCES Hosted_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[Name] NOCHECK CONSTRAINT [FK_Name_Name4]

/****** Object:  ForeignKey [FK_Name_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Publication] FOREIGN KEY([PublishedInID])
REFERENCES Hosted_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[Name] NOCHECK CONSTRAINT [FK_Name_Publication]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name] FOREIGN KEY([NameId])
REFERENCES Hosted_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name1]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name1] FOREIGN KEY([AcceptedNameId])
REFERENCES Hosted_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name1]

/****** Object:  ForeignKey [FK_NameBasedConcept_Name2]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name2] FOREIGN KEY([ParentNameId])
REFERENCES Hosted_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name2]

/****** Object:  ForeignKey [FK_NameBasedConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES Hosted_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Publication]

/****** Object:  ForeignKey [FK_PublicationTitle_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[PublicationTitle]  WITH NOCHECK ADD  CONSTRAINT [FK_PublicationTitle_Publication] FOREIGN KEY([PublicationId])
REFERENCES Hosted_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[PublicationTitle] NOCHECK CONSTRAINT [FK_PublicationTitle_Publication]

/****** Object:  ForeignKey [FK_TaxonConcept_Name]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[TaxonConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_TaxonConcept_Name] FOREIGN KEY([NameId])
REFERENCES Hosted_name.[Name] ([NameId])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[TaxonConcept] NOCHECK CONSTRAINT [FK_TaxonConcept_Name]

/****** Object:  ForeignKey [FK_TaxonConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[TaxonConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_TaxonConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES Hosted_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[TaxonConcept] NOCHECK CONSTRAINT [FK_TaxonConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Publication]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[VernacularConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_VernacularConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES Hosted_name.[Publication] ([PublicationID])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[VernacularConcept] NOCHECK CONSTRAINT [FK_VernacularConcept_Publication]

/****** Object:  ForeignKey [FK_VernacularConcept_Vernacular]    Script Date: 01/14/2011 10:53:08 ******/
ALTER TABLE Hosted_name.[VernacularConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_VernacularConcept_Vernacular] FOREIGN KEY([NameId])
REFERENCES Hosted_name.[Vernacular] ([VernacularId])
NOT FOR REPLICATION

ALTER TABLE Hosted_name.[VernacularConcept] NOCHECK CONSTRAINT [FK_VernacularConcept_Vernacular]


ALTER TABLE [hosted_name].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_Name] FOREIGN KEY([NameId])
REFERENCES [hosted_name].[Name] ([NameId])

ALTER TABLE [hosted_name].[Annotation] CHECK CONSTRAINT [FK_Annotation_Name]

ALTER TABLE [hosted_name].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_Publication] FOREIGN KEY([AccordingToId])
REFERENCES [hosted_name].[Publication] ([PublicationID])

ALTER TABLE [hosted_name].[Annotation] CHECK CONSTRAINT [FK_Annotation_Publication]

ALTER TABLE [hosted_name].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_TaxonConcept] FOREIGN KEY([ConceptId])
REFERENCES [hosted_name].[TaxonConcept] ([TaxonConceptId])

ALTER TABLE [hosted_name].[Annotation] CHECK CONSTRAINT [FK_Annotation_TaxonConcept]


go

grant execute on dbo.[HostedCache0] to dbi_user

go
