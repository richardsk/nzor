CREATE TABLE [dbo].NamePropertyType
(
	NamePropertyTypeID UNIQUEIDENTIFIER NOT NULL, 
	
	NameClassID UNIQUEIDENTIFIER NOT NULL, 
	
	Name NVARCHAR(50) NOT NULL, 
	Type NVARCHAR(10) NULL, 
	MinOccurrences INT NULL, 
	MaxOccurrences INT NULL, 
	GoverningCode NVARCHAR(5) NULL
)
