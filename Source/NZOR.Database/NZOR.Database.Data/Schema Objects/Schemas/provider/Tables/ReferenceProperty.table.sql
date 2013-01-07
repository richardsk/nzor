CREATE TABLE 
	[provider].[ReferenceProperty]
	(
	ReferencePropertyID UNIQUEIDENTIFIER NOT NULL, 
	
	ReferenceID UNIQUEIDENTIFIER NOT NULL, 
	ReferencePropertyTypeID UNIQUEIDENTIFIER NOT NULL, 
	
	SubType NVARCHAR(50) NULL,
	Sequence INT NULL,
	Level INT NULL,
	Value NVARCHAR(MAX) NULL
	)
