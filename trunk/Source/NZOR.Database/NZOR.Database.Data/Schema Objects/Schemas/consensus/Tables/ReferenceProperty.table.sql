CREATE TABLE 
	[consensus].[ReferenceProperty]
	(
	ReferencePropertyID UNIQUEIDENTIFIER NOT NULL, 
	
	ReferenceID UNIQUEIDENTIFIER NOT NULL, 
	ReferencePropertyTypeID UNIQUEIDENTIFIER NOT NULL, 
	
	Value NVARCHAR(MAX) NOT NULL
	)
