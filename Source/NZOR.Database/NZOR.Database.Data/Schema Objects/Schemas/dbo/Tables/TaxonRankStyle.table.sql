CREATE TABLE [dbo].[TaxonRankStyle]
(
	TaxonRankID uniqueidentifier not null,
	GoverningCode nvarchar(5) not null,
	Style int not null
)
