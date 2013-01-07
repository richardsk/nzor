CREATE TABLE consensus.[Annotation]
(
	AnnotationID uniqueidentifier not null,
	NameID uniqueidentifier null,
	ConceptID uniqueidentifier null,
	ReferenceID uniqueidentifier null,
		
	AnnotationType nvarchar(250) null,
	AnnotationText nvarchar(max) not null,
			
	AddedDate DATETIME NULL,
	ModifiedDate DATETIME NULL
)
