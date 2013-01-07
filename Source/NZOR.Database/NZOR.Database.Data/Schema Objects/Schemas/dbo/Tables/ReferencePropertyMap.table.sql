CREATE TABLE 
	[dbo].[ReferencePropertyMap]
	(
	ReferencePropertyMapID UNIQUEIDENTIFIER NOT NULL, 
	
	ReferenceTypeID UNIQUEIDENTIFIER NOT NULL, 
	ReferencePropertyTypeID UNIQUEIDENTIFIER NOT NULL, 
	
	MinOccurrences INT NULL, 
	MaxOccurrences INT NULL,
	Sequence INT NULL, 
	Label NVARCHAR(150) NULL
	)
