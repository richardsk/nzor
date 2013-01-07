UPDATE consensus.[Annotation]
   SET [NameID] = @NameID
      ,[ConceptID] = @ConceptID
      ,[ReferenceID] = @ReferenceID
      ,[AnnotationType] = @AnnotationType
      ,[AnnotationText] = @AnnotationText
      ,[AddedDate] = @AddedDate
      ,[ModifiedDate] = @ModifiedDate
 WHERE [AnnotationID] = @AnnotationID


