CREATE TABLE 
	[dbo].[TaxonRank]
	(
	TaxonRankID UNIQUEIDENTIFIER NOT NULL,

	AncestorRankID UNIQUEIDENTIFIER NULL,
	MatchRuleSetID INT NULL,

	Name NVARCHAR(100) NOT NULL,
	DisplayName nvarchar(100) not null,
	KnownAbbreviations NVARCHAR(200) NULL,
	SortOrder INT NULL,
	IncludeInFullName BIT NULL,
	ShowRank bit null,
	GoverningCode nvarchar(10) null,

	AddedBy NVARCHAR(100) NOT NULL,
	AddedDate DATETIME NOT NULL,
	ModifiedBy NVARCHAR(100) NULL,
	ModifiedDate DATETIME NULL
	)
