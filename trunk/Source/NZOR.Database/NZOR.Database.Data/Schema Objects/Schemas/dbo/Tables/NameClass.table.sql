CREATE TABLE 
	[dbo].[NameClass]
	(
	NameClassID UNIQUEIDENTIFIER NOT NULL, 

	Name NVARCHAR(50) NOT NULL,
	Description NVARCHAR(MAX) NOT NULL,
	HasClassification bit not null default(0)
	)
