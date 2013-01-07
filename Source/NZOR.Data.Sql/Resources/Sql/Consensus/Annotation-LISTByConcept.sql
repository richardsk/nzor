SELECT [AnnotationID]
      ,a.[NameID]
      ,a.[ConceptID]
      ,a.[ReferenceID]
      ,[AnnotationType]
      ,[AnnotationText]
      ,a.[AddedDate]
      ,a.[ModifiedDate]
from consensus.Annotation a
WHERE 
	a.ConceptID = @ConceptID

