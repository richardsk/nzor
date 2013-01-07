INSERT INTO consensus.[Annotation]
           ([AnnotationID]
           ,[NameID]
           ,[ConceptID]
           ,[ReferenceID]
           ,[AnnotationType]
           ,[AnnotationText]
           ,[AddedDate]
           ,[ModifiedDate])
     VALUES
           (@AnnotationID
           ,@NameID
           ,@ConceptID
           ,@ReferenceID
           ,@AnnotationType
           ,@AnnotationText
           ,@AddedDate
           ,@ModifiedDate)


