CREATE TABLE 
	[dbo].[NamePropertyLookUp]
	(
	NamePropertyLookUpID UNIQUEIDENTIFIER NOT NULL,  

	NameClassPropertyID UNIQUEIDENTIFIER NOT NULL, 
	ParentNamePropertyLookUpID UNIQUEIDENTIFIER NULL, 
	
	Value NVARCHAR(1000) NULL, 
	AlternativeStrings XML NULL, 
	SortOrder INT NULL
	)
