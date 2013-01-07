CREATE TABLE 
	[dbo].[TaxonPropertyLookUp]
	(
	TaxonPropertyLookupID UNIQUEIDENTIFIER NOT NULL,  
	
	TaxonPropertyTypeID UNIQUEIDENTIFIER NOT NULL,
	ParentTaxonPropertyLookUpID UNIQUEIDENTIFIER NULL,
	
	Value NVARCHAR(500) NULL, 
	AlternativeStrings XML NULL
	)
