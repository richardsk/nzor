CREATE TABLE provider.Annotation
(
	AnnotationID uniqueidentifier not null,
	NameID uniqueidentifier null,
	ConceptID uniqueidentifier null,
	ReferenceID uniqueidentifier null,
	DataSourceID uniqueidentifier not null,
	ConsensusAnnotationID uniqueidentifier null,
		
	AnnotationType nvarchar(250) null,
	AnnotationText nvarchar(max) not null,
	

	ProviderNameID NVARCHAR(1000) NULL, 
	ProviderConceptID NVARCHAR(1000) NULL, 
	ProviderReferenceID NVARCHAR(1000) NULL, 
	ProviderRecordID NVARCHAR(1000) NOT NULL, 
	ProviderCreatedDate DATETIME NULL, 
	ProviderModifiedDate DATETIME NULL, 
		
	AddedDate DATETIME NULL,
	ModifiedDate DATETIME NULL
)
