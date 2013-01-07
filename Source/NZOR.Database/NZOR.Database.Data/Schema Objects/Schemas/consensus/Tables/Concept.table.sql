CREATE TABLE 
	[consensus].[Concept]
	(
	ConceptID UNIQUEIDENTIFIER NOT NULL, 
	
	NameID UNIQUEIDENTIFIER NOT NULL, 
	AccordingToReferenceID UNIQUEIDENTIFIER NULL, 
	
	Orthography NVARCHAR(1000) NULL, 
	TaxonRank NVARCHAR(100) NULL, 
	HigherClassification NVARCHAR(MAX) NULL, 
	NameQualifier nvarchar(250) null,
	
	AddedDate DATETIME NULL, 
	ModifiedDate DATETIME NULL
	)
