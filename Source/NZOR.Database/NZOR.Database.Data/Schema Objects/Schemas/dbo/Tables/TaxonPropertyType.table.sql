CREATE TABLE 
	[dbo].[TaxonPropertyType]
	(
	TaxonPropertyTypeID UNIQUEIDENTIFIER NOT NULL, 
	
	TaxonPropertyClassID UNIQUEIDENTIFIER NOT NULL, 
	
	Name NVARCHAR(150) NOT NULL, 
	Description NVARCHAR(MAX) NULL
	)
