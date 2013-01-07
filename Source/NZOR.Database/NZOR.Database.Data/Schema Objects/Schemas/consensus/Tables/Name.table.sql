CREATE TABLE [consensus].[Name]
(
	NameID UNIQUEIDENTIFIER NOT NULL, 
	
	TaxonRankID UNIQUEIDENTIFIER NOT NULL, 
	NameClassID UNIQUEIDENTIFIER NOT NULL, 

	FullName NVARCHAR(500) NOT NULL,
	GoverningCode NVARCHAR(5) NULL, 
	IsRecombination bit null,
	
	AddedDate DATETIME NULL, 
	ModifiedDate DATETIME NULL
)
