CREATE TABLE [dbo].[Author]
(
	[AuthorId] [uniqueidentifier] NOT NULL,
	[CorrectAuthorId] [uniqueidentifier] NULL,
	[Abbreviation] [nvarchar](250) NOT NULL,
	[FirstName] [nvarchar](250) NULL,
	[LastName] [nvarchar](250) NULL,
	[TaxonGroups] [nvarchar](250) NULL,
	[DateRange] [nvarchar](250) NULL,
	[AlternativeNames] [nvarchar](max) NULL
)

