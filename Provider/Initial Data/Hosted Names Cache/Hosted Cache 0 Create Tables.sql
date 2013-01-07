USE [Name_Cache]
GO
/****** Object:  Table [hosted_name].[VernacularUse]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hosted_name].[VernacularUse](
	[VernacularUseId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[VernacularId] [uniqueidentifier] NOT NULL,
	[AccordingToId] [uniqueidentifier] NULL,
	[TaxonNameId] [uniqueidentifier] NOT NULL,
	[Rank] [nvarchar](50) NULL,
	[Gender] [nvarchar](100) NULL,
	[PartOfTaxon] [nvarchar](100) NULL,
	[GeoRegion] [nvarchar](500) NULL,
	[GeographicSchema] [nvarchar](500) NULL,
	[LifeStage] [nvarchar](100) NULL,
 CONSTRAINT [PK_VernacularUse] PRIMARY KEY CLUSTERED 
(
	[VernacularUseId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [hosted_name].[Vernacular]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hosted_name].[Vernacular](
	[VernacularId] [uniqueidentifier] NOT NULL,
	[NameFull] [nvarchar](1000) NULL,
	[PublishedIn] [nvarchar](max) NULL,
	[PublishedInId] [uniqueidentifier] NULL,
	[Language] [nvarchar](50) NULL,
	[Country] [nvarchar](150) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Vernacular] PRIMARY KEY CLUSTERED 
(
	[VernacularId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [hosted_name].[Metadata]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hosted_name].[Metadata](
	[ProviderId] [uniqueidentifier] NULL,
	[Name] [nvarchar](200) NULL,
	[ProviderCode] [nvarchar](50) NULL,
	[DataSourceId] [nvarchar](100) NULL,
	[OrganisationURL] [nvarchar](200) NULL,
	[Disclaimer] [nvarchar](max) NULL,
	[Attribution] [nvarchar](max) NULL,
	[Licensing] [nvarchar](max) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [hosted_name].[OAI]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [hosted_name].[OAI](
	[OAI_ID] [uniqueidentifier] NOT NULL,
	[TableName] [varchar](50) NOT NULL,
	[TableId] [varchar](500) NOT NULL,
	[AddedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[Counter] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_OAI_ID] PRIMARY KEY CLUSTERED 
(
	[OAI_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [hosted_name].[Publication]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [hosted_name].[Publication](
	[PublicationID] [uniqueidentifier] NOT NULL,
	[ParentPublicationId] [uniqueidentifier] NULL,
	[Type] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[Citation] [nvarchar](max) NULL,
	[AuthorsSimple] [nvarchar](max) NULL,
	[DateOfPublication] [nvarchar](100) NULL,
	[DateOnPublication] [nvarchar](100) NULL,
	[EditorsSimple] [nchar](10) NULL,
	[Volume] [varchar](50) NULL,
	[Issue] [varchar](150) NULL,
	[Edition] [varchar](50) NULL,
	[PageStart] [varchar](50) NULL,
	[PageEnd] [varchar](50) NULL,
	[PageTotal] [varchar](50) NULL,
	[PublisherName] [nvarchar](150) NULL,
	[PublisherCity] [nvarchar](150) NULL,
 CONSTRAINT [PK_Publication] PRIMARY KEY CLUSTERED 
(
	[PublicationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [hosted_name].[Deprecated]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hosted_name].[Deprecated](
	[DeprecatedOldId] [nvarchar](50) NOT NULL,
	[DeprecatedNewId] [nvarchar](50) NULL,
	[TableName] [nvarchar](100) NOT NULL,
	[DeprecatedDate] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [hosted_name].[VernacularConcept]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hosted_name].[VernacularConcept](
	[VernacularConceptId] [nvarchar](200) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[Name] [nvarchar](1000) NULL,
	[NameId] [uniqueidentifier] NULL,
	[AccordingTo] [nvarchar](1000) NULL,
	[AccordingToId] [uniqueidentifier] NULL,
	[Rank] [nvarchar](50) NULL,
 CONSTRAINT [PK_VernacularConcept] PRIMARY KEY CLUSTERED 
(
	[VernacularConceptId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [hosted_name].[Name]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hosted_name].[Name](
	[NameId] [uniqueidentifier] NOT NULL,
	[IsScientific] [bit] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[Namefull] [nvarchar](500) NULL,
	[NamePart] [nvarchar](500) NULL,
	[NameHTML] [nvarchar](500) NULL,
	[Rank] [nvarchar](100) NULL,
	[CanonicalName] [nvarchar](500) NULL,
	[Authorship] [nvarchar](500) NULL,
	[BasionymAuthors] [nvarchar](500) NULL,
	[CombiningAuthors] [nvarchar](500) NULL,
	[PublishedIn] [nvarchar](1500) NULL,
	[PublishedInID] [uniqueidentifier] NULL,
	[Year] [nvarchar](500) NULL,
	[MicroReference] [nvarchar](500) NULL,
	[TypeName] [nvarchar](500) NULL,
	[TypeNameID] [uniqueidentifier] NULL,
	[ProtologueOrthography] [nvarchar](500) NULL,
	[Basionym] [nvarchar](500) NULL,
	[BasionymID] [uniqueidentifier] NULL,
	[LaterHomonymOf] [nvarchar](500) NULL,
	[LaterHomonymOfId] [uniqueidentifier] NULL,
	[NomenclaturalStatus] [nvarchar](max) NULL,
	[NomenclaturalCode] [nvarchar](20) NULL,
	[Lanuage] [nvarchar](100) NULL,
	[Country] [nvarchar](100) NULL,
	[QualityCode] [nvarchar](50) NULL,
	[QualityStatement] [nvarchar](max) NULL,
	[BlockedName] [nvarchar](500) NULL,
	[BlockedNameId] [uniqueidentifier] NULL,
	[RecombinedName] [nvarchar](500) NULL,
	[RecombinedNameId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Name] PRIMARY KEY CLUSTERED 
(
	[NameId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [hosted_name].[TaxonNameUse]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hosted_name].[TaxonNameUse](
	[TaxonNameUseId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[NameId] [uniqueidentifier] NOT NULL,
	[AccordingToId] [uniqueidentifier] NULL,
	[AcceptedNameId] [uniqueidentifier] NULL,
	[ParentNameId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_NameBasedConcept] PRIMARY KEY CLUSTERED 
(
	[TaxonNameUseId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [hosted_name].[TaxonConcept]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [hosted_name].[TaxonConcept](
	[TaxonConceptId] [nvarchar](200) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[Name] [nvarchar](1000) NULL,
	[NameId] [uniqueidentifier] NULL,
	[AccordingTo] [nvarchar](1000) NULL,
	[AccordingToId] [uniqueidentifier] NULL,
	[Rank] [varchar](150) NULL,
	[AcceptedConceptID] [uniqueidentifier] NULL,
	[AcceptedConceptInUse] [bit] NULL,
	[ParentConceptId] [uniqueidentifier] NULL,
	[ParentConceptInUse] [bit] NULL,
 CONSTRAINT [PK_Concept] PRIMARY KEY CLUSTERED 
(
	[TaxonConceptId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [hosted_name].[Biostatus]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hosted_name].[Biostatus](
	[BiostatusID] [int] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[Taxon] [nvarchar](500) NULL,
	[ConceptID] [uniqueidentifier] NULL,
	[NameId] [uniqueidentifier] NULL,
	[AccordingTo] [nvarchar](1000) NULL,
	[AccordingToId] [uniqueidentifier] NULL,
	[Region] [nvarchar](500) NULL,
	[GeographicSchema] [nvarchar](500) NULL,
	[Biome] [nvarchar](150) NULL,
	[EnvironmentalContext] [nvarchar](100) NULL,
	[Origin] [nvarchar](100) NULL,
	[Occurrence] [nvarchar](100) NULL,
 CONSTRAINT [PK_Biostatus] PRIMARY KEY CLUSTERED 
(
	[BiostatusID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [hosted_name].[PublicationTitle]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [hosted_name].[PublicationTitle](
	[PublicationId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](500) NULL,
	[Level] [int] NULL,
	[type] [varchar](50) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [hosted_name].[ConceptApplication]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [hosted_name].[ConceptApplication](
	[ConceptApplicationId] [nvarchar](200) NULL,
	[FromConceptId] [nvarchar](200) NULL,
	[ToConceptId] [nvarchar](200) NULL,
	[ToNameId] nvarchar(200) null,
	[Gender] [nvarchar](100) NULL,
	[PartOfTaxon] [nvarchar](100) NULL,
	[GeoRegion] [nvarchar](500) NULL,
	[GeographicSchema] [nvarchar](500) NULL,
	[LifeStage] [nvarchar](100) NULL,
	[InUse] [bit] NULL,
	[Type] [varchar](20) NULL,
	[RowId] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_ConceptApplication] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [hosted_name].[ManagementStatus](
	[ManagementStatusId] [int] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[Taxon] [nvarchar](500) NULL,
	[ConceptId] [uniqueidentifier] NULL,
	[NameId] [uniqueidentifier] NULL,
	[AccordingTo] [nvarchar](2000) NULL,
	[Region] [nvarchar](500) NULL,
	[GeographicSchema] [nvarchar](500) NULL,
	[Biome] [nvarchar](150) NULL,
	[Action] [nvarchar](250) NULL,
	[Outcome] [nvarchar](250) NULL,
	[ActionStatus] [nvarchar](250) NULL,
	[Status] [nvarchar](250) NULL,
 CONSTRAINT [PK_ManagementStatus] PRIMARY KEY CLUSTERED 
(
	[ManagementStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



SET ANSI_PADDING OFF
GO
/****** Object:  Table [hosted_name].[ConceptRelationship]    Script Date: 11/03/2011 16:29:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [hosted_name].[ConceptRelationship](
	[ConceptRelationshipID] [nvarchar](200) NOT NULL,
	[FromConceptId] [nvarchar](200) NULL,
	[ToConceptId] [nvarchar](200) NULL,
	[Type] [varchar](50) NULL,
	[rank] [nvarchar](50) NULL,
	[InUse] [bit] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Default [DF_VernacularUse_VernacularUseId]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[VernacularUse] ADD  CONSTRAINT [DF_VernacularUse_VernacularUseId]  DEFAULT (newid()) FOR [VernacularUseId]
GO
/****** Object:  ForeignKey [FK_Biostatus_Name]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[Biostatus]  WITH NOCHECK ADD  CONSTRAINT [FK_Biostatus_Name] FOREIGN KEY([NameId])
REFERENCES [hosted_name].[Name] ([NameId])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[Biostatus] NOCHECK CONSTRAINT [FK_Biostatus_Name]
GO
/****** Object:  ForeignKey [FK_Biostatus_Publication]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[Biostatus]  WITH NOCHECK ADD  CONSTRAINT [FK_Biostatus_Publication] FOREIGN KEY([AccordingToId])
REFERENCES [hosted_name].[Publication] ([PublicationID])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[Biostatus] NOCHECK CONSTRAINT [FK_Biostatus_Publication]
GO
/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[ConceptApplication]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptApplication_VernacularConcept] FOREIGN KEY([FromConceptId])
REFERENCES [hosted_name].[VernacularConcept] ([VernacularConceptId])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[ConceptApplication] NOCHECK CONSTRAINT [FK_ConceptApplication_VernacularConcept]
GO
/****** Object:  ForeignKey [FK_ConceptApplication_VernacularConcept1]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[ConceptApplication]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptApplication_VernacularConcept1] FOREIGN KEY([ToConceptId])
REFERENCES [hosted_name].[VernacularConcept] ([VernacularConceptId])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[ConceptApplication] NOCHECK CONSTRAINT [FK_ConceptApplication_VernacularConcept1]
GO
/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[ConceptRelationship]  WITH CHECK ADD  CONSTRAINT [FK_ConceptRelationship_TaxonConcept] FOREIGN KEY([FromConceptId])
REFERENCES [hosted_name].[TaxonConcept] ([TaxonConceptId])
GO
ALTER TABLE [hosted_name].[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_TaxonConcept]
GO
/****** Object:  ForeignKey [FK_ConceptRelationship_TaxonConcept1]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[ConceptRelationship]  WITH NOCHECK ADD  CONSTRAINT [FK_ConceptRelationship_TaxonConcept1] FOREIGN KEY([ToConceptId])
REFERENCES [hosted_name].[TaxonConcept] ([TaxonConceptId])
GO
ALTER TABLE [hosted_name].[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_TaxonConcept1]
GO
/****** Object:  ForeignKey [FK_Name_Name]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name] FOREIGN KEY([TypeNameID])
REFERENCES [hosted_name].[Name] ([NameId])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[Name] NOCHECK CONSTRAINT [FK_Name_Name]
GO
/****** Object:  ForeignKey [FK_Name_Name1]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name1] FOREIGN KEY([BasionymID])
REFERENCES [hosted_name].[Name] ([NameId])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[Name] NOCHECK CONSTRAINT [FK_Name_Name1]
GO
/****** Object:  ForeignKey [FK_Name_Name2]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name2] FOREIGN KEY([LaterHomonymOfId])
REFERENCES [hosted_name].[Name] ([NameId])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[Name] NOCHECK CONSTRAINT [FK_Name_Name2]
GO
/****** Object:  ForeignKey [FK_Name_Name3]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name3] FOREIGN KEY([BlockedNameId])
REFERENCES [hosted_name].[Name] ([NameId])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[Name] NOCHECK CONSTRAINT [FK_Name_Name3]
GO
/****** Object:  ForeignKey [FK_Name_Name4]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Name4] FOREIGN KEY([RecombinedNameId])
REFERENCES [hosted_name].[Name] ([NameId])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[Name] NOCHECK CONSTRAINT [FK_Name_Name4]
GO
/****** Object:  ForeignKey [FK_Name_Publication]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[Name]  WITH NOCHECK ADD  CONSTRAINT [FK_Name_Publication] FOREIGN KEY([PublishedInID])
REFERENCES [hosted_name].[Publication] ([PublicationID])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[Name] NOCHECK CONSTRAINT [FK_Name_Publication]
GO
/****** Object:  ForeignKey [FK_PublicationTitle_Publication]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[PublicationTitle]  WITH NOCHECK ADD  CONSTRAINT [FK_PublicationTitle_Publication] FOREIGN KEY([PublicationId])
REFERENCES [hosted_name].[Publication] ([PublicationID])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[PublicationTitle] NOCHECK CONSTRAINT [FK_PublicationTitle_Publication]
GO
/****** Object:  ForeignKey [FK_TaxonConcept_Name]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[TaxonConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_TaxonConcept_Name] FOREIGN KEY([NameId])
REFERENCES [hosted_name].[Name] ([NameId])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[TaxonConcept] NOCHECK CONSTRAINT [FK_TaxonConcept_Name]
GO
/****** Object:  ForeignKey [FK_TaxonConcept_Publication]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[TaxonConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_TaxonConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES [hosted_name].[Publication] ([PublicationID])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[TaxonConcept] NOCHECK CONSTRAINT [FK_TaxonConcept_Publication]
GO
/****** Object:  ForeignKey [FK_NameBasedConcept_Name]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name] FOREIGN KEY([NameId])
REFERENCES [hosted_name].[Name] ([NameId])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name]
GO
/****** Object:  ForeignKey [FK_NameBasedConcept_Name1]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name1] FOREIGN KEY([AcceptedNameId])
REFERENCES [hosted_name].[Name] ([NameId])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name1]
GO
/****** Object:  ForeignKey [FK_NameBasedConcept_Name2]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Name2] FOREIGN KEY([ParentNameId])
REFERENCES [hosted_name].[Name] ([NameId])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Name2]
GO
/****** Object:  ForeignKey [FK_NameBasedConcept_Publication]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[TaxonNameUse]  WITH NOCHECK ADD  CONSTRAINT [FK_NameBasedConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES [hosted_name].[Publication] ([PublicationID])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[TaxonNameUse] NOCHECK CONSTRAINT [FK_NameBasedConcept_Publication]
GO
/****** Object:  ForeignKey [FK_VernacularConcept_Publication]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[VernacularConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_VernacularConcept_Publication] FOREIGN KEY([AccordingToId])
REFERENCES [hosted_name].[Publication] ([PublicationID])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[VernacularConcept] NOCHECK CONSTRAINT [FK_VernacularConcept_Publication]
GO
/****** Object:  ForeignKey [FK_VernacularConcept_Vernacular]    Script Date: 11/03/2011 16:29:37 ******/
ALTER TABLE [hosted_name].[VernacularConcept]  WITH NOCHECK ADD  CONSTRAINT [FK_VernacularConcept_Vernacular] FOREIGN KEY([NameId])
REFERENCES [hosted_name].[Vernacular] ([VernacularId])
NOT FOR REPLICATION
GO
ALTER TABLE [hosted_name].[VernacularConcept] NOCHECK CONSTRAINT [FK_VernacularConcept_Vernacular]
GO
